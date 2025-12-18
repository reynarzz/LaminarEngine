using System;
using System.Runtime.InteropServices;
using Engine.Graphics.OpenGL;
using OpenGL;

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
        private uint DepthStencilRBO;
        private int _width;
        private int _height;
        public int Width => _width;
        public int Height => _height;
        protected internal override GfxResource[] SubResources { get; protected set; }

        public GLFrameBuffer() : base(glGenFramebuffer,
                                    glDeleteFramebuffer,
                                    handle => glBindFramebuffer(GL_FRAMEBUFFER, handle))
        {
        }

        /// <summary>
        /// Blit the low-resolution framebuffer to the screen with nearest-neighbor scaling.
        /// </summary>
        internal void BlitToScreen(int windowWidth, int windowHeight)
        {
            glBindFramebuffer(GL_READ_FRAMEBUFFER, Handle);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            glBindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            glBlitFramebuffer(0, 0, _width, _height, 0, 0, windowWidth, windowHeight,
                                 GL_COLOR_BUFFER_BIT, GL_NEAREST);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            glBindFramebuffer(GL_FRAMEBUFFER, 0);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
        }

        internal void BlitTo(GLFrameBuffer target, bool color = true, bool depth = false)
        {
            glBindFramebuffer(GL_READ_FRAMEBUFFER, this.Handle);       // Source
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            glBindFramebuffer(GL_DRAW_FRAMEBUFFER, target.Handle);     // Destination
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            uint mask = 0;
            if (color) mask |= GL_COLOR_BUFFER_BIT;
            if (depth) mask |= GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT;

            glBlitFramebuffer(
                0, 0, this.Width, this.Height,
                0, 0, target.Width, target.Height,
                mask,
                GL_NEAREST);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            glBindFramebuffer(GL_FRAMEBUFFER, 0);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
        }

        /// <summary>
        /// Read pixels from the framebuffer (useful for screenshots or pixel effects)
        /// </summary>
        internal byte[] ReadPixels()
        {
            byte[] pixels = new byte[Width * Height * 4]; // RGBA8
            glBindFramebuffer(GL_FRAMEBUFFER, Handle);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            glReadPixels(0, 0, Width, Height, GL_RGBA, GL_UNSIGNED_BYTE, handle.AddrOfPinnedObject());
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            handle.Free();
            glBindFramebuffer(GL_FRAMEBUFFER, 0);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            return pixels;
        }

        protected override bool CreateResource(RenderTargetDescriptor descriptor)
        {
            _width = descriptor.Width;
            _height = descriptor.Height;
            Debug.Log($"FB, width: {_width}, height:{_height}");
            Bind();
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            ColorTexture = new GLTexture();
            ColorTexture.Create(new TextureDescriptor()
            {
                Buffer = null,
                Width = descriptor.Width,
                Height = descriptor.Height,
                Channels = 4
            });

            ColorTexture.Bind();
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            SubResources =
            [
                ColorTexture
            ];

            glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, ColorTexture.Handle, 0);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            // Depth + stencil
            DepthStencilRBO = glGenRenderbuffer();
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            glBindRenderbuffer(DepthStencilRBO);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, descriptor.Width, descriptor.Height);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, DepthStencilRBO);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
            {
                Debug.Error("Framebuffer is not complete!");
                return false;
            }
            else
            {

                Debug.Log("Frame buffer success");
            }
            Unbind();
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            return true;
        }

        internal override void UpdateResource(RenderTargetDescriptor descriptor)
        {
            ColorTexture.Dispose();
            glDeleteRenderbuffer(DepthStencilRBO);
#if DEBUG
            GLUtils.PrintGLErrors();
#endif
            CreateResource(descriptor);
        }
    }
}
