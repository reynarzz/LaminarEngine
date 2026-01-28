using Editor.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Data
{
    internal static class EditorDataManager
    {
        private static BuildSettings _buildSettingsData = new();
        public static BuildSettings BuildSettings => _buildSettingsData;


        public static void SaveAll()
        {
            // TODO: Save all unsaved data to disk.

        }
    }
}
