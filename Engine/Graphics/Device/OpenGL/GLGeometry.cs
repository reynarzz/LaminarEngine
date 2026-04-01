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
    internal unsafe class GLGeometry : GLGfxResource<GeometryDescriptor>
    {
        private readonly GLVertexBuffer _vertBuffer;
        private GLIndexBuffer _indexBuffer;
        private GLIndexBuffer _sharedBuffer;
        private static uint _prevVAO;
        public GLGeometry() : base(glGenVertexArray, glDeleteVertexArray, glBindVertexArray)
        {
            _vertBuffer = new GLVertexBuffer();
        }

        protected override bool CreateResource(GeometryDescriptor descriptor)
        {
            uint prevVAO = GetBoundVAO();// _prevVAO;
            
            Bind();
            GLHelpers.CheckGLError();
            if (!_vertBuffer.Create(descriptor.VertexDesc.BufferDesc))
            {
                Debug.Error("Failed to create vertex buffer.");
                Unbind();
                return false;
            }
            GLHelpers.CheckGLError();

            if (descriptor.SharedIndexBuffer == null && descriptor.IndexDesc != null)
            {
                _indexBuffer = new GLIndexBuffer();

                if (!_indexBuffer.Create(descriptor.IndexDesc))
                {
                    Debug.Error("Failed to create index buffer.");
                    Unbind();
                    return false;
                }
                GLHelpers.CheckGLError();

                _indexBuffer.Bind();
                GLHelpers.CheckGLError();

            }
            else if (descriptor.SharedIndexBuffer != null)
            {
                (descriptor.SharedIndexBuffer as GLIndexBuffer).Bind();
                GLHelpers.CheckGLError();

                _sharedBuffer = descriptor.SharedIndexBuffer as GLIndexBuffer;
            }
           
            _vertBuffer.Bind();
            GLHelpers.CheckGLError();

            for (uint i = 0; i < descriptor.VertexDesc.Attribs.Length; i++)
            {
                var attrib = descriptor.VertexDesc.Attribs[(int)i];

                if (attrib.Type == GfxValueType.Int || attrib.Type == GfxValueType.Uint)
                {
                    glVertexAttribIPointer(i, attrib.Count, attrib.Type.ToGL(), attrib.Stride, attrib.Offset);
                }
                else
                {
                    glVertexAttribPointer(i, attrib.Count, attrib.Type.ToGL(), attrib.Normalized, attrib.Stride, attrib.Offset);
                }
                GLHelpers.CheckGLError();

                glEnableVertexAttribArray(i);
                GLHelpers.CheckGLError();

            }

            //  Unbind();

            _prevVAO = prevVAO;
            glBindVertexArray(_prevVAO);
            GLHelpers.CheckGLError();

            return true;
        }
        private static uint GetBoundVAO()
        {
            int result;
//#if DESKTOP
            glGetIntegerv(GL_VERTEX_ARRAY_BINDING, &result);
            GLHelpers.CheckGLError();

            //#else
            //        int[] arr = new int[1];
            //        glGetIntegerv(GL_VERTEX_ARRAY_BINDING, arr);
            //        result = arr[0];
            //#endif
            return (uint)result;
        }
        internal override void UpdateResource(GeometryDescriptor descriptor)
        {
            uint prevVAO = GetBoundVAO();// _prevVAO;
            GLHelpers.CheckGLError();

            Bind();
            GLHelpers.CheckGLError();

            _vertBuffer.Update(descriptor.VertexDesc.BufferDesc);
            GLHelpers.CheckGLError();

            _vertBuffer.Bind();
            GLHelpers.CheckGLError();

            if (descriptor.SharedIndexBuffer != null && descriptor.SharedIndexBuffer != _sharedBuffer)
            {
                Debug.Error("Shared index buffer error");
                throw new Exception("Different shared index buffer, please handle it");
            }
            if (_sharedBuffer != null)
            {
                _sharedBuffer.Bind();
                GLHelpers.CheckGLError();

            }
            else if (_indexBuffer != null)
            {
                _indexBuffer.Update(descriptor.IndexDesc);
                GLHelpers.CheckGLError();

                _indexBuffer.Bind();
                GLHelpers.CheckGLError();

            }

            _prevVAO = prevVAO;
            glBindVertexArray(prevVAO);
            GLHelpers.CheckGLError();

        }

        internal override void Bind()
        {
            base.Bind();
            _prevVAO = Handle;

        }
        protected override void FreeResource()
        {
            _vertBuffer.Dispose();
            GLHelpers.CheckGLError();

            _sharedBuffer = null;
            if (_indexBuffer != null)
            {
                _indexBuffer.Dispose();
                GLHelpers.CheckGLError();

            }
            base.FreeResource();
        }
    }
}