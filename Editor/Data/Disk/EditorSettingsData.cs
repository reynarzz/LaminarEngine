using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class EditorSettingsData
    {
        [SerializedField] internal vec3 CameraPosition { get; set; }
        [SerializedField] internal quat CameraRotation { get; set; }
        [SerializedField] internal string OpenedSceneRefId { get; set; } = string.Empty;
        [SerializedField] internal Dictionary<string, WindowSettings> WindowsSettings { get; set; } = new();
    }

    internal class WindowSettings
    {
        [SerializedField] public string Name { get; set; }
        [SerializedField] public bool IsOpened { get; set; }
    }
}
