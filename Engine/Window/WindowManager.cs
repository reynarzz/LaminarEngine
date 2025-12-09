using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class WindowManager
    {
        private static IWindow _window;
        public static IWindow Window => _window;

        public WindowManager(string name, int width, int height, Color windowColor)
        {
#if WINDOWS
            _window = new Window(name, width, height, windowColor);
#endif
        }
    }
}
