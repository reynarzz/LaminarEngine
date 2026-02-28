using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Data
{
    internal class PhysicsSettings
    {
        [SerializedField] internal float FixedTimeStep { get; set; } = 0.02f;
    }

    internal class ProjectSettings : IEngineService
    {
        [SerializedField] internal Dictionary<int, string> Layers { get; set; } = new() { { 0, "Default" } };
        [SerializedField] internal List<Guid> Scenes {  get; set; }
        [SerializedField] internal PhysicsSettings Physics { get; set; } = new();
    }
}
