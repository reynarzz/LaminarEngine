using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Data
{
    internal class EditorLoadedProjectData
    {
        public string ProjectName { get; set; }
        public string ProjectRootPath { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyAbsolutePath { get; set; }
    }

    internal static class EditorConfigManager
    {
        private static EditorLoadedProjectData _loadedProject;
        public static EditorLoadedProjectData GetLastLoadedProject()
        {
            return _loadedProject;
        }

        public static bool IsProjectLoaded()
        {
            return _loadedProject != null;
        }

        internal static void SetLoadedProject(EditorLoadedProjectData projectData)
        {
            _loadedProject = projectData;
        }
    }
}
