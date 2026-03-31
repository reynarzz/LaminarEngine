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
using Editor.Data;

namespace Editor.Layers
{
    internal class HotReloadLayer : LayerBase
    {
        private PluginLoadContext _assemblyLoadContext;
        private Assembly _gameAppAssembly = null;
        private bool _canSwapDll = false;
        private bool _isSwappingDll = false;

        public override async Task<LayerInitResult> InitializeAsync()
        {
            await BuildSystem.BuildAsync(PlatformBuild.GameAppDomain);
            _canSwapDll = true;
            UpdateLayer();
            BuildSystem.OnBuildCompleted += OnBuildCompleted;

            return LayerInitResult.Success;
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

        private void SwapDll(bool serializeCurrentScene = true)
        {
            Debug.Log("Rebuild detected");

            BeforeReload(serializeCurrentScene);

            // Copy new compiled dll.
            File.Copy(EditorPaths.CompiledGameDllAbsolutePath, EditorPaths.GameHookDLLAbsolutePath, true);

            var pdbPath = Paths.ClearPathSeparation(Path.Combine(EditorPaths.GameBinFolderAbsolutePath,
                                                                 EditorPaths.GameCsProjName + ".pdb"));
            var pdbTargetPath = Paths.ClearPathSeparation(Path.Combine(EditorPaths.HookFolderAbsolutePath,
                                                          EditorPaths.GameCsProjName + ".pdb"));

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
            LaminarTypeRegistryEditor.GameAssembly = _gameAppAssembly;
            ReflectionUtils.SetTypeRegistry(LaminarTypeRegistryEditor.Resolve);

            foreach (var type in _gameAppAssembly.DefinedTypes)
            {
                LaminarTypeRegistryEditor.RegisterRecursive(type);
            }

            if (LaminarTypeRegistryEditor.GameAppType == null)
            {
                LaminarTypeRegistryEditor.GameAppType = typeof(ApplicationLayer);
                Debug.Warn($"No application layer was defined in the {EditorPaths.GAME_PROJECT_NAME}.dll, the default one will be used instead.");
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
                AddCustomEditorTypes(LaminarTypeRegistryEditor.GameAssembly, customEditorTypes, baseGenericDrawerType, baseDrawerType);
                AddCustomEditorTypes(LaminarTypeRegistryEditor.EngineAssembly, customEditorTypes, baseGenericDrawerType, baseDrawerType);
                AddCustomEditorTypes(LaminarTypeRegistryEditor.EditorAssembly, customEditorTypes, baseGenericDrawerType, baseDrawerType);
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
            if (!EditorConfigManager.IsProjectLoaded())
                return;

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

                LaminarTypeRegistryEditor.Clear();

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
            if (!EditorConfigManager.IsProjectLoaded())
                return;

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
