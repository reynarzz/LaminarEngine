using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using System.Runtime.InteropServices;

namespace Editor.Utils
{
    public static class NativeLogger
    {
        private static EditorNatives.LogCallback _callback; 

        private static void OnNativeLog(IntPtr message)
        {
            string msg = Marshal.PtrToStringAnsi(message);
            Debug.Log(msg); 
        }

        public static void Init()
        {
            _callback = OnNativeLog;
            EditorNatives.RegisterLogCallback(_callback);
        }
    }
}
