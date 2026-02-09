using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public enum BlendFactor
    {
        Zero,
        One,
        SrcColor,
        OneMinusSrcColor,
        DstColor,
        OneMinusDstColor,
        SrcAlpha,
        OneMinusSrcAlpha,
        DstAlpha,
        OneMinusDstAlpha,
        ConstantColor,
        OneMinusConstantColor,
        ConstantAlpha,
        OneMinusConstantAlpha,
        SrcAlphaSaturate
    }

    public enum BlendEquation
    {
        FuncAdd,
        FuncSubtract,
        FuncReverseSubtract,
        Min,
        Max
    }

    public class Blending
    {
        [SerializedField] public bool Enabled;

        [SerializedField] public BlendFactor SrcFactor = BlendFactor.One;
        [SerializedField] public BlendFactor DstFactor = BlendFactor.Zero;
        [SerializedField] public BlendEquation Equation = BlendEquation.FuncAdd;

        public static Blending Transparent => new Blending() { Enabled = true, SrcFactor = BlendFactor.SrcAlpha, DstFactor = BlendFactor.OneMinusSrcAlpha };
        public static Blending Additive => new Blending() { Enabled = true, SrcFactor = BlendFactor.One, DstFactor = BlendFactor.One };
    }
}
