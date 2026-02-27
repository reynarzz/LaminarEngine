using Editor.Build;
using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Layers;
using Engine.Serialization;
using Engine.Utils;
using System.Reflection;
using System.Runtime.Loader;
using Editor.Drawers;

namespace Editor.Layers
{
    internal class HotReloadLayer : LayerBase
    {
        private PluginLoadContext _assemblyLoadContext;
        private Assembly _gameAppAssembly = null;
        private bool _canSwapDll = false;
        private bool _isSwappingDll = false;

        public HotReloadLayer()
        {
            BuildSystem.OnBuildCompleted += OnBuildCompleted;

        }

        public override Task InitializeAsync()
        {
            return BuildSystem.BuildAsync(PlatformBuild.GameAppDomain);
        }


        private void OnBuildCompleted(BuildResult result)
        {
            if (result.IsSuccess && result.Platform == PlatformBuild.GameAppDomain)
            {
                if (!result.AnyStageSkippedBuild || _gameAppAssembly == null)
                {
                    _canSwapDll = true;
                }
            }
        }

        internal void SwapDll(bool serializeCurrentScene = true)
        {
            Debug.Log("Rebuild detected");

            BeforeReload(serializeCurrentScene);

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
            GfsTypeRegistryEditor.GameAssembly = _gameAppAssembly;
            ReflectionUtils.SetTypeRegistry(GfsTypeRegistryEditor.Resolve);

            foreach (var type in _gameAppAssembly.DefinedTypes)
            {
                GfsTypeRegistryEditor.RegisterRecursive(type);
            }

            if (GfsTypeRegistryEditor.GameAppType == null)
            {
                Debug.Error("No app layer is defined in the Game.dll");
            }

            UpdateCustomEditor();

            SceneManagerEditor.DeserializeScenesHotReload();
        }

        private void UpdateCustomEditor()
        {
            CustomEditorDatabase.InitCustomComponentDrawers(GetDrawers(typeof(ComponentDrawer<>), typeof(ComponentDrawer)));
            CustomEditorDatabase.InitCustomPropertiesDrawers(GetDrawers(typeof(PropertyDrawer<,>), typeof(PropertyDrawer)));

            List<Type> GetDrawers(Type baseGenericDrawerType, Type baseDrawerType)
            {
                var customEditorTypes = new List<Type>();
                AddCustomEditorTypes(GfsTypeRegistryEditor.GameAssembly, customEditorTypes, baseGenericDrawerType, baseDrawerType);
                AddCustomEditorTypes(GfsTypeRegistryEditor.EngineAssembly, customEditorTypes, baseGenericDrawerType, baseDrawerType);
                AddCustomEditorTypes(GfsTypeRegistryEditor.EditorAssembly, customEditorTypes, baseGenericDrawerType, baseDrawerType);
                return customEditorTypes;
            }

            void AddCustomEditorTypes(Assembly assembly, List<Type> types, Type baseGenericDrawerType, Type baseDrawerType)
            {
                foreach (var typeInfo in assembly.DefinedTypes)
                {
                    var type = typeInfo.AsType();

                    if (InheritsFromGenericDrawer(type) && type != baseDrawerType &&
                        type != baseGenericDrawerType)
                    {
                        Debug.Log($"Custom drawer type found: {type.Name}");
                        types.Add(type);
                    }
                }

                bool InheritsFromGenericDrawer(Type type)
                {
                    for (var current = type; current != null && current != typeof(object); current = current.BaseType)
                    {
                        if (!current.IsGenericType)
                            continue;

                        if (current.GetGenericTypeDefinition() == baseGenericDrawerType)
                            return true;
                    }

                    return false;
                }
            }
        }


        // Swap happens at a certain point to avoid UI's sudden jumps.
        internal override void UpdateLayer()
        {
            if (_canSwapDll && !_isSwappingDll && !Application.IsInPlayMode)
            {
                _canSwapDll = false;
                _isSwappingDll = true;
                SwapDll();
                _isSwappingDll = false;
            }
        }

        private void BeforeReload(bool serializeCurrentScene)
        {
            if (_assemblyLoadContext != null)
            {
                if (serializeCurrentScene)
                {
                    SceneManagerEditor.SerializeScenesHotReload();
                }

                GfsTypeRegistryEditor.Clear();

                _assemblyLoadContext.Unload();
                _assemblyLoadContext = null;
                _gameAppAssembly = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public override void OnEvent(EventType currentEvent, object value)
        {
            if (currentEvent == EventType.WindowFocusEnter)
            {
                BuildSystem.BuildAsync(PlatformBuild.GameAppDomain);
            }
        }

        public override void Close()
        {
        }
    }
}
