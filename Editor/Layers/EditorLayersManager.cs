using Editor.Build;
using Editor.Layers;
using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Layers;
using Engine.Serialization;
using Engine.Utils;
using Editor.Cooker;
using Editor.Data;

namespace Editor
{
    internal class EditorLayersManager : LayersManager
    {
        private PlaymodeController _playmodeController; // Remove from here
        private readonly LayerBase[] _initializationLayers;
        public EditorLayersManager(InputLayerBase inputLayer, WindowStandalone win)
        {
            var timeLayer = new TimeLayer();
            var mainThreadDispLayer = new MainThreadDispatcher();
            var audioLayer = new AudioLayer();
            var imguiLayer = new ImGuiLayer(win, inputLayer, this);
            var renderingLayer = new RenderingLayer();
            var editorLayer = new EditorIOLayer();
            var hotReloadLayer = new HotReloadLayer();
            var sceneLayer = new SceneLayer();

            Layers =
            [
                timeLayer,
                inputLayer,
                null, // App Layer
                mainThreadDispLayer,
                sceneLayer, 
                audioLayer,
                null, // Physics layer.
                editorLayer,
                hotReloadLayer,
                imguiLayer,
                mainThreadDispLayer,
                renderingLayer,
            ];

            _initializationLayers =
            [
                renderingLayer
            ];

            _playmodeController = new PlaymodeController(this, timeLayer, hotReloadLayer);
        }

        internal override void InitializeAsync()
        {
            if (LayersInitialized)
            {
                Debug.EngineError("Layers are already initialized");
                return;
            }

            async Task InitializeLayers()
            {
                for (int i = Layers.Length - 1; i >= 0; i--)
                {
                    var layer = Layers[i];

                    if (layer != null)
                    {
                        await layer.InitializeAsync();
                        Debug.Log("Initialized layer: " + layer.GetType().Name);
                    }
                }

                Debug.Log("Layers fully initialized");
                LayersInitialized = true;

                await MainThreadDispatcher.EnqueueAsync(OnLayersInitialized);
            }

            Task.Run(InitializeLayers);
        }

        private void OnLayersInitialized()
        {
            LoadScene();
        }

        private void LoadScene()
        {
            if (Guid.TryParse(EditorDataManager.EditorSettings.OpenedSceneRefId, out var id))
            {
                SceneManager.LoadScene(id);
            }
            else
            {
                // TODO: load default scene from another place.
                SceneManager.LoadEmptyScene("Default Scene");
                var camera = new Actor("Main Camera").AddComponent<Camera>();
                camera.Transform.LocalPosition = new GlmNet.vec3(0, 0, -10);
                camera.BackgroundColor = new Color32(49, 121, 79, 255);
            }
        }

        internal override void Update()
        {
            base.Update();

            if (!LayersInitialized)
            {
                // Updates layers needed to present initialization info to the user.
                foreach (var layer in _initializationLayers)
                {
                    if (layer.IsInitialized)
                    {
                        layer.UpdateLayer();
                    }
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Physics2D.DrawColliders = !Physics2D.DrawColliders;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.DrawUILines = !Debug.DrawUILines;
            }
        }

        // TODO: Move this
        private void GenerateIconAndroidSizes(Texture defaultTexture, Texture foreground, Texture background)
        {
            var resolutions = Enum.GetValues<AndroidIconSizes>();
            var dir = Path.Combine(EditorPaths.AppRoot, "Platforms/Android/Resources");

            var channels = 4;

            void AddIcons(string name, Texture texture)
            {
                if (!texture)
                {
                    Debug.Error("Icon is null, can't convert: " + name);
                    return;
                }

                for (int i = 0; i < resolutions.Length; i++)
                {
                    var resEnum = resolutions[i];

                    int dim = (int)resEnum;
                    var outBytes = new byte[dim * dim * channels];

                    EditorImage.Resize(texture.Data, channels, texture.Width, texture.Height, outBytes, dim, dim);

                    var format = EditorImageWriteFormat.Png;
                    var iconPath = Path.Combine(dir, $"mipmap-{resEnum}", $"{name}.{format.ToString().ToLower()}");
                    EditorImage.Write(iconPath, dim, dim, channels, outBytes, format);
                }
            }

            AddIcons("appicon", defaultTexture);
            AddIcons("appicon_foreground", foreground);
            AddIcons("appicon_background", background);

        }

    }
}