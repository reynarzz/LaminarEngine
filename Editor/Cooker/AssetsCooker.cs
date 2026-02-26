using Newtonsoft.Json;
using Engine;
using System.Reflection;
using Editor.Cooker.Generator;

namespace Editor.Cooker
{
    internal class AssetsCooker
    {
        private static readonly OrderedDictionary<string, AssetType> _assetsTypes;
        private static Dictionary<CookingType, AssetsCookerBase> _assetCookers;

        private static AssetsDatabaseInfo _databaseInfo;
        internal static AssetsDatabaseInfo DatabaseInfo => _databaseInfo; // Remove this.

        static AssetsCooker()
        {
            // Asset types, order of insertion matters so assets can import correctly. Ex: Tilemaps need textures to be imported already.
            _assetsTypes = new(StringComparer.OrdinalIgnoreCase)
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
                // { ".ldtk", AssetType.Text },
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

                // animation
                { ".anim", AssetType.AnimationClip },
                { ".animctrl", AssetType.AnimationController },

                // scene
                { ".scene", AssetType.Scene },
                { ".ldtk", AssetType.Tilemap },
            };

            InitAssetCookers();
        }

        private static void InitAssetCookers()
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
                { AssetType.AnimationClip, new AnimClipProcessorEditor() },
                { AssetType.AnimationController, new AnimControllerClipProcessorEditor() },
                { AssetType.Material, new MaterialProcessorEditor() },
                { AssetType.Scene, new SceneAssetProcessorEditor() },
                { AssetType.Tilemap, new TilemapAssetProcessor() }
            };

            var releaseModeAssetsProcessors = new Dictionary<AssetType, IAssetProcessor>()
            {
                { AssetType.Texture, new TextureAssetProcessor() },
                { AssetType.Audio, new AudioAssetProcessor() },
                { AssetType.Text, new TextAssetProcessor() },
                { AssetType.Shader, new ShaderAssetProcessorOld() },
                { AssetType.ShaderV2, new ShaderProcessorRelease() },
                { AssetType.Font, new RawBytesAssetProcessor() },
                { AssetType.AnimationClip, new RawBytesAssetProcessor() }, // TODO: binary serialization
                { AssetType.AnimationController, new RawBytesAssetProcessor() }, // TODO: binary serialization
                { AssetType.Material, new MaterialProcessorRelease() },
                { AssetType.Scene, new SceneAssetProcessorRelease() },
                { AssetType.Tilemap, new TilemapAssetProcessor() }
            };

            _assetCookers = new Dictionary<CookingType, AssetsCookerBase>()
            {
                {  CookingType.DevMode, new DevModeFilesCooker(_databaseInfo, devModeAssetsProcessors) },
                {  CookingType.ReleaseMode, new ReleaseModeFilesCooker(releaseModeAssetsProcessors) },
            };
        }

        // TODO: implement multi pass import.
        public static async Task<DishResult> CookAllAsync(CookOptions options)
        {
            // Search project's files first
            var files = Directory.GetFiles(options.AssetsFolderPath, "*", SearchOption.AllDirectories).ToList();

            var cookersFiles = Directory.GetFiles(Path.Combine(EditorPaths.CookerPaths.CookerRoot, "Assets"), "*", SearchOption.AllDirectories);

            files.AddRange(cookersFiles);

            var extensionOrder = _assetsTypes.Keys.Select((ext, index) => new { ext, index })
                                                  .ToDictionary(x => x.ext, x => x.index, StringComparer.OrdinalIgnoreCase);

            var selectedFiles = files.Where(path => extensionOrder.ContainsKey(Path.GetExtension(path)))
                                     .Select(path => (path: Paths.ClearPathSeparation(path), assetType: _assetsTypes[Path.GetExtension(path)],
                                                                                             order: extensionOrder[Path.GetExtension(path)]))
                                     .OrderBy(x => x.order)
                                     .Select(x => (x.path, x.assetType));

            if (options.Type == CookingType.ReleaseMode && options.MatchingFiles != null && options.MatchingFiles.Length > 0)
            {
                // This clears all the types that will be collected by the typeRegistry.
                TypeRegistryClassGenerator.ClearTypesLibrary();

                Console.WriteLine("Warning: Building only selected files, make sure these are updated!");
                selectedFiles = selectedFiles.Where(x =>
                {
                    for (int i = 0; i < options.MatchingFiles.Length; i++)
                    {
                        if (string.IsNullOrEmpty(options.MatchingFiles[i]))
                            continue;

                        if (x.path.EndsWith(options.MatchingFiles[i]) || x.path.Contains(EditorPaths.CookerPaths.InternalAssetsPath))
                        {
                            return true;
                        }

                    }
                    return false;
                });
            }
            var collectedFiles = selectedFiles.ToArray();
            var result = await _assetCookers[options.Type].CookAssetsAsync(options.FileOptions, options.Platform,
                                             collectedFiles, options.ExportFolderPath);

            if (result && options.Type == CookingType.ReleaseMode)
            {
                // This generates the whole type registry after all the types where collected from the assets.
                TypeGenerationStage.GenerateTypeRegistry();
            }
            return new DishResult()
            {
                IsSuccess = result,
                DataInfo = _databaseInfo
            };
        }
    }

    public class DishResult
    {
        public bool IsSuccess { get; set; }
        public AssetsDatabaseInfo DataInfo { get; set; }
    }
}