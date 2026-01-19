using Editor.AssemblyHotReload;
using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Layers;
using Engine.Serialization;
using Engine.Utils;
using SharedTypes;
using System.Reflection;
using System.Runtime.Loader;

namespace Editor.Layers
{
    internal class HotReloadLayer : LayerBase
    {
        private readonly GameAssemblyBuilder _gameAssemblyBuilder;
        private PluginLoadContext _assemblyLoadContext;
        private Assembly _gameAppAssembly = null;
        private readonly List<string> _sceneList = new();
        private readonly List<List<Actor>> _actorsSerialized = new();
        private bool _canSwapDll = false;
        private bool _isSwappingDll = false;

        public HotReloadLayer()
        {
            _gameAssemblyBuilder = new GameAssemblyBuilder();
            _gameAssemblyBuilder.OnBuildCompleted += OnBuildCompleted;
        }

        public override async void Initialize()
        {
            await _gameAssemblyBuilder.BuildAsync();
        }

        private void OnBuildCompleted(bool success, bool isRebuild)
        {
            if (success)
            {
                if (isRebuild || _gameAppAssembly == null)
                {
                    _canSwapDll = true;
                }
                ImportAssets();
            }
        }
        private class PluginLoadContext : AssemblyLoadContext
        {
            private readonly AssemblyDependencyResolver _resolver;

            public PluginLoadContext(string pluginPath) : base(pluginPath, true)
            {
                _resolver = new AssemblyDependencyResolver(pluginPath);
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                var path = _resolver.ResolveAssemblyToPath(assemblyName);
                return path != null ? LoadFromAssemblyPath(path) : null;
            }
        }

        private void SwapDll()
        {
            Debug.Log("Rebuild detected");

            BeforeReload();

            // Copy new compiled dll.
            File.Copy(EditorPaths.CompiledGameDllAbsolutePath, EditorPaths.GameHookDLLAbsolutePath, true);

            var pdbPath = Paths.ClearPathSeparation(Path.Combine(EditorPaths.GameBinFolderAbsolutePath,
                                                                 EditorPaths.GAME_PROJECT_NAME + ".pdb"));
            var pdbTargetPath = Paths.ClearPathSeparation(Path.Combine(EditorPaths.HookFolderAbsolutePath,
                                                          EditorPaths.GAME_PROJECT_NAME + ".pdb"));

            byte[] pdbBytes = null;
            if (File.Exists(pdbPath))
            {
                File.Copy(pdbPath, pdbTargetPath, true);
                pdbBytes = File.ReadAllBytes(pdbTargetPath);
            }

            _assemblyLoadContext = new(EditorPaths.GameHookDLLAbsolutePath);

            var buildPath = EditorPaths.CompiledGameDllAbsolutePath;
            var resolver = new AssemblyDependencyResolver(buildPath);

            _assemblyLoadContext.Resolving += (context, name) =>
            {
                // Prevents resolving the main assembly here.
                if (name.Name == Path.GetFileNameWithoutExtension(buildPath))
                    return null;

                var path = resolver.ResolveAssemblyToPath(name);
                if (path != null)
                {
                    return context.LoadFromAssemblyPath(path);
                }

                return null;
            };

            var asmBytes = File.ReadAllBytes(EditorPaths.GameHookDLLAbsolutePath);
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
            ReflectionUtils.SetGameAssembly(_gameAppAssembly, GfsTypeRegistry.Resolve);

            foreach (var type in _gameAppAssembly.DefinedTypes)
            {
                GfsTypeRegistry.RegisterRecursive(type);
            }

            if (GfsTypeRegistry.GameAppType == null)
            {
                Debug.Error("No app layer is defined in the Game.dll");
            }
            DeserializeScenes();
        }

        // Swap happens at a certain point to avoid UI's sudden jumps.
        internal override void UpdateLayer()
        {
            if (_canSwapDll && !_isSwappingDll)
            {
                _canSwapDll = false;
                _isSwappingDll = true;
                SwapDll();
                _isSwappingDll = false;
            }
        }

        private void BeforeReload()
        {
            if (_assemblyLoadContext != null)
            {
                SerializeScene();

                ReflectionUtils.RemoveGame(_gameAppAssembly);
                GfsTypeRegistry.Clear();

                _assemblyLoadContext.Unload();
                _assemblyLoadContext = null;
                _gameAppAssembly = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void SerializeScene()
        {
            _sceneList.Clear();
            _actorsSerialized.Clear();
            foreach (var scene in SceneManager.Scenes)
            {
                var sceneObj = SceneSerializer.SerializeScene(scene,
                    new SceneSerializer.SerializationOptions()
                    {
                        CollectedPhysicalActors = true,
                        RemoveGameDLLComponentsFromActors = true,
                    });
                var serializedScene = EditorJsonUtils.Serialize(sceneObj);
                _actorsSerialized.Add(sceneObj.Actors);

                _sceneList.Add(serializedScene);
            }
        }

        private void DeserializeScenes()
        {
            if (_sceneList.Count > 0)
            {
                for (int i = 0; i < _sceneList.Count; i++)
                {
                    var sceneSerialized = EditorJsonUtils.Deserialize<SerializedScene>(_sceneList[i]);
                    SceneDeserializer.DeserializeSceneComponents(_actorsSerialized[i], sceneSerialized.ActorsData);
                }
            }

            _sceneList.Clear();
            _actorsSerialized.Clear();
        }

        public override async void OnEvent(EventType currentEvent, object value)
        {
            if (currentEvent == EventType.WindowFocusEnter)
            {
                await _gameAssemblyBuilder.BuildAsync();
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
