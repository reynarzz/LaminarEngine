using System;
using System.Runtime.InteropServices;
using Engine.Graphics.OpenGL;
#if DESKTOP
using static OpenGL.GL;
#else
    using static OpenGL.ES.GLES30;
#endif


namespace Engine.Graphics
{
    internal class GLFrameBuffer : GLGfxResource<RenderTargetDescriptor>
    {
        public GLTexture ColorTexture { get; private set; }
        private uint _depthStencilRBO;
        private uint _colorRBO;
        private int _width;
        private int _height;
        public int Width => _width;
        public int Height => _height;
        private RenderTargetDescriptor _descriptor;

        protected internal override GfxResource[] SubResources { get; protected set; }

        public GLFrameBuffer() : base(glGenFramebuffer,
                                    glDeleteFramebuffer,
                                    handle => glBindFramebuffer(GL_FRAMEBUFFER, handle))
        {
        }

        /// <summary>
        /// Blit the low-resolution framebuffer to the screen with nearest-neighbor scaling.
        /// </summary>


        protected override bool CreateResource(RenderTargetDescriptor descriptor)
        {
            _descriptor = descriptor;
            _width = descriptor.Width;
            _height = descriptor.Height;

            Bind();

            if (descriptor.IsMultiSample)
            {
                //ColorTexture = new GLTexture();
                //ColorTexture.Create(new TextureDescriptor()
                //{
                //    Buffer = null,
                //    Width = descriptor.Width,
                //    Height = descriptor.Height,
                //    Channels = 4,
                //    IsMultiSample = true,
                //    SamplesCount = descriptor.SamplesCount
                //});

                //ColorTexture.Bind();
                //SubResources = [ColorTexture];
                // glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D_MULTISAMPLE, ColorTexture.Handle, 0);

                _colorRBO = glGenRenderbuffer();
                glBindRenderbuffer(_colorRBO);
                glRenderbufferStorageMultisample(GL_RENDERBUFFER, descriptor.SamplesCount, GL_RGBA8, _width, _height);
                glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_RENDERBUFFER, _colorRBO);

                // Multisampled Depth and Stencil 
                _depthStencilRBO = glGenRenderbuffer();
                glBindRenderbuffer(_depthStencilRBO);
                glRenderbufferStorageMultisample(GL_RENDERBUFFER, descriptor.SamplesCount, GL_DEPTH24_STENCIL8, _width, _height);
                glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, _depthStencilRBO);
            }
            else
            {
                ColorTexture = new GLTexture();
                ColorTexture.Create(new TextureDescriptor()
                {
                    Buffer = null,
                    Width = descriptor.Width,
                    Height = descriptor.Height,
                    Channels = 4,
                    Filter = descriptor.ColorTextureDescriptor.Filter,
                    Mode = descriptor.ColorTextureDescriptor.Mode,
                });

                ColorTexture.Bind();
                SubResources = [ColorTexture];

                glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, ColorTexture.Handle, 0);

                _depthStencilRBO = glGenRenderbuffer();
                glBindRenderbuffer(_depthStencilRBO);
                glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, descriptor.Width, descriptor.Height);
                glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, _depthStencilRBO);
            }

            if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
            {
                Debug.Error("Framebuffer is not complete!");
                return false;
            }

            Unbind();
            return true;
        }

        internal override void UpdateResource(RenderTargetDescriptor descriptor)
        {
            _descriptor = descriptor;

            ColorTexture?.Dispose();
            glDeleteRenderbuffer(_depthStencilRBO);

            CreateResource(descriptor);
        }

        internal void BlitToScreen(int windowWidth, int windowHeight)
        {
            glBindFramebuffer(GL_FRAMEBUFFER, 0);
            glBindFramebuffer(GL_READ_FRAMEBUFFER, Handle);
            glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
            glBlitFramebuffer(0, 0, _width, _height, 0, 0, windowWidth, windowHeight,
                                 GL_COLOR_BUFFER_BIT, GL_NEAREST);
            glBindFramebuffer(GL_FRAMEBUFFER, 0);
        }

        internal void BlitTo(GLFrameBuffer target, bool color = true, bool depth = false, bool linear = false)
        {
            glBindFramebuffer(GL_READ_FRAMEBUFFER, this.Handle);       // Source
            glBindFramebuffer(GL_DRAW_FRAMEBUFFER, target.Handle);     // Destination

            uint mask = 0;
            if (color) mask |= GL_COLOR_BUFFER_BIT;
            if (depth) mask |= GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT;

            glBlitFramebuffer(
                0, 0, this.Width, this.Height,
                0, 0, target.Width, target.Height,
                mask,
               linear ? GL_LINEAR : GL_NEAREST);

            glBindFramebuffer(GL_FRAMEBUFFER, 0);
            glBindFramebuffer(GL_READ_FRAMEBUFFER, 0);       // Source
            glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);     // Destination

        }

        /// <summary>
        /// Read pixels from the framebuffer (useful for screenshots or pixel effects)
        /// </summary>
        internal byte[] ReadPixels(int x, int y, int width, int height)
        {
            byte[] pixels = new byte[width * height * 4];
            glBindFramebuffer(GL_FRAMEBUFFER, Handle);
            GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);

            // int flippedY = Height - y - height;
            glReadPixels(x, y, width, height, GL_RGBA, GL_UNSIGNED_BYTE, handle.AddrOfPinnedObject());

            handle.Free();
            glBindFramebuffer(GL_FRAMEBUFFER, 0);
            return pixels;
        }
    }
}
