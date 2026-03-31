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
    internal static class EditorProjectDataManager
    {
        private static BuildSettings _buildSettings;
        private static EditorSettingsData _editorSettings;
        public static BuildSettings BuildSettings => _buildSettings ?? (_buildSettings = LoadData<BuildSettings>(EditorPaths.BUILD_SETTINGS_NAME, out _));
        public static EditorSettingsData EditorSettings => _editorSettings ?? (_editorSettings = LoadData<EditorSettingsData>(EditorPaths.EDITOR_SETTINGS_NAME, out _));


        internal static void Init()
        {
            InitProjectSettings();
        }

        private static void InitProjectSettings()
        {
            var service = EngineServices.GetService<EngineDataService>();
            var projectSettings = LoadData<ProjectSettingsData>(EditorPaths.PROJECT_SETTINGS_NAME, out var isNewlyCreated);
            if (isNewlyCreated)
            {
                projectSettings.InitDefault();
            }
            service.Initialize(projectSettings);
        }

        public static void SaveAll()
        {
            // TODO: Save all unsaved data to disk.

            SaveBuildSettings();
            SaveProjectSettings();
            SaveEditorSettings();
        }

        private static void SaveBuildSettings()
        {
            WriteData(EditorPaths.BUILD_SETTINGS_NAME, _buildSettings);
        }

        internal static void SaveProjectSettings()
        {
            var service = EngineServices.GetService<EngineDataService>();
            WriteData(EditorPaths.PROJECT_SETTINGS_NAME, service.GetProjectSettings());
        }

        internal static void SaveEditorSettings()
        {
            WriteData(EditorPaths.EDITOR_SETTINGS_NAME, _editorSettings);
        }

        private static void WriteData(string name, object data)
        {
            var projectSettings = Paths.GetProjectSettingsFolder();
            var json = EditorJsonUtils.Serialize(Serializer.Serialize(data));
            File.WriteAllText(Path.Combine(projectSettings, $"{name}{EditorPaths.EDITOR_DATA_EXTENSION}"), json);
        }

        private static T LoadData<T>(string name, out bool newlyCreated) where T : class, new()
        {
            newlyCreated = false;
            var projectSettings = Paths.GetProjectSettingsFolder();
            var filePath = Path.Combine(projectSettings, $"{name}{EditorPaths.EDITOR_DATA_EXTENSION}");

            if (!File.Exists(filePath))
            {
                newlyCreated = true;
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
