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

        public GLTexture() : base(glGenTexture, glDeleteTexture)
        {
        }

        protected unsafe override bool CreateResource(TextureDescriptor descriptor)
        {
            _isMultisample = descriptor.IsMultiSample;

            Bind();

            if (!descriptor.IsMultiSample)
            {
                fixed (byte* data = descriptor.Buffer)
                {
                    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, descriptor.Width, descriptor.Height, 0,
                                 GL_RGBA, GL_UNSIGNED_BYTE, data);
                }


                int minFilter = 0;
                int magFilter = 0;

                switch (descriptor.Filter)
                {
                    case TextureFilter.Nearest:
                        if (descriptor.EnableMipMaps)
                        {
                            minFilter = GL_NEAREST_MIPMAP_NEAREST;
                            magFilter = GL_NEAREST_MIPMAP_NEAREST;
                        }
                        else
                        {
                            minFilter = GL_NEAREST;
                            magFilter = GL_NEAREST;
                        }
                        break;
                    case TextureFilter.Linear:
                        if (descriptor.EnableMipMaps)
                        {
                            minFilter = GL_LINEAR_MIPMAP_LINEAR;
                            magFilter = GL_LINEAR_MIPMAP_LINEAR;
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
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, magFilter);

                int texMode = 0;
                switch (descriptor.Mode)
                {
                    case TextureMode.Clamp:
                        texMode = GL_CLAMP_TO_EDGE;
                        break;
                    case TextureMode.Repeat:
                        texMode = GL_REPEAT;
                        break;
                    default:
                        break;
                }

                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, texMode);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, texMode);


            }
            else
            {
                glTexImage2DMultisample(GL_TEXTURE_2D_MULTISAMPLE, descriptor.SamplesCount, GL_RGBA8,
                                         descriptor.Width, descriptor.Height, true);
            }


            Unbind();

            return true;
        }

        internal override unsafe void UpdateResource(TextureDescriptor descriptor)
        {
            Bind();
            fixed (void* data = descriptor.Buffer)
            {
                glTexSubImage2D(GL_TEXTURE_2D, 0, descriptor.XOffset, descriptor.YOffset, descriptor.Width, descriptor.Height, GL_RGBA, GL_UNSIGNED_BYTE, data);
            }
            Unbind();
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
            if (_isMultisample)
            {
                glBindTexture(GL_TEXTURE_2D_MULTISAMPLE, Handle);
            }
            else
            {
                _slotBound = slot;
                glActiveTexture(GL_TEXTURE0 + slot);
                glBindTexture(GL_TEXTURE_2D, Handle);
            }
        }

        internal override void Unbind()
        {
            if (_isMultisample)
            {
                glBindTexture(GL_TEXTURE_2D_MULTISAMPLE, 0);
            }
            else
            {
                glActiveTexture(GL_TEXTURE0 + _slotBound);
                glBindTexture(GL_TEXTURE_2D, 0);
                _slotBound = 0;
            }
        }
    }
}