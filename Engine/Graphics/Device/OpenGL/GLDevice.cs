using Engine.Utils;
using GlmNet;
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
    internal class GLDevice : GfxDevice
    {
        private readonly GfxDeviceInfo _gfxDeviceInfo;
        private static uint _defaultVAO;
        internal static uint DefaultVAO => _defaultVAO;
        public GLDevice()
        {
            int maxTextureUnits;
            int maxTextureUnitsAccessInVertexShader;

            unsafe
            {
                glGetIntegerv(GL_MAX_TEXTURE_IMAGE_UNITS, &maxTextureUnits);
                glGetIntegerv(GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS, &maxTextureUnitsAccessInVertexShader);

                int maxVertexUniforms = 0;
                glGetIntegerv(GL_MAX_VERTEX_UNIFORM_COMPONENTS, &maxVertexUniforms);
                int maxFragmentUniforms = 0;
                glGetIntegerv(GL_MAX_FRAGMENT_UNIFORM_COMPONENTS, &maxFragmentUniforms);

                _gfxDeviceInfo.MaxUniformsCount = (int)MathF.Min(Consts.Graphics.MAX_UNIFORMS_PER_DRAWCALL, MathF.Min(maxFragmentUniforms, maxFragmentUniforms));

                int maxSamples;
                glGetIntegerv(GL_MAX_SAMPLES, &maxSamples);
                _gfxDeviceInfo.MaxSamples = maxSamples;

                //int maxUniformBlocks = 0;
                //glGetIntegerv(GL_MAX_UNIFORM_BUFFER_BINDINGS, &maxUniformBlocks);
            }

            _gfxDeviceInfo.MaxHardwareTextureUnits = maxTextureUnits;
            _gfxDeviceInfo.MaxTexAccessInVertexShader = maxTextureUnitsAccessInVertexShader;

            _gfxDeviceInfo.Vendor = glGetString(GL_VENDOR);
            _gfxDeviceInfo.Device = glGetString(GL_RENDERER);
            _gfxDeviceInfo.Version = glGetString(GL_VERSION);

            Debug.Log("OpenGL Version: " + _gfxDeviceInfo.Version);
            Debug.Log("OpenGL Device: " + _gfxDeviceInfo.Device);
            Debug.Log("OpenGL Max Shader Texture Units: " + _gfxDeviceInfo.MaxShaderTextureUnits);
            Debug.Log("OpenGL Max Hardware Texture Units: " + _gfxDeviceInfo.MaxHardwareTextureUnits);
            Debug.Log("OpenGL MaxUniformsCount: " + _gfxDeviceInfo.MaxUniformsCount);

            _defaultVAO = glGenVertexArray();
            GLHelpers.CheckGLError(GetType().Name);

            glBindVertexArray(_defaultVAO);
            GLHelpers.CheckGLError(GetType().Name);

        }

        internal override void Initialize() { }
        internal override void Close() { }

        internal override void Clear(ClearDeviceConfig config)
        {
            var target = config.RenderTarget as GLFrameBuffer;

            if (target != null)
            {
                target.Bind();
            }
            glClearColor(config.Color.R, config.Color.G, config.Color.B, config.Color.A);
            GLHelpers.CheckGLError(GetType().Name);

            glClearStencil(0);
            GLHelpers.CheckGLError(GetType().Name);

            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);
            GLHelpers.CheckGLError(GetType().Name);

            //if (target != null)
            //    glViewport(0, 0, target.Width, target.Height);

            if (target != null)
            {
                target.Unbind();
            }
        }

        internal override GfxResource CreateGeometry(GeometryDescriptor desc)
        {
            GLGeometry geometry = new GLGeometry();
            geometry.Create(desc);
            GLHelpers.CheckGLError(GetType().Name);

            return geometry;
        }

        internal override GfxResource CreateVertexBuffer(VertexDataDescriptor desc)
        {
            // TODO: Also create the vao here
            var vertexBuffer = new GLVertexBuffer();
            vertexBuffer.Create(desc.BufferDesc);
            GLHelpers.CheckGLError(GetType().Name);

            return vertexBuffer;
        }

        internal override GfxResource CreateIndexBuffer(BufferDataDescriptor desc)
        {
            var indexBuffer = new GLIndexBuffer();
            indexBuffer.Create(desc);
            GLHelpers.CheckGLError(GetType().Name);

            return indexBuffer;
        }

        internal override GfxResource CreateShader(ShaderDescriptor desc)
        {
            glBindVertexArray(DefaultVAO);
            GLHelpers.CheckGLError(GetType().Name);

            var shader = new GLShader();
            shader.Create(desc);
            GLHelpers.CheckGLError(GetType().Name);

            return shader;
        }

        internal override GfxResource CreateTexture(TextureDescriptor desc)
        {
            var texture = new GLTexture();
            texture.Create(desc);
            GLHelpers.CheckGLError(GetType().Name);

            return texture;
        }

        internal override GfxResource CreateRenderTarget(RenderTargetDescriptor desc)
        {
            var fbuffer = new GLFrameBuffer();
            fbuffer.Create(desc);
            GLHelpers.CheckGLError(GetType().Name);

            return fbuffer;
        }

        private void DrawIndexed(DrawMode mode, int indicesLength)
        {
            unsafe
            {
                glDrawElements(GetGLDrawMode(mode), indicesLength, GL_UNSIGNED_INT, null);
                GLHelpers.CheckGLError(GetType().Name);

            }
        }

        internal override void DrawArrays(DrawMode mode, int startIndex, int vertexCount)
        {
            glDrawArrays(GetGLDrawMode(mode), startIndex, vertexCount);
            GLHelpers.CheckGLError(GetType().Name);

        }

        private int GetGLDrawMode(DrawMode mode)
        {
            var internalMode = mode switch
            {
                DrawMode.Triangles => GL_TRIANGLES,
                DrawMode.Lines => GL_LINES,
                DrawMode.Points => GL_POINTS,
                DrawMode.LineStrips => GL_LINE_STRIP,
                _ => -1
            };

            if (internalMode == -1)
            {
                throw new NotImplementedException($"Draw mode unsupported: {mode}");
            }

            return internalMode;
        }

        internal override void UpdateResource(GfxResource resource, IGfxResourceDescriptor desc)
        {
            if (resource as GLGeometry != null)
            {
                (resource as GLGeometry).UpdateResource(desc as GeometryDescriptor);
                GLHelpers.CheckGLError(GetType().Name);

            }
            else if (resource is GLFrameBuffer buffer)
            {
                buffer.UpdateResource(desc as RenderTargetDescriptor);
                GLHelpers.CheckGLError(GetType().Name);

            }
            else if (resource is GLTexture texture)
            {
                texture.UpdateResource(desc as TextureDescriptor);
                GLHelpers.CheckGLError(GetType().Name);

            }
        }

        internal override GfxDeviceInfo GetDeviceInfo()
        {
            return _gfxDeviceInfo;
        }

        private ivec4 _cachedViewport;
        internal override void SetViewport(vec4 viewport)
        {
            var x = (int)viewport.x;
            var y = (int)viewport.y;
            var w = (int)viewport.z;
            var h = (int)viewport.w;

            if (_cachedViewport.x != x || _cachedViewport.y != y || _cachedViewport.z != w || _cachedViewport.w != h)
            {
                _cachedViewport = new ivec4(x, y, w, h);
                glViewport(x, y, w, h);
                GLHelpers.CheckGLError(GetType().Name);
            }
        }

        internal override void Present()
        {
            WindowManager.Window.SwapBuffers();
            GLHelpers.CheckGLError(GetType().Name);
        }

        internal override void Present(GfxResource renderTarget)
        {
            var frameBuffer = renderTarget as GLFrameBuffer;
            if (frameBuffer != null)
            {
                frameBuffer.BlitToScreen(Screen.Width, Screen.Height);
                GLHelpers.CheckGLError(GetType().Name);

            }

            Present();
        }

        private PipelineFeatures _cachedPipFeatures = new();

        private void SetPipelineFeatures(PipelineFeatures features)
        {
            var isStencilEnabled = features.Stencil.Enabled;
            var isBlendingEnabled = features.Blending.Enabled;
            var isDepthTestEnabled = features.DepthTesting;

            if (isBlendingEnabled != _cachedPipFeatures.Blending.Enabled)
            {
                _cachedPipFeatures.Blending.Enabled = isBlendingEnabled;
                if (features.Blending.Enabled)
                {
                    glEnable(GL_BLEND);
                    GLHelpers.CheckGLError(GetType().Name);

                    glBlendFunc(GLHelpers.MapBlendFactor(features.Blending.SrcFactor), GLHelpers.MapBlendFactor(features.Blending.DstFactor));
                    GLHelpers.CheckGLError(GetType().Name);

                    glBlendEquation(GLHelpers.MapBlendEquation(features.Blending.Equation));
                    GLHelpers.CheckGLError(GetType().Name);

                }
                else
                {
                    glDisable(GL_BLEND);
                    GLHelpers.CheckGLError(GetType().Name);

                }
            }

            if (isStencilEnabled != _cachedPipFeatures.Stencil.Enabled)
            {
                _cachedPipFeatures.Stencil.Enabled = isStencilEnabled;
                if (isStencilEnabled)
                {
                    glEnable(GL_STENCIL_TEST);
                    GLHelpers.CheckGLError(GetType().Name);

                    glStencilMask(0xFF);
                    GLHelpers.CheckGLError(GetType().Name);

                    glStencilFunc(GLHelpers.MapFunc(features.Stencil.Func), features.Stencil.Ref, features.Stencil.Mask);
                    GLHelpers.CheckGLError(GetType().Name);

                    glStencilOp(GLHelpers.MapOp(features.Stencil.FailOp), GLHelpers.MapOp(features.Stencil.ZFailOp), GLHelpers.MapOp(features.Stencil.ZPassOp));
                    GLHelpers.CheckGLError(GetType().Name);

                }
                else
                {
                    glDisable(GL_STENCIL_TEST);
                    GLHelpers.CheckGLError(GetType().Name);

                }
            }

            if (isDepthTestEnabled != _cachedPipFeatures.DepthTesting)
            {
                _cachedPipFeatures.DepthTesting = isDepthTestEnabled;
                if (features.DepthTesting)
                {
                    glEnable(GL_DEPTH_TEST);
                    GLHelpers.CheckGLError(GetType().Name);

                }
                else
                {
                    glDisable(GL_DEPTH_TEST);
                    GLHelpers.CheckGLError(GetType().Name);

                }
            }
        }

        internal override void Draw(Action draw, GfxResource renderTarget)
        {
            var frameBuffer = renderTarget as GLFrameBuffer;
            frameBuffer?.Bind();
            GLHelpers.CheckGLError(GetType().Name);

            draw?.Invoke();
            GLHelpers.CheckGLError(GetType().Name);

            frameBuffer?.Unbind();
            GLHelpers.CheckGLError(GetType().Name);

        }
        internal override void Draw(DrawCallData drawCallData)
        {
            (drawCallData.Geometry as GLGeometry).Bind();
            GLHelpers.CheckGLError(GetType().Name);

            var shader = drawCallData.Shader as GLShader;
            shader.Bind();
            GLHelpers.CheckGLError(GetType().Name);

            int textureIndex = 0;
            for (; textureIndex < drawCallData.Textures.Length; textureIndex++)
            {
                var tex = drawCallData.Textures[textureIndex];
                if (tex == null)
                    break;
                (tex as GLTexture).Bind(textureIndex);
                GLHelpers.CheckGLError(GetType().Name);

            }

            foreach (var (uniform, texture) in drawCallData.NamedTextures)
            {
                shader.SetUniform(uniform, textureIndex);
                GLHelpers.CheckGLError(GetType().Name);

                (texture as GLTexture).Bind(textureIndex);
                GLHelpers.CheckGLError(GetType().Name);

                textureIndex++;
            }

            var renderTarget = drawCallData.RenderTarget as GLFrameBuffer;

            if (renderTarget != null)
            {
                renderTarget.Bind();
                GLHelpers.CheckGLError(GetType().Name);
            }

            SetViewport(drawCallData.Viewport);

            GLHelpers.SetUniforms(shader, drawCallData.Uniforms);
            SetPipelineFeatures(drawCallData.Features);

            switch (drawCallData.DrawType)
            {
                case DrawType.Indexed:
                    DrawIndexed(drawCallData.DrawMode, drawCallData.IndexedDraw.IndexCount);
                    break;
                case DrawType.Arrays:
                    DrawArrays(drawCallData.DrawMode, drawCallData.ArraysDraw.StartIndex, drawCallData.ArraysDraw.VertexCount);
                    break;
                default:
                    Debug.Error($"Draw type: '{drawCallData.DrawType}' is not implemented.");
                    break;
            }

            if (renderTarget != null)
            {
                renderTarget.Unbind();
                GLHelpers.CheckGLError(GetType().Name);
            }
            shader.Unbind();
            GLHelpers.CheckGLError(GetType().Name);
            (drawCallData.Geometry as GLGeometry).Unbind();
            GLHelpers.CheckGLError(GetType().Name);
        }

        internal override byte[] ReadRenderTargetColors(GfxResource nativeResource, int x, int y, int width, int height)
        {
            if (nativeResource is GLFrameBuffer buffer)
            {
                var buff = buffer.ReadPixels(x, y, width, height);
                GLHelpers.CheckGLError(GetType().Name);

                return buff;
            }

            return null;
        }

        internal override void BlitRenderTargetTo(GfxResource source, GfxResource target, bool color = true, bool depth = false, bool linear = false)
        {
            var sourceFB = (source as GLFrameBuffer);
            sourceFB.BlitTo(target as GLFrameBuffer, color, depth, linear);
            GLHelpers.CheckGLError(GetType().Name);
        }

        internal override void DestroyResource(GfxResource resource)
        {
            if (resource != null && resource.IsInitialized)
            {
                resource.Dispose();
                GLHelpers.CheckGLError(GetType().Name);

            }
        }

        internal override bool IsResourceValid(GfxResource nativeResource)
        {
            return nativeResource.IsInitialized;
        }
    }
}
