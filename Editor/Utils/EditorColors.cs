using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    internal static class EditorColors
    {
        //public static Color MainColor { get; } = new Color(0.133f, 0.545f, 0.133f, 1.0f);
        // public static Color MainColor { get; } = new Color32(174, 198, 216, 255);
        //public static Color MainColor { get; } = new Color(0.1686f, 0.1765f, 0.2588f, 1.0f);
        public static Color MainColor => Green;// Color.FromRGBHex(0xD6CE93);
        public static Color TabColor => Green;// Color.FromRGBHex(0xF46036);
        public static Color Background { get; } = new Color(0.14509805f, 0.14509805f, 0.14901961f, 1);

        public static Color Green { get; } = new Color(0.133f, 0.545f, 0.133f, 1.0f);

    }
}
