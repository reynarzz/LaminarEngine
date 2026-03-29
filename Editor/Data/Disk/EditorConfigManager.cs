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
        public string RootDir { get; set; }
        public string AssemblyName { get; set; }
    }

    internal static class EditorConfigManager
    {
        public static EditorLoadedProjectData GetLastLoadedProject()
        {
            return  null;
        }

        public static bool IsProjectLoaded()
        {
            return false;
        }
    }
}
