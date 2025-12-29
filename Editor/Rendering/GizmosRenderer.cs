using Engine;
using Engine.Graphics;
using Engine.Graphics.Client;
using Engine.Rendering;
using Engine.Utils;
using GlmNet;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Rendering
{
    // TODO: remove from dictionary when the component is destroyed.
    internal class GizmosRenderer : IGizmosRenderer
    {
        private readonly Batcher2D _batcher;
        private readonly DrawCallData _drawCallData;
        private readonly Dictionary<Guid, RendererData2D> _renderDatasByType;
        private List<Batch2D> _batches;
        private PipelineFeatures _pipelineFeatures;
        private RenderTexture _renderTexture;
        private readonly Color SemiTransparent = new Color(1, 1, 1, 0.7f);
        private Shader _gizmosShader;
        public int PixelsPerUnit { get; set; } = 64;
        private Material _mat;
        private enum GizmoType
        {
            Camera,
        }

        string _gizmosVert = @"
        #version 330 core
        layout(location = 0) in vec3 position;
        layout(location = 1) in vec2 uv;
        layout(location = 2) in uint color; 
        layout(location = 3) in int texIndex; 
        layout(location = 5) in vec3 worldCenter; 

        out vec2 fragUV;
        flat out int fragTexIndex;
        out vec4 vColor;
        out vec2 worldUV;

        uniform mat4 uVP;
        uniform mat4 uViewMatrix;

        vec4 unpackColor(uint c) 
        {
            float r = float((c >> 24) & 0xFFu) / 255.0;
            float g = float((c >> 16) & 0xFFu) / 255.0;
            float b = float((c >>  8) & 0xFFu) / 255.0;
            float a = float( c        & 0xFFu) / 255.0;
            return vec4(r,g,b,a);
        }

        void main() 
        {
            fragUV = uv;
            fragTexIndex = texIndex;
            vColor = unpackColor(color);

            // inverse camera rotation
            mat3 invViewRot = transpose(mat3(uViewMatrix));

            // rotate around quad center
            vec3 localPos = position - worldCenter;
            vec3 billboardPos = worldCenter + invViewRot * localPos;

            worldUV = billboardPos.xy * 0.1;

            gl_Position = uVP * vec4(billboardPos, 1.0);
        }
";
        string _gizmosFrag = @"
        #version 330 core

        uniform sampler2D uTextures[15]; //uniform sampler2D uTextures[{32}]
        in vec2 fragUV;
        in vec4 vColor;

        flat in int fragTexIndex;
        out vec4 fragColor;

        vec4 SampleIndexedTexture(int index, vec2 uv)
        {
            switch(index)
            {
                case 0:  return texture(uTextures[0], uv);
                case 1:  return texture(uTextures[1], uv);
                case 2:  return texture(uTextures[2], uv);
                case 3:  return texture(uTextures[3], uv);
                case 4:  return texture(uTextures[4], uv);
                case 5:  return texture(uTextures[5], uv);
                case 6:  return texture(uTextures[6], uv);
                case 7:  return texture(uTextures[7], uv);
                case 8:  return texture(uTextures[8], uv);
                case 9:  return texture(uTextures[9], uv);
                case 10: return texture(uTextures[10], uv);
                case 11: return texture(uTextures[11], uv);
                case 12: return texture(uTextures[12], uv);
                case 13: return texture(uTextures[13], uv);
                case 14: return texture(uTextures[14], uv);
            }

            // fallback color if out of range
            return vec4(1.0, 0.0, 1.0, 1.0); 
        }

        void main()
        {
            vec4 c = SampleIndexedTexture(fragTexIndex, fragUV) * vColor;
            c.rgb *= c.a;

            fragColor = c;
        }
";
        string _gizmosLineVert = @"
  #version 330 core
  layout(location = 0) in vec3 position;
  layout(location = 1) in vec2 uv;
  layout(location = 2) in uint color; 
  layout(location = 3) in int texIndex; 
  
  out vec2 fragUV;
  flat out int fragTexIndex;            // flat = no interpolation between vertices
  out vec4 vColor;
  uniform mat4 uVP;
  out vec2 worldUV;

  vec4 unpackColor(uint c) 
  {
      float r = float((c >> 24) & 0xFFu) / 255.0;
      float g = float((c >> 16) & 0xFFu) / 255.0;
      float b = float((c >>  8) & 0xFFu) / 255.0;
      float a = float( c        & 0xFFu) / 255.0;
      return vec4(r,g,b,a);
  }
  
  void main() 
  {
      fragUV = uv;
      worldUV = position.xy * 0.1;
      fragTexIndex = texIndex; 
      vColor = unpackColor(color);
      gl_Position = uVP * vec4(position, 1.0);
  }
";

        private Material _lineMat;
        public GizmosRenderer()
        {
            _batcher = new Batcher2D(Consts.Graphics.MAX_QUADS_PER_BATCH);
            _renderDatasByType = new Dictionary<Guid, RendererData2D>();

            _renderTexture = new RenderTexture(1920, 1080, TextureFilter.Linear, true,
                Math.Min(GfxDeviceManager.Current.GetDeviceInfo().MaxSamples, 4));
            // _renderTexture = new RenderTexture(1920, 1080, TextureFilter.Linear, true);

            _mat = new Material(new Shader(_gizmosVert, _gizmosFrag));
            _lineMat = new Material(new Shader(_gizmosLineVert, _gizmosFrag));

            _drawCallData = new DrawCallData()
            {
                Textures = new GfxResource[GfxDeviceManager.Current.GetDeviceInfo().MaxValidTextureUnits],
                Uniforms = new UniformValue[10],
            };

            _pipelineFeatures = new PipelineFeatures();
            _pipelineFeatures.DepthTesting = false;
            _pipelineFeatures.Blending = Blending.Transparent;
            _pipelineFeatures.Blending.SrcFactor = BlendFactor.One;

            InitIcons();
        }

        private Sprite _cameraSprite;
        private Sprite _audioSprite;

        private void InitIcons()
        {
            Sprite LoadSprite(string pathInResources)
            {
                StbImage.stbi_set_flip_vertically_on_load(1);
                var image = ImageResult.FromStream(File.OpenRead(Path.Combine(EditorPaths.DataRoot, "Resources", pathInResources)));
                return new Sprite(new Texture2D(TextureMode.Clamp, TextureFilter.Linear, image.Width, image.Height, 4, PixelsPerUnit, image.Data));
            }

            _cameraSprite = LoadSprite("cameraIcon3.png");
            _audioSprite = LoadSprite("audioIcon2.png");
        }

        private static Guid _testLineGuid = Guid.NewGuid();

        private static RendererData2D _lineRenderData;

        public void OnBegin(ICamera camera)
        {
            // TODO:This is slow, it creates a new instance every frame,
            var cameras = SceneManager.FindAll<Camera>(false);
            var audio = SceneManager.FindAll<AudioSource>(false);

            GetRenderData(cameras, _renderDatasByType, _cameraSprite, camera);
            GetRenderData(audio, _renderDatasByType, _audioSprite, camera);

            if (cameras.Count > 0)
            {
                var cameraGame = cameras.ElementAt(0);
                CameraFrustum(cameraGame);
            }

            DrawSelected();
            _batches = _batcher.GetBatches(_renderDatasByType.Values);
        }

        private void DrawSelected()
        {
            if (Selector.SelectedTransform())
            {
                var renderer = Selector.SelectedTransform().GetComponent<Renderer>();

                //if(renderer is TilemapRenderer tilemap)
                //{
                //    var lines = GraphicsHelper.CreateGrid((int)tilemap.GridSize.x, (int)tilemap.GridSize.y, 16);

                //    for (int i = 0; i < lines.Count-1; i+= 2)
                //    {
                //        Debug.DrawLine(lines[0], lines[1], Color.White);
                //    }
                //}
                //else
                if (renderer)
                {
                    var s = MathF.Sin(Time.UnscaledTime * 10) * 0.5f + 0.5f;
                    var size = renderer.RendererData.Bounds.Size;// + new vec3(s, s) * 0.5f;
                    Debug.DrawBox(Selector.SelectedTransform().WorldPosition + renderer.RendererData.Bounds.Center, size, SemiTransparent);

                }
            }
        }

        private void CameraFrustum(Camera camera)
        {
            var points = default(List<vec3>);

            if (camera.ProjectionMode == CameraProjectionMode.Perspective)
            {
                points = GraphicsHelper.CreatePerspectiveFrustumLines(camera.WorldPosition, camera.Forward, camera.Right, camera.Up,
                                                                      glm.radians(camera.Fov), camera.Aspect, camera.NearPlane, camera.FarPlane);

            }
            else
            {
                points = GraphicsHelper.CreateOrthoFrustumLines(camera.WorldPosition, camera.Forward, camera.Right, camera.Up,
                                                                camera.OrthographicSize * 2.0f, camera.Aspect, camera.NearPlane, camera.FarPlane);
            }

            for (int i = 0; i < points.Count - 1; i += 2)
            {
                Debug.DrawLine(points[i], points[i + 1], SemiTransparent);
            }
        }

        private void GetRenderData<T>(List<T> components, Dictionary<Guid, RendererData2D> renderDatas,
            Sprite sprite, ICamera camera) where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                var component = components[i];
                if (!renderDatas.ContainsKey(component.GetID()))
                {
                    renderDatas.Add(component.GetID(), new RendererData2D(component.GetID(), new Transform())
                    {
                        IsBillboard = true,
                        SortOrder = 20,
                        Sprite = sprite,
                        Material = _mat
                    });
                }
                else
                {
                    var translation = component.Transform.GetRenderingWorldMatrix()[3];
                    var renderData = renderDatas[component.GetID()];
                    renderData.Transform.WorldPosition = new vec3(translation);
                    renderData.IsDirty = true;
                    const int scaling = 100;
                    renderData.SortOrder = -(int)(glm.dot(component.Transform.WorldPosition - camera.WorldPosition, camera.Forward) * scaling);
                }
            }
        }

        public RenderTexture OnRender(ICamera camera, RenderingSurface surface, RenderTexture renderTarget)
        {
            GfxDeviceManager.Current.BlitRenderTargetTo(renderTarget.NativeResource, _renderTexture.NativeResource);

            Debug.DrawGeometries(camera.Projection * camera.ViewMatrix, surface.UIViewProj, renderTarget.NativeResource);

            foreach (var batch in _batches)
            {
                if (!batch.IsActive)
                {
                    break;
                }

                batch.Flush();

                int boundTex = 0;
                for (; boundTex < batch.Textures.Length; boundTex++)
                {
                    var tex = batch.Textures[boundTex];

                    if (tex == null)
                        break;

                    _drawCallData.Textures[boundTex] = tex.NativeResource;
                }

                // Set material's texture
                foreach (var (uniformName, texture) in batch.Material.Textures)
                {
                    _drawCallData.NamedTextures[uniformName] = texture.NativeResource;
                }

                // _drawCallData.Textures[screenGrabIndex] = pass.IsScreenGrabPass ? screenGrabTarget.NativeResource.SubResources[0] : null;

                // Pipeline
                //_pipelineFeatures.Blending = pass.Blending;
                //_pipelineFeatures.Stencil = pass.Stencil;

                _drawCallData.DrawType = batch.DrawType;
                _drawCallData.DrawMode = batch.DrawMode;
                _drawCallData.IndexedDraw.IndexCount = batch.IndexCount;
                _drawCallData.Shader = batch.Material.Shader.NativeShader;
                _drawCallData.Geometry = batch.Geometry;
                _drawCallData.Features = _pipelineFeatures;
                _drawCallData.RenderTarget = _renderTexture.NativeResource;
                _drawCallData.Viewport = new vec4(0, 0, _renderTexture.Width, _renderTexture.Height);

                var VP = camera.Projection * camera.ViewMatrix;

                // Uniforms
                _drawCallData.Uniforms[0].SetMat4(Consts.VIEW_PROJ_UNIFORM_NAME, VP);
                _drawCallData.Uniforms[1].SetMat4(Consts.VIEW_UNIFORM_NAME, camera.ViewMatrix);
                _drawCallData.Uniforms[2].SetMat4(Consts.PROJECTION_UNIFORM_NAME, camera.Projection);
                _drawCallData.Uniforms[3].SetIntArr(Consts.TEX_ARRAY_UNIFORM_NAME, Batch2D.TextureSlotArray);
                _drawCallData.Uniforms[4].SetMat4(Consts.MODEL_UNIFORM_NAME, batch.WorldMatrix);
                _drawCallData.Uniforms[5].SetVec2(Consts.SCREEN_SIZE_UNIFORM_NAME, new vec2(_renderTexture.Width, _renderTexture.Height));
                _drawCallData.Uniforms[6].SetVec3(Consts.TIME_UNIFORM_NAME, new vec3(Time.UnscaledTimeWrap, Time.TimeCurrentWrap, Time.DeltaTime));


                GfxDeviceManager.Current.Draw(_drawCallData);
            }

            GfxDeviceManager.Current.BlitRenderTargetTo(_renderTexture.NativeResource, renderTarget.NativeResource);
            return renderTarget;
        }

        public void OnEnd()
        {
        }
    }
}
