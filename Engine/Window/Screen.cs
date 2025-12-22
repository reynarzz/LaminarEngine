using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Window abstraction
    /// </summary>
    public static class Screen
    {
        public static int Width => WindowManager.Window.Width;
        public static int Height => WindowManager.Window.Height;

        public static int PhysicalWidth => WindowManager.Window.PhysicalWidth;
        public static int PhysicalHeight => WindowManager.Window.PhysicalHeight;

        public static int OffsetX => WindowManager.Window.OffsetX;
        public static int OffsetY => WindowManager.Window.OffsetY;

        public static bool IsFullScreen { get => WindowManager.Window.IsFullScreen; set => WindowManager.Window.IsFullScreen = value; }
        public static void SetScreenSize(int width, int height)
        {
            WindowManager.Window.SetWindowSize(width, height);
        }
    }
}
