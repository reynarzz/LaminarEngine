using Editor.Build;
using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Serialization;
using Engine.Utils;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Data
{
    internal static class EditorDataManager
    {
        private static BuildSettings _buildSettings;
        public static BuildSettings BuildSettings => _buildSettings ?? (_buildSettings = LoadProjectData<BuildSettings>(BUILD_SETTINGS));

        private const string BUILD_SETTINGS = "BuildSettings";

        public static void SaveAll()
        {
            // TODO: Save all unsaved data to disk.

            SaveBuildSettings();
        }

        private static void SaveBuildSettings()
        {
            WriteProjectData(BUILD_SETTINGS, _buildSettings);
        }

        private static void WriteProjectData(string name, object data)
        {
            var projectSettings = Paths.GetProjectSettingsFolder();
            var json = EditorJsonUtils.Serialize(Serializer.Serialize(data));
            File.WriteAllText(Path.Combine(projectSettings, $"{name}{EditorPaths.EDITOR_DATA_EXTENSION}"), json);
        }

        private static T LoadProjectData<T>(string name) where T : class, new()
        {
            var projectSettings = Paths.GetProjectSettingsFolder();
            var filePath = Path.Combine(projectSettings, $"{name}{EditorPaths.EDITOR_DATA_EXTENSION}");

            if (!File.Exists(filePath))
            {
                return new T();
            }
            var json = File.ReadAllText(filePath);
            var ir = EditorJsonUtils.Deserialize<List<SerializedPropertyIR>>(json);
            var value = Deserializer.Deserialize<T>(ir);

            if (value == null)
            {
                value = ReflectionUtils.GetDefaultValueInstance(typeof(T)) as T;
            }
            return value;
        }
    }
}
