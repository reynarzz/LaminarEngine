using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class WindowManager
    {
        public static IWindow Window { get; internal set; }
        public static IWindow PhysicalWindow { get; internal set; }
    }
}
