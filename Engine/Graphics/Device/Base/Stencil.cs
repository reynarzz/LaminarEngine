using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public enum StencilFunc
    {
        Never,      // Never passes
        Less,       // Passes if (ref & mask) < (stencil & mask)
        Lequal,     // Passes if (ref & mask) <= (stencil & mask)
        Greater,    // Passes if (ref & mask) > (stencil & mask)
        Gequal,     // Passes if (ref & mask) >= (stencil & mask)
        Equal,      // Passes if (ref & mask) == (stencil & mask)
        NotEqual,   // Passes if (ref & mask) != (stencil & mask)
        Always      // Always passes
    };

    public enum StencilOp
    {
        Keep,       // Keep current value
        Zero,       // Set stencil to 0
        Replace,    // Replace stencil with ref value
        Incr,       // Increment stencil, clamp at max
        IncrWrap,   // Increment stencil, wrap on overflow
        Decr,       // Decrement stencil, clamp at 0
        DecrWrap,   // Decrement stencil, wrap on underflow
        Invert      // Bitwise invert stencil value
    };

    public class Stencil
    {
        [SerializedField] public bool Enabled;

        /// <summary>
        /// Comparison function.
        /// </summary>
        [SerializedField] public StencilFunc Func;

        /// <summary>
        /// Reference value.
        /// </summary>
        [SerializedField] public int Ref;
        [SerializedField] public uint Mask = 0xFF;    // Bitmask for comparison

        /// <summary>
        /// WWhen stencil test fails.
        /// </summary>
        [SerializedField] public StencilOp FailOp;

        /// <summary>
        /// When stencil passes but depth fails.
        /// </summary>
        [SerializedField] public StencilOp ZFailOp;

        /// <summary>
        /// When both stencil and depth pass.
        /// </summary>
        [SerializedField] public StencilOp ZPassOp;    
    }
}