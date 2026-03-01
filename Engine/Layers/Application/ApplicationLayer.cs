using Engine.Data;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    public abstract class ApplicationLayer : LayerBase
    {
        public sealed override Task InitializeAsync()
        {
#if SHIP_BUILD
            var result = LoadLaunchScene();
            if (!result)
            {
                return Task.FromCanceled(new CancellationToken(true));
            }
#endif
            return MainThreadDispatcher.EnqueueAsync(OnInitialize);
        }
#if SHIP_BUILD
        private bool LoadLaunchScene()
        {
            Guid GetGuidSafe(string str)
            {
                Guid.TryParse(str, out var guid);
                return guid;
            }
            bool TryGetSceneAssetSafe(SceneSettings settings, out SceneAsset asset)
            {
                asset = null;
                for (int i = 0; i < settings.Scenes.Count; i++)
                {
                    asset = (SceneAsset)Assets.GetAssetFromGuid(GetGuidSafe(settings.Scenes[i]));

                    if (asset)
                    {
                        return true;
                    }
                }

                return false;
            }
            Debug.Log("Load Launch Scene");
            var sceneSettings = EngineServices.GetService<EngineDataService>().GetProjectSettings().SceneSettings;
            SceneManager.Initialize();
            var scene = (SceneAsset)Assets.GetAssetFromGuid(GetGuidSafe(sceneSettings.LaunchScene));

            if (!scene)
            {
                Debug.Warn("Launch scene wasn't found, make sure it's added to the project settings.");

                if (!TryGetSceneAssetSafe(sceneSettings, out scene))
                {
                    Debug.Warn("No valid scenes were found to load.");
                    return false;
                }
            }

            SceneDeserializer.DeserializeScene(scene.SceneIR, SceneManager.ActiveScene);

            return true;
        }
#endif
        protected abstract void OnInitialize();
        public virtual void OnFocusEnter() { }
        public virtual void OnFocusExit() { }
    }
}