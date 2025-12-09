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
        public static bool IsFullScreen { get => WindowManager.Window.IsFullScreen; set => WindowManager.Window.IsFullScreen = value; }
        public static void SetScreenSize(int width, int height)
        {
            WindowManager.Window.SetWindowSize(width, height);
        }
    }
}
