using Editor.Build;
using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Data;
using Engine.Serialization;
using Engine.Utils;
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
        public static BuildSettings BuildSettings => _buildSettings ?? (_buildSettings = LoadData<BuildSettings>(BUILD_SETTINGS));


        private const string BUILD_SETTINGS = "BuildSettings";
        private const string PROJECT_SETTINGS = "ProjectSettings";
        private const string EDITOR_SETTINGS = "EditorSettings";


        internal static void Init()
        {
            InitProjectSettings();
        }

        private static void InitProjectSettings()
        {
            var service = EngineServices.GetService<EngineDataService>();
            var projectSettings = LoadData<ProjectSettingsData>(PROJECT_SETTINGS);
            service.Initialize(projectSettings);
        }

        public static void SaveAll()
        {
            // TODO: Save all unsaved data to disk.

            SaveBuildSettings();
            SaveProjectSettings();
        }

        private static void SaveBuildSettings()
        {
            WriteData(BUILD_SETTINGS, _buildSettings);
        }

        internal static void SaveProjectSettings()
        {
            var service = EngineServices.GetService<EngineDataService>();
            WriteData(PROJECT_SETTINGS, service.GetProjectSettings());
        }

        private static void WriteData(string name, object data)
        {
            var projectSettings = Paths.GetProjectSettingsFolder();
            var json = EditorJsonUtils.Serialize(Serializer.Serialize(data));
            File.WriteAllText(Path.Combine(projectSettings, $"{name}{EditorPaths.EDITOR_DATA_EXTENSION}"), json);
        }

        private static T LoadData<T>(string name) where T : class, new()
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
