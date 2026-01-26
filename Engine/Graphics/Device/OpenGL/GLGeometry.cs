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
            uint prevVAO = _prevVAO;
            
            Bind();
            if (!_vertBuffer.Create(descriptor.VertexDesc.BufferDesc))
            {
                Debug.Error("Failed to create vertex buffer.");
                Unbind();
                return false;
            }

            if (descriptor.SharedIndexBuffer == null && descriptor.IndexDesc != null)
            {
                _indexBuffer = new GLIndexBuffer();

                if (!_indexBuffer.Create(descriptor.IndexDesc))
                {
                    Debug.Error("Failed to create index buffer.");
                    Unbind();
                    return false;
                }

                _indexBuffer.Bind();
            }
            else if (descriptor.SharedIndexBuffer != null)
            {
                (descriptor.SharedIndexBuffer as GLIndexBuffer).Bind();

                _sharedBuffer = descriptor.SharedIndexBuffer as GLIndexBuffer;
            }
           
            _vertBuffer.Bind();

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

                glEnableVertexAttribArray(i);
            }

            //  Unbind();

            _prevVAO = prevVAO;
            glBindVertexArray(_prevVAO);

            return true;
        }

        internal override void UpdateResource(GeometryDescriptor descriptor)
        {
            uint prevVAO = _prevVAO;

            Bind();
            _vertBuffer.Update(descriptor.VertexDesc.BufferDesc);
            _vertBuffer.Bind();

            if (descriptor.SharedIndexBuffer != null && descriptor.SharedIndexBuffer != _sharedBuffer)
            {
                Debug.Error("Shared index buffer error");
                throw new Exception("Different shared index buffer, please handle it");
            }
            if (_sharedBuffer != null)
            {
                _sharedBuffer.Bind();
            }
            else if (_indexBuffer != null)
            {
                _indexBuffer.Update(descriptor.IndexDesc);
                _indexBuffer.Bind();
            }

            _prevVAO = prevVAO;
            glBindVertexArray(_prevVAO);
        }

        internal override void Bind()
        {
            base.Bind();
            _prevVAO = Handle;

        }
        protected override void FreeResource()
        {
            _vertBuffer.Dispose();
            _sharedBuffer = null;
            if (_indexBuffer != null)
            {
                _indexBuffer.Dispose();
            }
            base.FreeResource();
        }
    }
}