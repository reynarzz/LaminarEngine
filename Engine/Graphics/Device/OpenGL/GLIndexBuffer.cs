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
    // TODO: fix duplicate code
    internal class GLIndexBuffer : GLBuffer
    {
        private static uint _currentBoundBuffer;
        private static int _currentBoundBufferTarget;

        private static uint _prevBoundBuffer;
        private static int _prevBoundBufferTarget;

        internal GLIndexBuffer() : base(GL_ELEMENT_ARRAY_BUFFER) { }

        internal override void Bind()
        {
            _prevBoundBuffer = _currentBoundBuffer;
            _prevBoundBufferTarget = _currentBoundBufferTarget;
            base.Bind();
            GLHelpers.CheckGLError();

            _currentBoundBuffer = Handle;
            _currentBoundBufferTarget = Target;
        }

        internal override void Unbind()
        {
            base.Unbind();
            GLHelpers.CheckGLError();

            _currentBoundBuffer = _prevBoundBuffer;
            _currentBoundBufferTarget = _prevBoundBufferTarget;
            if (_currentBoundBuffer >= 0)
            {
                glBindBuffer(_currentBoundBufferTarget, _currentBoundBuffer);
                GLHelpers.CheckGLError();

            }
        }
    }
}
