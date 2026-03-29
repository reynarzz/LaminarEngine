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
        public sealed override async Task<LayerInitResult> InitializeAsync()
        {
#if SHIP_BUILD
            var result = LoadMainScene();
            if (!result)
            {
                return LayerInitResult.Error;
            }
#endif
            await MainThreadDispatcher.EnqueueAsync(OnInitialize);

            return LayerInitResult.Success;
        }
#if SHIP_BUILD
        private bool LoadMainScene()
        {
            Guid GetGuidSafe(string str)
            {
                Guid.TryParse(str, out var guid);
                return guid;
            }
            bool TryGetSceneAssetSafe(SceneSettings settings, out SceneAsset asset)
            {
                asset = null;
                for (int i = 0; i < settings.Scenes.Length; i++)
                {
                    asset = (SceneAsset)Assets.GetAssetFromGuid(GetGuidSafe(settings.Scenes[i].RefId));

                    if (asset)
                    {
                        return true;
                    }
                }

                return false;
            }
            var sceneSettings = EngineServices.GetService<EngineDataService>().GetProjectSettings().SceneSettings;
            SceneManager.Initialize();
            var scene = (SceneAsset)Assets.GetAssetFromGuid(GetGuidSafe(sceneSettings.MainScene));

            if (!scene)
            {
                Debug.Warn("Main scene wasn't found, make sure it's added to the project settings.");

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