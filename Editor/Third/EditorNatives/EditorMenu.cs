using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;

namespace Editor
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Callback();

    internal static class EditorMenu
    {
        private readonly static Dictionary<string, Callback> _callbacks = new();



        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void PushMenu(string path, IntPtr callback);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void PushMenuToggle(string path, IntPtr callback, byte toggle);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void PushMenuShortcut(string path, IntPtr callback, byte toggle, string shortcut);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Toggle(string path, byte isChecked);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Enable(string path, byte enabled);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern byte IsEnabled(string path);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern byte IsChecked(string path);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Separator(string path);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SeparatorAt(string path, int index);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RemoveSeparators(string path);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RemoveSeparator(string path);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RemoveSeparatorAt(string path, int index);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void DestroyMenuAtPath(string path);


        private static IntPtr RegisterCallbackPtr(string path, Callback callback)
        {
            _callbacks[path] = callback;
            return Marshal.GetFunctionPointerForDelegate(callback);
        }

        private static bool ToBool(byte v)
        {
            return (v & 1) == 1;
        }

        private static byte ToByte(bool v)
        {
            return (byte)(v ? 1 : 0);
        }

        internal static void PushMenu(string path, Callback callback)
        {
            PushMenu(path, RegisterCallbackPtr(path, callback));
        }

        internal static void PushMenu(string path, Callback callback, bool toggle)
        {
            PushMenuToggle(path, RegisterCallbackPtr(path, callback), ToByte(toggle));
        }

        internal static void PushMenu(string path, Callback callback, bool toggle, string shortcut)
        {
            PushMenuShortcut(path, RegisterCallbackPtr(path, callback), ToByte(toggle), shortcut);
        }

        internal static void Toggle(string path, bool isChecked)
        {
            Toggle(path, ToByte(isChecked));
        }

        public static void SetEnabled(string path, bool enabled)
        {
            Enable(path, ToByte(enabled));
        }

        public static bool GetEnabled(string path)
        {
            return ToBool(IsEnabled(path));
        }

        public static bool GetChecked(string path)
        {
            return ToBool(IsChecked(path));
        }

        public static void AddSeparator(string path)
        {
            Separator(path);
        }

        public static void AddSeparator(string path, int index)
        {
            SeparatorAt(path, index);
        }

        public static void ClearSeparators(string path)
        {
            RemoveSeparators(path);
        }

        public static void RemoveSeparatorAtIndex(string path, int index)
        {
            RemoveSeparatorAt(path, index);
        }

        public static void RemoveFirstSeparator(string path)
        {
            RemoveSeparator(path);
        }

        public static void DestroyMenu(string path)
        {
            DestroyMenuAtPath(path);
        }
    }
}
