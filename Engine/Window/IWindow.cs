using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public interface IWindow
    {
        event Action<int, int> OnWindowChanged;
        event Action OnWindowClose;

        string Name { get; set; }
        bool IsFullScreen { get; set; }
        int Width { get; }
        int Height { get; }
        bool CursorVisible { get; set; }
        Color StartWindowColor { get; }
        bool ShouldClose { get; }
        int MonitorCount { get; }
        bool IsInitialized { get; }
        bool CanResize { get; set; }
        int PhysicalWidth { get; }
        int PhysicalHeight { get; }
        int OffsetX { get; }
        int OffsetY { get; }
        IntPtr NativeWindow { get; }
        void SwapBuffers();
        void SetWindowSize(int width, int height);
    }
}
