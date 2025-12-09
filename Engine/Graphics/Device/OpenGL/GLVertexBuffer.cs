using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if DESKTOP
    using static OpenGL.GL;
#else
    using static OpenGL.ES.GLES30;
#endif
namespace Engine.Graphics.OpenGL
{
    internal class GLVertexBuffer : GLBuffer
    {
        internal GLVertexBuffer() : base(GL_ARRAY_BUFFER) { }
    }
}
