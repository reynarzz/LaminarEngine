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
        private static uint DefaultFrameBuffer = 0;
        public GLTexture ColorTexture { get; private set; }
        private uint _depthStencilRBO;
        private uint _colorRBO;
        private int _width;
        private int _height;
        public int Width => _width;
        public int Height => _height;
        private RenderTargetDescriptor _descriptor;

        protected internal override GfxResource[] SubResources { get; protected set; }

        private static uint _boundFrameBuffer = uint.MaxValue;
        private static uint _boundReadFrameBuffer = uint.MaxValue;
        private static uint _boundDrawFrameBuffer = uint.MaxValue;

        public GLFrameBuffer() : base(glGenFramebuffer,
            glDeleteFramebuffer,
            handle => BindFrameBufferCached(handle))
        {
        }

        private static void BindFrameBufferCached(uint handle)
        {
            if (handle == 0)
                handle = DefaultFrameBuffer;

            if (_boundFrameBuffer == handle)
            {
                return;
            }
            
            glBindFramebuffer(GL_FRAMEBUFFER, handle);
            _boundFrameBuffer = handle;
        }

        private static void BindReadFrameBufferCached(uint handle)
        {
            if (handle == 0)
                handle = DefaultFrameBuffer;
            
            if (_boundReadFrameBuffer == handle)
            {
                return;
            }

            glBindFramebuffer(GL_READ_FRAMEBUFFER, handle);
            _boundReadFrameBuffer = handle;
        }

        private static void BindDrawFrameBufferCached(uint handle)
        {
            if (handle == 0)
                handle = DefaultFrameBuffer;
            
            if (_boundDrawFrameBuffer == handle)
            {
                return;
            }

            glBindFramebuffer(GL_DRAW_FRAMEBUFFER, handle);
            _boundDrawFrameBuffer = handle;
        }

        protected override bool CreateResource(RenderTargetDescriptor descriptor)
        {
            _descriptor = descriptor;
            _width = descriptor.Width;
            _height = descriptor.Height;

            Bind();

            if (descriptor.IsMultiSample)
            {
                _colorRBO = glGenRenderbuffer();
                GLHelpers.CheckGLError();
                glBindRenderbuffer(_colorRBO);
                GLHelpers.CheckGLError();

                glRenderbufferStorageMultisample(GL_RENDERBUFFER, descriptor.SamplesCount, GL_RGBA8, _width, _height);
                GLHelpers.CheckGLError();

                glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_RENDERBUFFER, _colorRBO);
                GLHelpers.CheckGLError();

                _depthStencilRBO = glGenRenderbuffer();
                GLHelpers.CheckGLError();

                glBindRenderbuffer(_depthStencilRBO);
                GLHelpers.CheckGLError();

                glRenderbufferStorageMultisample(GL_RENDERBUFFER, descriptor.SamplesCount, GL_DEPTH24_STENCIL8, _width, _height);
                GLHelpers.CheckGLError();

                glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, _depthStencilRBO);
                GLHelpers.CheckGLError();

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
                GLHelpers.CheckGLError();

                _depthStencilRBO = glGenRenderbuffer();
                GLHelpers.CheckGLError();

                glBindRenderbuffer(_depthStencilRBO);
                GLHelpers.CheckGLError();

                glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, descriptor.Width, descriptor.Height);
                GLHelpers.CheckGLError();

                glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, _depthStencilRBO);
                GLHelpers.CheckGLError();

            }

            if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
            {
                Debug.Error("Framebuffer is not complete!");
                return false;
            }
            
            return true;
        }

        internal override void UpdateResource(RenderTargetDescriptor descriptor)
        {
            _descriptor = descriptor;

            ColorTexture?.Dispose();
            glDeleteRenderbuffer(_depthStencilRBO);
            GLHelpers.CheckGLError();

            CreateResource(descriptor);
            GLHelpers.CheckGLError();
        }

        internal void BlitToScreen(int windowWidth, int windowHeight)
        {
            BindFrameBufferCached(DefaultFrameBuffer);
            GLHelpers.CheckGLError();

            BindReadFrameBufferCached(Handle);
            GLHelpers.CheckGLError();

            BindDrawFrameBufferCached(DefaultFrameBuffer);
            GLHelpers.CheckGLError();

            glBlitFramebuffer(0, 0, _width, _height, 0, 0, windowWidth, windowHeight,
                                 GL_COLOR_BUFFER_BIT, GL_NEAREST);
            GLHelpers.CheckGLError();
            
            BindFrameBufferCached(DefaultFrameBuffer);
        }

        internal void BlitTo(GLFrameBuffer target, bool color = true, bool depth = false, bool linear = false)
        {
            BindReadFrameBufferCached(this.Handle);
            GLHelpers.CheckGLError();

            BindDrawFrameBufferCached(target.Handle);
            GLHelpers.CheckGLError();

            uint mask = 0;
            if (color) mask |= GL_COLOR_BUFFER_BIT;
            if (depth) mask |= GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT;

            glBlitFramebuffer(0, 0, this.Width, this.Height, 0, 0, target.Width, target.Height,
                              mask, linear ? GL_LINEAR : GL_NEAREST);
            GLHelpers.CheckGLError();

            BindFrameBufferCached(DefaultFrameBuffer);
            GLHelpers.CheckGLError();

            BindReadFrameBufferCached(DefaultFrameBuffer);
            GLHelpers.CheckGLError();

            BindDrawFrameBufferCached(DefaultFrameBuffer);
            GLHelpers.CheckGLError();
        }

        internal unsafe byte[] ReadPixels(int x, int y, int width, int height)
        {
            byte[] pixels = new byte[width * height * 4];

            BindFrameBufferCached(Handle);
            GLHelpers.CheckGLError();

            fixed (byte* ptr = pixels)
            {
                glReadPixels(x, y, width, height, GL_RGBA, GL_UNSIGNED_BYTE, (IntPtr)ptr);
                GLHelpers.CheckGLError();

            }

            BindFrameBufferCached(DefaultFrameBuffer);
            GLHelpers.CheckGLError();

            return pixels;
        }

        internal override void Unbind()
        {
            BindFrameBufferCached(DefaultFrameBuffer);
        }

        internal static unsafe void SyncDefaultFrameBuffer()
        {
            int fbo = 0;
            glGetIntegerv(GL_FRAMEBUFFER_BINDING,  &fbo);
            DefaultFrameBuffer = (uint)fbo;
            
            _boundFrameBuffer = uint.MaxValue;
            _boundReadFrameBuffer = uint.MaxValue;
            _boundDrawFrameBuffer = uint.MaxValue;
        }
    }
}