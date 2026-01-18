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
        private AssemblyLoadContext _assemblyLoadContext;
        private Assembly _gameAppAssembly = null;
        private readonly List<string> _sceneList = new();
        private readonly List<List<Actor>> _actorsSerialized = new();
        private readonly List<Type> _componentsTypes = new();

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

            BeforeReload();

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

            ReflectionUtils.SetGameAssembly(_gameAppAssembly, GfsTypeRegistry.Resolve);

            foreach (var type in _gameAppAssembly.DefinedTypes)
            {
                EditorReflection.RegisterTypeRecursive(type);

                if (type.IsAssignableTo(typeof(ApplicationLayer)))
                {
                    Debug.Success(type.AssemblyQualifiedName);
                }
            }

            DeserializeScenes();
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
                        SerializeOnlyGameDLLComponents = true
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
