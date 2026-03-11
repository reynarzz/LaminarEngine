using Newtonsoft.Json;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Editor.Utils;
using Engine.Data;

namespace Editor.Cooker
{
    internal class ShipModeFilesCooker : AssetsCookerBase
    {
        /* .gfs file format
         
            - Magic (char[4])
            - Version (u32)
            - Creation Date (s64)
            
            EngineData
            [
                ProjectSettings
                [
                    Layers
                    [
                        - Layers count (s32)
                        - Layers (string[])  
                    ]
                    ScenesSettings
                    [
                        - LaunchScene (guid)
                        - Scenes count (s32)
                        - Scenes guid (guid[])
                    ],
                    PhysicsSettings
                    [
                        - Gravity (vec2)
                        - FixedTimeStep (f32)
                        - CollisionMatrix count (s32)
                        - Collision matrix (bool[])
                    ]  
                ]
            ]

            Data info    
            [
                - total assets (int32)
                - creation date (int64)
            ]
            
            Location table (n assets)
            [
                - asset block location (int64)
                - asset data (in block) location (int64)
                - asset's meta (in block) location (int64)
            }
            
            Assets block (n assets)
            [
                - asset guid (guid)
                - asset path size (int32)
                - asset path (byte[])
                - asset type (int32)
                - isCompressed (bool)
                - isEncrypted (bool)
                - asset data size (int32)
                - meta data size (int32)
                - asset data (byte[])
                - meta data (format defined by asset type)
            ]
         */

        private const long metaLocSize = sizeof(long);
        private const long assetBlockLocSize = sizeof(long);
        private const long assetDataLocSize = sizeof(long);
        private const long fieldIfOffset = assetBlockLocSize + assetDataLocSize + metaLocSize;

        private const int TEMP_BUFFER_SIZE = 81920;
        private const uint VERSION = 1;

        public ShipModeFilesCooker(Dictionary<AssetType, IAssetProcessor> processor) : base(processor)
        {
        }

        internal override async Task<bool> CookAssetsAsync(CookFileOptions fileOptions, CookingPlatform platform, (string, AssetType)[] files,
                                                            string outFolder)
        {
            bool success = false;
            Exception failureException = null;
            var path = Path.Combine(outFolder, Paths.GetAssetBuildDataFilename());
            Directory.CreateDirectory(outFolder);
            try
            {
                await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, TEMP_BUFFER_SIZE, useAsync: true);
                using var bufWritter = new BinaryWriter(fs, Encoding.UTF8, leaveOpen: true);

                // Writes header
                bufWritter.Write(Encoding.ASCII.GetBytes(AssetUtils.GFSFileFormat.HEADER));

                // Write version
                bufWritter.Write(VERSION);

                // Creation date
                bufWritter.Write(DateTime.Now.ToBinary());

                // Write engine data
                WriteEngineData(bufWritter);

                // Writes total asset files
                bufWritter.Write(files.Length);

                // creation date
                var creationDate = DateTime.Now.ToBinary();
                bufWritter.Write(creationDate);

                var currentFileIdPosition = bufWritter.BaseStream.Position;

                long currentAssetOffset = fieldIfOffset * files.Length;

                bufWritter.BaseStream.Position += currentAssetOffset;
                int count = 0;
                foreach (var (filePath, assetType) in files)
                {
                    long startAssetBlockPos = bufWritter.BaseStream.Position;

                    var meta = EditorAssetUtils.GetMetaFromAbsolutePath(filePath, assetType);
                    var relAssetPath = Paths.GetRelativeAssetPath(filePath);

                    // asset guid 
                    EditorFileUtils.WriteGuidNoAlloc(bufWritter, meta.GUID);

                    var pathBytes = Encoding.UTF8.GetBytes(relAssetPath);

                    if (fileOptions.EncryptFilesPath)
                    {
                        pathBytes = await Task.Run(() => AssetEncrypter.EncryptBytes(pathBytes, AssetUtils.ENCRYPTION_VERY_SECURE_PASSWORD));
                    }

                    // asset path size
                    bufWritter.Write(pathBytes.Length);

                    // asset path
                    bufWritter.Write(pathBytes);

                    // Writes asset type
                    bufWritter.Write((int)assetType);

                    var shouldCompressFile = fileOptions.CompressAllFiles || fileOptions.FilesToCompress.Contains(assetType);
                    var shouldEncryptFile = fileOptions.EncryptAllFiles || fileOptions.FilesToEncrypt.Contains(assetType);

                    // is compressed
                    bufWritter.Write(shouldCompressFile);

                    // is encrypted
                    bufWritter.Write(shouldEncryptFile);

                    // encrypt files path
                    bufWritter.Write(fileOptions.EncryptFilesPath);

                    using var fileStream = File.OpenRead(filePath);
                    using var reader = new BinaryReader(fileStream);
                    reader.BaseStream.Position = 0;

                    var assetData = await Task.Run(() => ProcessAsset(platform, assetType, meta, reader));

                    if (!assetData.IsSuccess)
                    {
                        return false;
                    }

                    if (shouldCompressFile)
                    {
                        assetData.Data = await Task.Run(() => AssetCompressor.CompressBytes(assetData.Data, fileOptions.CompressionLevel));
                    }

                    if (shouldEncryptFile)
                    {
                        assetData.Data = await Task.Run(() => AssetEncrypter.EncryptBytes(assetData.Data, AssetUtils.ENCRYPTION_VERY_SECURE_PASSWORD));
                    }

                    // Asset length
                    bufWritter.Write(assetData.Data.Length);

                    var metaBuffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(meta));

                    // meta size
                    bufWritter.Write(metaBuffer.Length);

                    long assetDataLoc = bufWritter.BaseStream.Position;
                    // Asset data
                    bufWritter.Write(assetData.Data);

                    long metaStartLocation = bufWritter.BaseStream.Position;

                    // metadata
                    bufWritter.Write(metaBuffer);

                    long assetEndPos = bufWritter.BaseStream.Position;

                    // Set table position to write asset info
                    bufWritter.BaseStream.Position = currentFileIdPosition;


                    // asset block location
                    bufWritter.Write(startAssetBlockPos);

                    // asset data location
                    bufWritter.Write(assetDataLoc);

                    // asset's meta location
                    bufWritter.Write(metaStartLocation);

                    // apply current asset block write position
                    bufWritter.BaseStream.Position = assetEndPos;

                    // advance file info position in table
                    currentFileIdPosition += fieldIfOffset;

                    count++;

                    Console.WriteLine($"Building Assets ({count * 100 / files.Length}%): {filePath}");
                }

                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                failureException = e;
                return false;
            }
            finally
            {
                if (!success && File.Exists(path))
                {
                    if (failureException != null)
                    {
                        File.WriteAllText(path + "Error.txt", failureException.ToString());
                    }

                    File.Delete(path);
                }
            }

            return true;
            // File.WriteAllText(Path.Combine(outFolder, Paths.ASSET_BUILD_DATA_FILE_META_NAME), JsonConvert.SerializeObject(new GameDataMetaFile() {  CreationDateBinary = creationDate }));
        }

        private void WriteEngineData(BinaryWriter writer)
        {
            var data = EngineServices.GetService<EngineDataService>();
            var projectSettings = data.GetProjectSettings();

            WriteProjectSettings(writer, projectSettings);
        }

        private void WriteProjectSettings(BinaryWriter writer, ProjectSettingsData settings)
        {
            // ----Layers

            // Write layers count
            writer.Write(settings.LayerSettings.Layers.Length);
            foreach (var layer in settings.LayerSettings.Layers)
            {
                // Write layers
                EditorFileUtils.WriteString(writer, layer);
            }

            // ----Scenes
            EditorFileUtils.WriteGuidNoAlloc(writer, Guid.Parse(settings.SceneSettings.MainScene));

            var scenes = settings.SceneSettings.Scenes.Where(x => x.IsBuildAdded && !string.IsNullOrEmpty(x.RefId))
                                                      .Select(x => { Guid.TryParse(x.RefId, out var scenId); return scenId; }).ToArray();
            // Write Scenes count
            writer.Write(scenes.Length);

            foreach (var scenesId in scenes)
            {
                // Write scenes id
                EditorFileUtils.WriteGuidNoAlloc(writer, scenesId);
            }

            // ----Physics

            // Write gravity
            EditorFileUtils.WriteStruct(writer, settings.Physics.Gravity);

            // Write fixed time step
            writer.Write(settings.Physics.FixedTimeStep);

            // Write collision matrix count
            if (settings.Physics.CollisionMatrix != null && settings.Physics.CollisionMatrix.Length > 0)
            {
                var collisionMatrixCount = settings.Physics.CollisionMatrix.Length;
                writer.Write((int)collisionMatrixCount);

                // Write collision matrix
                for (var i = 0; i < collisionMatrixCount; i++)
                {
                    // Write collision values
                    EditorFileUtils.WriteBool(writer, settings.Physics.CollisionMatrix[i]);
                }
            }
            else
            {
                writer.Write((int)0);
            }
        }
    }
}