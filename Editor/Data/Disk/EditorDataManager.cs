using Editor.Build;
using Editor.Serialization;
using Editor.Utils;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Data
{
    internal static class EditorDataManager
    {
        private static BuildSettings _buildSettingsData;
        public static BuildSettings BuildSettings => _buildSettingsData ?? (_buildSettingsData = new());

        public static void SaveAll()
        {
            // TODO: Save all unsaved data to disk.

            SaveBuildSettings();
        }

        private static void SaveBuildSettings()
        {
            WriteProjectData("BuildSettings", _buildSettingsData);
        }

        private static void WriteProjectData(string name, object data)
        {
            var projectSettings = Paths.GetProjectSettingsFolder();
            var json = EditorJsonUtils.Serialize(Serializer.Serialize(data));
            File.WriteAllText(Path.Combine(projectSettings, $"{name}{EditorPaths.EDITOR_DATA_EXTENSION}"), json);
        }
    }
}
