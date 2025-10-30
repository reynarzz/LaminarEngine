using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class UIDrawList
    {
        public struct QuadCmd
        {
            public Rect Rect;
            public Color Color;
            public Texture2D Texture;
        }

        public List<QuadCmd> Quads = new();

        public void AddQuad(Rect rect, Color color, Texture2D texture)
        {
            Quads.Add(new QuadCmd { Rect = rect, Color = color, Texture = texture });
        }

        public void AddText(string text, vec2 position, Color color)
        {
        }

        public void Clear()
        {
            Quads.Clear();
        }
    }

    public class UIRenderer
    {
        public void SubmitUI(UIDrawList drawList)
        {
            foreach (var cmd in drawList.Quads)
            {
            }
        }
    }

}
