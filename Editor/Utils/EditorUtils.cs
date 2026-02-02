using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    internal static class EditorUtils
    {
        public static bool IsNativeDll(string dllPath)
        {
            if (!File.Exists(dllPath))
            {
                Debug.Error($"Dll not found at path: {dllPath}");
                return false;
            }

            try
            {
                AssemblyName.GetAssemblyName(dllPath);
                return false;
            }
            catch (BadImageFormatException)
            {
                return true;
            }
        }
    }
}
