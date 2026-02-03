using Engine;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal static class EditorFileDialog
    {
        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void FreeAllocatedString(IntPtr str);
        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void FreeAllocatedStringArray(IntPtr array, ulong count);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void DisplayFolder(string path, byte highlight);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr OpenFile([MarshalAs(UnmanagedType.LPUTF8Str)] string openPath, IntPtr filterNames,
                                              IntPtr filterPatterns, ulong filterCount);
        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr OpenFilesWithFilters(string openPath, IntPtr filterNames, IntPtr filterPatterns,
                                                          ulong filterCount, out ulong outCount);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr OpenFiles(string openPath, out ulong outCount);


        [DllImport(EditorNatives.EDITOR_NATIVES, EntryPoint = "SaveFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr _SaveFile(string openPath, string defaultName);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SaveFileWithFilters(string openPath, string defaultName, IntPtr filterNames,
                                                         IntPtr filterPatterns, ulong filterCount);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr OpenFolder(string openPath);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr OpenFolders(string openPath, out ulong count);

        internal static void DisplayFolder(string path, bool highlight)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return;
            }

            DisplayFolder(path, EditorNativesUtils.ToByte(highlight));
        }

        internal static void DisplayFolder(string path)
        {
            DisplayFolder(path, 0);
        }

        public static bool SaveFile(string openPath, out string path)
        {
            return SaveFile(openPath, string.Empty, out path);
        }

        public static bool SaveFile(string openPath, FileFilter[] filters, out string path)
        {
            return SaveFile(openPath, string.Empty, filters, out path);
        }

        public static bool SaveFile(string openPath, string defaultName, out string path)
        {
            path = string.Empty;
            IntPtr resultPtr = _SaveFile(openPath, defaultName);

            if (resultPtr == IntPtr.Zero)
            {
                return false;
            }

            string result = Marshal.PtrToStringUTF8(resultPtr);

            FreeAllocatedString(resultPtr);
            path = Paths.ClearPathSeparation(result);

            return !string.IsNullOrEmpty(path);
        }

        public static bool SaveFile(string openPath, string defaultName, FileFilter[] filters, out string path)
        {
            path = string.Empty;

            if (filters == null || filters.Length == 0)
            {
                return SaveFile(openPath, defaultName, out path);
            }

            int count = filters.Length;

            IntPtr namesPtr = IntPtr.Zero;
            IntPtr patternsPtr = IntPtr.Zero;

            try
            {
                GetTitleAndExtensionsFiltersPtr(filters, ref namesPtr, ref patternsPtr);
                IntPtr resultPtr = SaveFileWithFilters(openPath, defaultName, namesPtr, patternsPtr, (ulong)count);

                if (resultPtr == IntPtr.Zero)
                {
                    return false;
                }

                var result = Marshal.PtrToStringUTF8(resultPtr);
                FreeAllocatedString(resultPtr);
                path = Paths.ClearPathSeparation(result);

                return !string.IsNullOrEmpty(path);
            }
            finally
            {
                if (namesPtr != IntPtr.Zero)
                    EditorNativesUtils.FreeUtf8StringArray(namesPtr, count);

                if (patternsPtr != IntPtr.Zero)
                    EditorNativesUtils.FreeUtf8StringArray(patternsPtr, count);
            }
        }


        public static bool OpenFile(string openPath, FileFilter[] filters, out string path)
        {
            path = string.Empty;
            if (filters.Length != filters.Length)
                throw new ArgumentException("Filter arrays must match length");

            var count = filters.Length;
            var namesPtr = IntPtr.Zero;
            var patternsPtr = IntPtr.Zero;

            try
            {
                GetTitleAndExtensionsFiltersPtr(filters, ref namesPtr, ref patternsPtr);

                IntPtr resultPtr = OpenFile(openPath, namesPtr, patternsPtr, (ulong)count);

                path = Paths.ClearPathSeparation(Marshal.PtrToStringUTF8(resultPtr));

                return !string.IsNullOrEmpty(path);
            }
            finally
            {
                if (namesPtr != IntPtr.Zero)
                {
                    EditorNativesUtils.FreeUtf8StringArray(namesPtr, count);
                }

                if (patternsPtr != IntPtr.Zero)
                {
                    EditorNativesUtils.FreeUtf8StringArray(patternsPtr, count);
                }
            }
        }

        public static bool OpenFiles(string openPath, out string[] paths)
        {
            paths = null;
            ulong outCount;
            try
            {
                var resultPtr = OpenFiles(openPath, out outCount);

                if (resultPtr == IntPtr.Zero || outCount == 0)
                {
                    return false;
                }

                var result = new string[outCount];

                for (ulong i = 0; i < outCount; i++)
                {
                    IntPtr strPtr = Marshal.ReadIntPtr(resultPtr, (int)(i * (ulong)IntPtr.Size));
                    result[i] = Paths.ClearPathSeparation(Marshal.PtrToStringUTF8(strPtr));
                }

                FreeAllocatedStringArray(resultPtr, outCount);
                paths = result;

                return outCount > 0;
            }
            catch (Exception e)
            {
                Debug.EngineError(e.ToString());

                return false;
            }
        }

        public static bool OpenFiles(string openPath, FileFilter[] filters, out string[] files)
        {
            int filterCount = filters?.Length ?? 0;
            files = null;
            IntPtr namesPtr = IntPtr.Zero;
            IntPtr patternsPtr = IntPtr.Zero;

            try
            {
                GetTitleAndExtensionsFiltersPtr(filters, ref namesPtr, ref patternsPtr);
                ulong outCount;
                var resultPtr = OpenFilesWithFilters(openPath, namesPtr, patternsPtr, (ulong)filterCount, out outCount);

                if (resultPtr == IntPtr.Zero || outCount == 0)
                {
                    return false;
                }

                var result = new string[outCount];

                for (ulong i = 0; i < outCount; i++)
                {
                    IntPtr strPtr = Marshal.ReadIntPtr(resultPtr, (int)(i * (ulong)IntPtr.Size));
                    result[i] = Paths.ClearPathSeparation(Marshal.PtrToStringUTF8(strPtr));
                }

                FreeAllocatedStringArray(resultPtr, outCount);
                files = result;

                return outCount > 0;
            }
            finally
            {
                if (namesPtr != IntPtr.Zero)
                {
                    EditorNativesUtils.FreeUtf8StringArray(namesPtr, filterCount);
                }

                if (patternsPtr != IntPtr.Zero)
                {
                    EditorNativesUtils.FreeUtf8StringArray(patternsPtr, filterCount);
                }
            }
        }

        private static void GetTitleAndExtensionsFiltersPtr(FileFilter[] filters, ref IntPtr titlesPtr, ref IntPtr extensionsPtr)
        {
            if (filters.Length > 0)
            {
                var titles = new string[filters.Length];
                var patterns = new string[filters.Length];

                for (int i = 0; i < filters.Length; i++)
                {
                    titles[i] = filters[i].Title;
                    patterns[i] = filters[i].Extensions;
                }

                titlesPtr = EditorNativesUtils.AllocUtf8StringArray(titles);
                extensionsPtr = EditorNativesUtils.AllocUtf8StringArray(patterns);
            }
        }

    }
    internal struct FileFilter
    {

        public string Title;
        /// <summary>
        /// Extension are divided by commas, and no 'dot' as prefix, ex: c,txt,png
        /// </summary>
        public string Extensions;
    }

}