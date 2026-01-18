using Editor.AssemblyHotReload;
using Engine;
using Engine.Layers;
using Engine.Utils;
using SharedTypes;
using System.Reflection;
using System.Runtime.Loader;

namespace Editor.Layers
{
    internal class HotReloadLayer : LayerBase
    {
        private readonly GameAssemblyBuilder _gameAssemblyBuilder;
        private AssemblyLoadContext _assemblyLoadContext;
        private Assembly _gameAppAssembly = null;

        public HotReloadLayer()
        {
            _gameAssemblyBuilder = new GameAssemblyBuilder();
            _gameAssemblyBuilder.OnBuildCompleted += OnBuildCompleted;
        }

        public override void Initialize()
        {
            _gameAssemblyBuilder.Build();
        }

        private void OnBuildCompleted(bool success, bool isRebuild)
        {
            if (success)
            {
                if (isRebuild || _gameAppAssembly == null)
                {
                    SwapDll();
                }
                ImportAssets();
            }
        }

        private void SwapDll()
        {
            Debug.Log("Rebuild detected");

            Unload();

            // Copy new compiled dll.
            File.Copy(EditorPaths.NewGameDllAbsolutePath, EditorPaths.GameHookDLLAbsolutePath, true);
            //  File.Copy(EditorPaths.NewGameDllAbsolutePath, EditorPaths.GameHookDLLAbsolutePath, true);
            var pdbPath = Paths.ClearPathSeparation(Path.Combine(EditorPaths.GameBinFolderAbsolutePath,
                                                                 EditorPaths.GAME_PROJECT_NAME + ".pdb"));

            var asmBytes = File.ReadAllBytes(EditorPaths.GameHookDLLAbsolutePath);
            var pdbBytes = File.Exists(pdbPath) ? File.ReadAllBytes(pdbPath) : null;

            _assemblyLoadContext = new AssemblyLoadContext(EditorPaths.GAME_PROJECT_NAME, isCollectible: true);

            using var asmStream = new MemoryStream(asmBytes);
            using var pdbStream = pdbBytes != null ? new MemoryStream(pdbBytes) : null;

            if (pdbStream != null)
            {
                _gameAppAssembly = _assemblyLoadContext.LoadFromStream(asmStream, pdbStream);
            }
            else
            {
                _gameAppAssembly = _assemblyLoadContext.LoadFromStream(asmStream);
            }

            _assemblyLoadContext.Resolving += (context, name) =>
            {
                var depPath = Path.Combine(EditorPaths.GameBinFolderAbsolutePath, name.Name + ".dll");
                if (!File.Exists(depPath))
                {
                    return null;
                }

                return context.LoadFromAssemblyPath(depPath);
            };

            ReflectionUtils.PushAssembly(_gameAppAssembly);

            foreach (var type in _gameAppAssembly.DefinedTypes)
            {
                if (type.IsAssignableTo(typeof(ApplicationLayer)))
                {
                    Debug.Success(type.Name);
                }
                Debug.Log(type.Name);
            }


        }

        private void Unload()
        {
            if (_assemblyLoadContext != null)
            {
                ReflectionUtils.PopAssembly(_gameAppAssembly);

                _assemblyLoadContext.Unload();
                _assemblyLoadContext = null;
                _gameAppAssembly = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public override void OnEvent(EventType currentEvent, object value)
        {
            if (currentEvent == EventType.WindowFocusEnter)
            {
                _gameAssemblyBuilder.Build();
            }
        }

        private void ImportAssets()
        {
            var releaseAssetsList = default(string[]);
            if (File.Exists(Paths.GetShipAssetsFilePath()))
            {
                releaseAssetsList = File.ReadAllText(Paths.GetShipAssetsFilePath())?.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            }
            var assetDatabase = new GameCooker.AssetsCooker().CookAll(new GameCooker.CookOptions()
            {
                Type = GameCooker.CookingType.DevMode,
                Platform = GameCooker.CookingPlatform.Windows,
                AssetsFolderPath = Paths.GetAssetsFolderPath(),
                ExportFolderPath = Paths.GetAssetDatabaseFolder(),
                FileOptions = new GameCooker.CookFileOptions()
                {
                    CompressAllFiles = false,
                    CompressionLevel = 12,
                    EncryptAllFiles = false,
                },
                // TODO: The editor will walk through all the scenes recursively and detect which assets are used,
                //       so no manual list  will be needed.
                MatchingFiles = releaseAssetsList
            });

            foreach (var guid in assetDatabase.UpdatedAssets)
            {
                IOLayer.Database?.UpdateReloadAsset(guid);
            }
        }

        public override void Close()
        {
        }


    }
}
