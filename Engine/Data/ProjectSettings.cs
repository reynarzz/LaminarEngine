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
        [SerializedField, HideFromInspector] public bool[,] CollisionMatrix { get; set; }
    }
    internal class LayersSettings
    {
        [SerializedField] internal string[] Layers { get; set; } = new[] { "Default" };
    }

    internal class SceneSettings
    {
        public class SceneConfig
        {
            public Guid Id { get; set; }
            public bool Build { get; set; }
        }
        [SerializedField] public SceneAsset LaunchScene { get; set; }
        [SerializedField] internal List<SceneAsset> Scenes { get; set; } = new();
    }
    internal class ProjectSettings : IEngineService
    {
        [SerializedField] internal LayersSettings LayerSettings { get; set; } = new();
        [SerializedField] internal SceneSettings SceneSettings { get; set; } = new();
        [SerializedField] internal PhysicsSettings Physics { get; set; } = new();
    }
}
