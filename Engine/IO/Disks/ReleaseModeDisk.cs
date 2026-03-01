using Engine;
using Engine.Data;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal class ReleaseModeDisk : DiskBase
    {
        private readonly BinaryReader _reader;
        private struct AssetLocInfo
        {
            public long AssetDataLoc { get; set; }
            public int AssetDataSize { get; set; }
            public long AssetMetaLoc { get; set; }
            public int AssetMetaSize { get; set; }
        }

        private readonly Dictionary<Guid, AssetLocInfo> _assetsLocations = new();
        public ReleaseModeDisk(string folderPath)
        {
            var executablePath = "";
            executablePath = Path.Combine(folderPath, Paths.GetAssetBuildDataFilename());

            //Try to check if the file exists alonside the executable.
            if (!File.Exists(executablePath))
            {
                var path = Path.Combine(AppContext.BaseDirectory, Paths.GetAssetBuildDataFilename());

                if (File.Exists(path))
                {
                    executablePath = path;
                }
                else
                {
                    Debug.Error($"Can't find game assets '.gfs', maybe the build failed?");
                }
            }
            _reader = new BinaryReader(new FileStream(executablePath, FileMode.Open, FileAccess.Read));
        }

        public ReleaseModeDisk(BinaryReader reader)
        {
            _reader = reader;
        }

        public override bool Initialize()
        {
            var header = _reader.ReadBytes(AssetUtils.GFSFileFormat.HEADER.Length);

            var headerStr = Encoding.UTF8.GetString(header);

            if (!headerStr.Equals(AssetUtils.GFSFileFormat.HEADER))
            {
                throw new Exception("Corrupted file data");
            }

            // File version
            var version = _reader.ReadUInt32();

            // Creation date
            var creationData = DateTime.FromBinary(_reader.ReadInt64());

            // Read engine data
            ReadEngineData(_reader);

            var totalAssets = _reader.ReadInt32();
            AssetDatabaseInfo.Assets.EnsureCapacity(totalAssets);
            _assetsLocations.EnsureCapacity(totalAssets);

            var creationTimeBuffer = _reader.ReadInt64();
            AssetDatabaseInfo.CreationDate = DateTime.FromBinary(creationTimeBuffer);
            AssetDatabaseInfo.TotalAssets = totalAssets;

            for (int i = 0; i < totalAssets; i++)
            {
                long assetBlockLoc = _reader.ReadInt64();
                long assetDataLoc = _reader.ReadInt64();
                long metaBlockLoc = _reader.ReadInt64();
                long currentPos = _reader.BaseStream.Position;

                _reader.BaseStream.Position = assetBlockLoc;

                Guid guid = EngineFileUtils.ReadGuidNoAlloc(_reader);

                int pathSize = _reader.ReadInt32();

                var pathBytes = _reader.ReadBytes(pathSize);

                var assetType = (AssetType)_reader.ReadInt32();
                bool isCompressed = _reader.ReadBoolean();
                bool isEncrypted = _reader.ReadBoolean();
                bool isPathEncrypted = _reader.ReadBoolean();
                int assetDataSize = _reader.ReadInt32();
                int metaDataSize = _reader.ReadInt32();

                if (isPathEncrypted)
                {
                    pathBytes = AssetEncrypter.DecryptBytes(pathBytes, AssetUtils.ENCRYPTION_VERY_SECURE_PASSWORD);
                }
                AssetDatabaseInfo.Assets.Add(guid, new AssetInfo()
                {
                    Type = assetType,
                    IsCompressed = isCompressed,
                    IsEncrypted = isEncrypted,
                    Path = Encoding.UTF8.GetString(pathBytes)
                });

                _assetsLocations.Add(guid, new AssetLocInfo()
                {
                    AssetDataLoc = assetDataLoc,
                    AssetDataSize = assetDataSize,
                    AssetMetaLoc = metaBlockLoc,
                    AssetMetaSize = metaDataSize,
                });

                _reader.BaseStream.Position = currentPos;
            }

            return true;
        }

        private void ReadEngineData(BinaryReader reader)
        {
            var data = EngineServices.GetService<EngineDataService>();
            data.Initialize(ReadProjectSettings(reader));
        }

        private ProjectSettings ReadProjectSettings(BinaryReader reader)
        {
            var settings = new ProjectSettings();

            // ----Layers:

            // Read layers count
            settings.LayerSettings.Layers = new string[reader.ReadInt32()];

            for (int i = 0; i < settings.LayerSettings.Layers.Length; i++)
            {
                // Read layer
                settings.LayerSettings.Layers[i] = EngineFileUtils.ReadString(reader);
            }

            // ----Scenes:
            // Read launch scene
            settings.SceneSettings.MainScene = EngineFileUtils.ReadGuidNoAlloc(reader).ToString();

            // Read scenes count
            var scenesCount = reader.ReadInt32();
            settings.SceneSettings.Scenes = new List<string>();
            CollectionsMarshal.SetCount(settings.SceneSettings.Scenes, scenesCount);
            for (int i = 0; i < scenesCount; i++)
            {
                settings.SceneSettings.Scenes[i] = EngineFileUtils.ReadGuidNoAlloc(reader).ToString();
            }

            // ----Physics:
            // Read gravity
            settings.Physics.Gravity = EngineFileUtils.ReadStructNoAlloc<GlmNet.vec3>(reader);

            // Read fixed time step
            settings.Physics.FixedTimeStep = reader.ReadSingle();

            // Read collision matrix count
            var count = reader.ReadInt32();
            settings.Physics.CollisionMatrix = new bool[count];

            // Read collision matrix
            for (int i = 0; i < count; i++)
            {
                settings.Physics.CollisionMatrix[i] = EngineFileUtils.ReadBool(reader);
            }

            return settings;
        }

        protected override byte[] LoadAssetFromDisk(Guid guid)
        {
            if (_assetsLocations.TryGetValue(guid, out var locations))
            {
                _reader.BaseStream.Position = locations.AssetDataLoc;
                return _reader.ReadBytes(locations.AssetDataSize);
            }

            return null;
        }

        protected override async Task<byte[]> LoadAssetFromDiskAsync(Guid guid)
        {
            if (_assetsLocations.TryGetValue(guid, out var locations))
            {
                _reader.BaseStream.Position = locations.AssetDataLoc;
                var data = new byte[locations.AssetDataSize];
                var bytesRead = 0;
                while (bytesRead < data.Length)
                {

#if IOS
                    int read = _reader.BaseStream.Read(data, bytesRead, data.Length - bytesRead);
#else
                    int read = await _reader.BaseStream.ReadAsync(data, bytesRead, data.Length - bytesRead);
#endif
                    if (read == 0)
                    {
                        throw new EndOfStreamException("Unexpected end of stream while reading asset data.");
                    }
                    bytesRead += read;
                }
                return data;
            }

            return null;
        }

        protected override byte[] LoadMetaFromDisk(Guid guid)
        {
            if (_assetsLocations.TryGetValue(guid, out var locations))
            {
                _reader.BaseStream.Position = locations.AssetMetaLoc;
                var meta = new byte[locations.AssetMetaSize];

                _reader.BaseStream.Read(meta, 0, meta.Length);

                return meta;
            }

            return null;
        }
    }
}
