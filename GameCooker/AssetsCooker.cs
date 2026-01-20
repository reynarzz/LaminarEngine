using Newtonsoft.Json;
using SharedTypes;

namespace GameCooker
{
    public class AssetsCooker
    {
        private Dictionary<string, AssetType> _assetsTypes;
        private Dictionary<CookingType, AssetsCookerBase> _assetCookers;

        private AssetsDatabaseInfo _databaseInfo;

        public AssetsCooker()
        {
            _assetsTypes = new Dictionary<string, AssetType>(StringComparer.OrdinalIgnoreCase)
            {
                // Image
                { ".png", AssetType.Texture },
                { ".tga", AssetType.Texture },
                { ".jpg", AssetType.Texture },
                { ".psd", AssetType.Texture },
                { ".hdr", AssetType.Texture },
                { ".pic", AssetType.Texture },
                { ".bmp", AssetType.Texture },

                // Audio
                { ".mp3", AssetType.Audio },
                { ".wav", AssetType.Audio },
                { ".ogg", AssetType.Audio },
                { ".flac", AssetType.Audio },
                

                // Text
                { ".txt", AssetType.Text },
                { ".ldtk", AssetType.Text },
                { ".json", AssetType.Text },
                { ".xml", AssetType.Text },
                { ".csv", AssetType.Text },
                
                // Shader
                { ".vert", AssetType.Shader },
                { ".frag", AssetType.Shader },

                { ".shader", AssetType.ShaderV2 },
                { ".material", AssetType.Material },

                // Font
                { ".ttf", AssetType.Font },
                { ".otf", AssetType.Font },

                { ".anim", AssetType.AnimationClip },
                { ".animctrl", AssetType.AnimationController },

            };

            InitAssetCookers();
        }


        private void InitAssetCookers()
        {
            if (File.Exists(Paths.GetAssetDatabaseFilePath()))
            {
                _databaseInfo = JsonConvert.DeserializeObject<AssetsDatabaseInfo>(File.ReadAllText(Paths.GetAssetDatabaseFilePath()));
            }

            if (_databaseInfo == null)
            {
                _databaseInfo = new AssetsDatabaseInfo()
                {
                    CreationDate = DateTime.Now
                };
            }

            var devModeAssetsProcessors = new Dictionary<AssetType, IAssetProcessor>()
            {
                { AssetType.Texture, new TextureAssetProcessor() },
                { AssetType.Audio, new AudioAssetProcessor() },
                { AssetType.Text, new TextAssetProcessor() },
                { AssetType.Shader, new ShaderAssetProcessorOld() },
                { AssetType.ShaderV2, new ShaderAssetProcessor() },
                { AssetType.Font, new RawBytesAssetProcessor() },
                { AssetType.AnimationClip, new AnimClipAssetProcessorDevMode() },
                { AssetType.AnimationController, new AnimControllerClipAssetProcessorDevMode() },
                { AssetType.Material, new MaterialAssetProcessorDevMode() },
            };

            var releaseModeAssetsProcessors = new Dictionary<AssetType, IAssetProcessor>()
            {
                { AssetType.Texture, new TextureAssetProcessor() },
                { AssetType.Audio, new AudioAssetProcessor() },
                { AssetType.Text, new TextAssetProcessor() },
                { AssetType.Shader, new ShaderAssetProcessorOld() },
                { AssetType.ShaderV2, new ShaderAssetProcessor() },
                { AssetType.Font, new RawBytesAssetProcessor() },
                { AssetType.AnimationClip, new RawBytesAssetProcessor() }, // TODO: binary serialization
                { AssetType.AnimationController, new RawBytesAssetProcessor() }, // TODO: binary serialization
                { AssetType.Material, new RawBytesAssetProcessor() }, // TODO: binary serialization
            };

            _assetCookers = new Dictionary<CookingType, AssetsCookerBase>()
            {
                {  CookingType.DevMode, new DevModeFilesCooker(_databaseInfo, devModeAssetsProcessors) },
                {  CookingType.ReleaseMode, new ReleaseModeFilesCooker(releaseModeAssetsProcessors) },
            };
        }

        public async Task<AssetsDatabaseInfo> CookAllAsync(CookOptions options)
        {
            var files = Directory.GetFiles(options.AssetsFolderPath, "*", SearchOption.AllDirectories);

            var selectedFiles = files.Where(path => _assetsTypes.TryGetValue(Path.GetExtension(path), out _))
                                     .Select(path => (path: Paths.ClearPathSeparation(path), assetType: _assetsTypes[Path.GetExtension(path)]));

            if (options.Type == CookingType.ReleaseMode && options.MatchingFiles != null && options.MatchingFiles.Length > 0)
            {
                Console.WriteLine("Warning: Building only selected files, make sure these are updated!");
                selectedFiles = selectedFiles.Where(x =>
                {
                    for (int i = 0; i < options.MatchingFiles.Length; i++)
                    {
                        if (string.IsNullOrEmpty(options.MatchingFiles[i]))
                            continue;

                        if (x.path.EndsWith(options.MatchingFiles[i]))
                        {
                            return true;
                        }

                    }
                    return false;
                });
            }

            await _assetCookers[options.Type].CookAssetsAsync(options.FileOptions, options.Platform,
                                                              selectedFiles.ToArray(), options.ExportFolderPath);

            return _databaseInfo;
        }

        public AssetsDatabaseInfo CookAll(CookOptions options)
        {
            return CookAllAsync(options).GetAwaiter().GetResult();
        }
    }
}