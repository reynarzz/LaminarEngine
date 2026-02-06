using Newtonsoft.Json;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class ReleaseModeFilesCooker : AssetsCookerBase
    {
        /* .gfs file format
         
            - Magic (char[4])
            
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
                - asset guid size (int32)
                - asset guid (byte[])
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
        public ReleaseModeFilesCooker(Dictionary<AssetType, IAssetProcessor> processor) : base(processor)
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

                    var meta = AssetUtils.GetMeta(filePath + Paths.ASSET_META_EXT_NAME, assetType);
                    var relAssetPath = Paths.GetRelativeAssetPath(filePath);

                    var guidBinary = meta.GUID.ToByteArray();
                    // asset guid size
                    bufWritter.Write(guidBinary.Length);
                    //if (guidBinary.Length != 16)
                    //{
                    //    throw new Exception("Bigger than 16");
                    //}
                    // asset guid
                    bufWritter.Write(guidBinary);

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

                    var assetData = await Task.Run(() => ProcessAsset(platform, assetType, meta, filePath));

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

                    // ---- Set table position to write asset info
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
    }
}