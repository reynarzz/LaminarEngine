using System.Runtime.InteropServices;

namespace Engine
{
    internal static partial class RuntimeCore
    {
        private const string RUNTIME_CORE_DLL = "RuntimeCore";
        [DllImport(RUNTIME_CORE_DLL)]
        internal static extern void InitSonyGamepad();

        [DllImport(RUNTIME_CORE_DLL)]
        internal static extern void UpdateSonyGamepad(float deltaTime);
    }
}