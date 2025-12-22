using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct Rect
    {
        public float X, Y, Width, Height;

        public Rect(Vector2 position, Vector2 size) : this(new vec2(position.X, position.Y), new vec2(size.X, size.Y))
        {

        }
        public Rect(vec2 position, vec2 size)
        {
            X = position.x;
            Y = position.y;
            Width = size.x;
            Height = size.y;
        }

        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float XMin => X;
        public float YMin => Y;
        public float XMax => X + Width;
        public float YMax => Y + Height;

        public vec2 Size => new vec2(Width, Height);

        public vec2 Min => new vec2(X, Y);
        public vec2 Max => Min + Size;
        public vec2 Center => Min + Size * 0.5f;

        public bool Contains(vec2 p)
        {
            return p.x >= XMin && p.x <= XMax &&
                   p.y >= YMin && p.y <= YMax;
        }
        public bool Overlaps(Rect other)
        {
            return XMin < other.XMax && XMax > other.XMin &&
                   YMin < other.YMax && YMax > other.YMin;

        }

        public Rect Move(vec2 delta) { return new Rect(Min + delta, Size); }
        public Rect Inflate(float d) { return new Rect(Min - new vec2(d), Size + new vec2(d * 2)); }

        public override string ToString() => $"Rect({Min.x},{Min.y},{Width},{Height})";
    }

}



