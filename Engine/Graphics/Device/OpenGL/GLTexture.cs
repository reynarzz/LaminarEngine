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
    internal class GLTexture : GLGfxResource<TextureDescriptor>
    {
        private int _slotBound = 0;
        private bool _isMultisample = false;
        private int _width;
        private int _height;
        private int _channels;
        private static uint _prevBoundTexture;
        private static int _prevBoundSlot;

        public GLTexture() : base(glGenTexture, glDeleteTexture)
        {
        }

        private byte[] _buffer;
        protected unsafe override bool CreateResource(TextureDescriptor descriptor)
        {
            _isMultisample = descriptor.IsMultiSample;

            Bind();
            _width = descriptor.Width;
            _height = descriptor.Height;
            _channels = descriptor.Channels;
            if (!descriptor.IsMultiSample)
            {
                if (descriptor.Buffer != null)
                {
                    _buffer = descriptor.Buffer;
                }
                
                SetTextureFeatures(descriptor);
                
                fixed (byte* data = _buffer)
                {
                    glTexImage2D(GL_TEXTURE_2D, 0, GetInternalFormat(descriptor.Channels), descriptor.Width, descriptor.Height, 0,
                        GetFormat(descriptor.Channels), GL_UNSIGNED_BYTE, data);
                    GLHelpers.CheckGLError();
                }
            }
            else
            {
                //glTexImage2DMultisample(GL_TEXTURE_2D_MULTISAMPLE, descriptor.SamplesCount, GL_RGBA8,
                //                         descriptor.Width, descriptor.Height, true);

                throw new Exception("Not multiplatorm");
            }

            Unbind();

            return true;
        }

        internal override unsafe void UpdateResource(TextureDescriptor descriptor)
        {
            var prevBoundText = _prevBoundTexture;
            var prevBoundSlot = _prevBoundSlot;
            Bind();
            GLHelpers.CheckGLError();

            SetTextureFeatures(descriptor);

            GLHelpers.CheckGLError();
            fixed (void* data = descriptor.Buffer)
            {
                var isSizeFit = DoesNewSizeFit(descriptor);

                int prev;
                glGetIntegerv(GL_UNPACK_ALIGNMENT, &prev);

                GLHelpers.CheckGLError();
                glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
                GLHelpers.CheckGLError();

                _channels = descriptor.Channels;
                glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
                GLHelpers.CheckGLError();

                if (isSizeFit)
                {
                    glTexSubImage2D(GL_TEXTURE_2D, 0, descriptor.XOffset, descriptor.YOffset,
                        descriptor.Width, descriptor.Height, GetFormat(descriptor.Channels), GL_UNSIGNED_BYTE, data);
                    GLHelpers.CheckGLError();
                }
                else
                {
                    _width = descriptor.Width;
                    _height = descriptor.Height;
                    glTexImage2D(GL_TEXTURE_2D, 0, GetInternalFormat(descriptor.Channels), descriptor.Width, descriptor.Height, 0,
                        GetFormat(descriptor.Channels), GL_UNSIGNED_BYTE, data);
                    GLHelpers.CheckGLError();
                }

                //if (descriptor.EnableMipMaps)
                //{
                //    glGenerateMipmap(GL_TEXTURE_2D);
                //}

                glPixelStorei(GL_UNPACK_ALIGNMENT, prev);
                GLHelpers.CheckGLError();
            }

            Unbind();
            GLHelpers.CheckGLError();

            if (prevBoundText >= 0)
            {
                Bind(prevBoundText, prevBoundSlot);
                GLHelpers.CheckGLError();
            }
        }

        private int GetFormat(int channels)
        {
            return channels switch
            {
                1 => GL_RED,
                2 => GL_RG,
                3 => GL_RGB,
                4 => GL_RGBA,
                _ => GL_RGBA
            };
        }

        private int GetInternalFormat(int channels)
        {
            return channels switch
            {
                1 => GL_R8,
                2 => GL_RG8,
                3 => GL_RGB8,
                4 => GL_RGBA8,
                _ => GL_RGBA8
            };
        }

        private bool DoesNewSizeFit(TextureDescriptor descriptor)
        {
            return descriptor.XOffset + descriptor.Width <= _width &&
                   descriptor.YOffset + descriptor.Height <= _height;
        }


        private void SetTextureFeatures(TextureDescriptor descriptor)
        {
            int minFilter = 0;
            int magFilter = 0;

            switch (descriptor.Filter)
            {
                case TextureFilterMode.Nearest:
                    if (descriptor.EnableMipMaps)
                    {
                        minFilter = GL_NEAREST_MIPMAP_NEAREST;
                        magFilter = GL_NEAREST;
                    }
                    else
                    {
                        minFilter = GL_NEAREST;
                        magFilter = GL_NEAREST;
                    }

                    break;
                case TextureFilterMode.Linear:
                    if (descriptor.EnableMipMaps)
                    {
                        minFilter = GL_LINEAR_MIPMAP_LINEAR;
                        magFilter = GL_LINEAR;
                    }
                    else
                    {
                        minFilter = GL_LINEAR;
                        magFilter = GL_LINEAR;
                    }

                    break;
                default:
                    throw new Exception("GL filter not implemented");
            }

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, minFilter);
            GLHelpers.CheckGLError();

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, magFilter);
            GLHelpers.CheckGLError();

            int texMode = 0;
            switch (descriptor.Mode)
            {
                case TextureWrapMode.Clamp:
                    texMode = GL_CLAMP_TO_EDGE;
                    break;
                case TextureWrapMode.Repeat:
                    texMode = GL_REPEAT;
                    break;
                default:
                    break;
            }

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, texMode);
            GLHelpers.CheckGLError();

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, texMode);
            GLHelpers.CheckGLError();
        }

        /// <summary>
        /// Binds texture to first slot (0)
        /// </summary>
        internal override void Bind()
        {
            Bind(0);
        }

        internal void Bind(int slot)
        {
            Bind(Handle, slot);
        }

        internal void Bind(uint handle, int slot)
        {
            if (_isMultisample)
            {
#if !MOBILE
                glBindTexture(GL_TEXTURE_2D_MULTISAMPLE, handle);
#endif
            }
            else
            {
                _slotBound = slot;
                glActiveTexture(GL_TEXTURE0 + slot);
                GLHelpers.CheckGLError();

                glBindTexture(GL_TEXTURE_2D, handle);
                GLHelpers.CheckGLError();
            }

            _prevBoundTexture = handle;
            _prevBoundSlot = slot;
        }

        internal override void Unbind()
        {
            if (_isMultisample)
            {
#if !MOBILE
                glBindTexture(GL_TEXTURE_2D_MULTISAMPLE, 0);
                GLHelpers.CheckGLError();

#endif
            }
            else
            {
                glActiveTexture(GL_TEXTURE0 + _slotBound);
                GLHelpers.CheckGLError();

                glBindTexture(GL_TEXTURE_2D, 0);
                GLHelpers.CheckGLError();

                _slotBound = 0;
            }
        }
    }
}