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
        [SerializedField, HideFromInspector] public bool[] CollisionMatrix { get; set; } = [];
    }

    internal class LayersSettings
    {
        [SerializedField] internal string[] Layers { get; set; } = ["Default", "Background", "Foreground", "Player", "UI"];
    }

    // TODO: I cannot serialize Guids directly, so for now I'm using strings.
    internal class SceneSettings
    {
        [SerializedField] public string LaunchScene { get; set; } = Guid.Empty.ToString();
        [SerializedField] internal List<string> Scenes { get; set; } = [];
    }

    internal class ProjectSettings : IEngineService
    {
        [SerializedField] internal LayersSettings LayerSettings { get; set; } = new();
        [SerializedField] internal SceneSettings SceneSettings { get; set; } = new();
        [SerializedField] internal PhysicsSettings Physics { get; set; } = new();
    }
}
