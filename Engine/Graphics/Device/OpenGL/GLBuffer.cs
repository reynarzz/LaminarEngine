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
    internal class GLBuffer : GLGfxResource<BufferDataDescriptor>
    {
        protected int Target { get; private set; }

        public GLBuffer(int target) : base(glGenBuffer,
                                           glDeleteBuffer,
                                           handle => glBindBuffer(target, handle))
        {
            Target = target;
        }

        protected override unsafe bool CreateResource(BufferDataDescriptor desc)
        {
            int usage = desc.Usage switch
            {
                BufferUsage.Static => GL_STATIC_DRAW,
                BufferUsage.Dynamic => GL_DYNAMIC_DRAW,
                BufferUsage.Stream => GL_STREAM_DRAW,
                BufferUsage.Invalid => 0,
                _ => 0
            };

            if (usage == 0)
            {
                Debug.Error("Invalid buffer draw mode");
                return false;
            }

            if (desc.BufferLength == 0 || desc.GetBufferUnsafePtr().ToPointer() == null)
            {
                Debug.Error("Invalid buffer data (zero/null)");

                return false;
            }

            int prevBuffer;
            prevBuffer = glGetInteger(Target);

            Bind();
            glBufferData(Target, desc.BufferLength, desc.GetBufferUnsafePtr().ToPointer(), usage);
            Unbind();

            if(prevBuffer >= 0)
            glBindBuffer(Target, (uint)prevBuffer);

            return true;
        }

        internal unsafe override void UpdateResource(BufferDataDescriptor desc)
        {
            int prevBuffer;
            glGetIntegerv(Target, &prevBuffer);

            Bind();
            unsafe
            {
                glBufferSubData(Target, desc.Offset, desc.Count, desc.GetBufferUnsafePtr().ToPointer());
            }
            Unbind();


            if (prevBuffer >= 0)
                glBindBuffer(Target, (uint)prevBuffer);

        }
    }
}