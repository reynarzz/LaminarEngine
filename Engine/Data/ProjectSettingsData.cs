using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Data
{
    internal class PhysicsSettings
    {
        [SerializedField] public vec3 Gravity { get; set; } = new vec3(0, -9.8f, 0);
        [SerializedField] internal float FixedTimeStep { get; set; } = 0.02f;
        [SerializedField, HideFromInspector] public bool[] CollisionMatrix { get; set; } = new bool[LayerMask.ARRAY_SIZE];
    }

    internal class LayersSettings
    {
        [SerializedField] internal string[] Layers;

        public LayersSettings()
        {
            Layers = GetDefaultLayers();
        }
        internal string[] GetDefaultLayers()
        {
            var layers = new string[sizeof(ulong) * 8];
            layers[0] = "Default";
            layers[1] = "Background";
            layers[2] = "Foreground";
            layers[3] = "UI";
            layers[4] = "Player";

            return layers;
        }
    }

    internal class SceneSettings
    {
        internal class SceneBuildInfo
        {
            [SerializedField] public bool IsBuildAdded { get; set; }
            // TODO: I cannot serialize Guids directly, so for now I'm using strings.
            [SerializedField] public string RefId { get; set; }
        }

        [SerializedField] public string MainScene { get; set; } = Guid.Empty.ToString();
        [SerializedField] internal SceneBuildInfo[] Scenes = [];

        internal Guid[] GetAllScenesId()
        {
            bool isValid(string str, out Guid guid)
            {
                guid = Guid.Empty;
                return !string.IsNullOrEmpty(str) && Guid.TryParse(str, out guid) && guid != Guid.Empty;
            }

            int count = Scenes.Length + 1;
            var scenesId = new Guid[count];
            int index = 0;

            for (int i = 0; i < Scenes.Length; i++)
            {
                var sceneInfo = Scenes[i];
                if (!sceneInfo.IsBuildAdded)
                    continue;

                if (isValid(sceneInfo.RefId, out var guid))
                {
                    scenesId[index++] = guid;
                }
            }

            if (isValid(MainScene, out var mainId))
            {
                scenesId[index++] = mainId;
            }

            if (index == scenesId.Length)
            {
                return scenesId;
            }

            Array.Resize(ref scenesId, index);
            return scenesId;
        }
    }

    internal class ProjectSettingsData : IEngineService
    {
        [SerializedField] internal LayersSettings LayerSettings { get; set; } = new();
        [SerializedField] internal SceneSettings SceneSettings { get; set; } = new();
        [SerializedField] internal PhysicsSettings Physics { get; set; } = new();
    }
}
