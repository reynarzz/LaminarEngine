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
            fragColor = SampleIndexedTexture(fragTexIndex, fragUV) * vColor;
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

            _mat = new Material(new Shader(_gizmosVert, _gizmosFrag));
            _lineMat = new Material(new Shader(_gizmosLineVert, _gizmosFrag));

            _drawCallData = new DrawCallData()
            {
                Textures = new GfxResource[GfxDeviceManager.Current.GetDeviceInfo().MaxValidTextureUnits],
                Uniforms = new UniformValue[10],
            };

            _pipelineFeatures = new PipelineFeatures();
            // _pipelineFeatures.DepthBuffer = true;
            _pipelineFeatures.Blending = Blending.Transparent;


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
                return new Sprite(new Texture2D(TextureMode.Clamp, image.Width, image.Height, 4, PixelsPerUnit, image.Data));
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

                if (_lineRenderData == null)
                {
                    var transform = new Transform();
                    var pointsTest = new List<vec3>() { new vec3(0, 0, -10), new vec3(0, 5, -10), new vec3(5, 5, -10),
                new vec3(5, -5, -10), new vec3(-5, -5, -10), new vec3(-5, -5, -5), new vec3(-5, -10, -5)};
                    var line = GraphicsHelper.CreateLineMesh3D(pointsTest, 0.1f);

                    _lineRenderData = new RendererData2D(_testLineGuid, transform)
                    {
                        Mesh = new Mesh()
                        {
                            Vertices = line.Vertices.ToList(),
                            Indices = line.Indices,
                            IndicesToDrawCount = line.Indices.Length
                        },
                        Material = _lineMat,
                        PrivateBatch = true
                    };


                    var points = GraphicsHelper.CreatePerspectiveFrustumLines(cameraGame.WorldPosition, cameraGame.Forward, cameraGame.Right, cameraGame.Up,
                                                                 glm.radians(17), cameraGame.Aspect, cameraGame.NearPlane, cameraGame.FarPlane);

                    var mesh = GraphicsHelper.CreateNonContiguousLines(points, 0.2f);

                    _lineRenderData.Mesh.Vertices = mesh.Vertices.ToList();
                    _lineRenderData.Mesh.Indices = mesh.Indices;
                    _lineRenderData.Mesh.IndicesToDrawCount = mesh.Indices.Length;
                    _lineRenderData.IsDirty = true;

                    _renderDatasByType.Add(_testLineGuid, _lineRenderData);
                }
                else
                {
                    var points = default(List<vec3>);

                    if (cameraGame.ProjectionMode == CameraProjectionMode.Perspective)
                    {
                        points = GraphicsHelper.CreatePerspectiveFrustumLines(cameraGame.WorldPosition, cameraGame.Forward, cameraGame.Right, cameraGame.Up,
                                                                              glm.radians(cameraGame.Fov), cameraGame.Aspect, cameraGame.NearPlane, cameraGame.FarPlane);

                    }
                    else
                    {
                        points = GraphicsHelper.CreateOrthoFrustumLines(cameraGame.WorldPosition, cameraGame.Forward, cameraGame.Right, cameraGame.Up,
                                                                        cameraGame.OrthographicSize, cameraGame.Aspect, cameraGame.NearPlane, cameraGame.FarPlane);
                    }

                    var mesh = GraphicsHelper.CreateNonContiguousLines(points, 0.09f);

                    _lineRenderData.Mesh.Vertices = mesh.Vertices.ToList();
                    _lineRenderData.Mesh.Indices = mesh.Indices;
                    _lineRenderData.Mesh.IndicesToDrawCount = mesh.Indices.Length;
                    _lineRenderData.IsDirty = true;
                }
            }
            _batches = _batcher.GetBatches(_renderDatasByType.Values);
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
                    var renderData = renderDatas[component.GetID()];
                    renderData.Transform.WorldPosition = component.Transform.WorldPosition;
                    renderData.IsDirty = true;
                    const int scaling = 100;
                    renderData.SortOrder = -(int)(glm.dot(component.Transform.WorldPosition - camera.WorldPosition, camera.Forward) * scaling);
                }
            }
        }

        public RenderTexture OnRender(ICamera camera, RenderTexture renderTarget)
        {
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
                _drawCallData.RenderTarget = renderTarget.NativeResource;
                _drawCallData.Viewport = new vec4(0, 0, renderTarget.Width, renderTarget.Height);

                var VP = camera.Projection * camera.ViewMatrix;

                // Uniforms
                _drawCallData.Uniforms[0].SetMat4(Consts.VIEW_PROJ_UNIFORM_NAME, VP);
                _drawCallData.Uniforms[1].SetMat4(Consts.VIEW_UNIFORM_NAME, camera.ViewMatrix);
                _drawCallData.Uniforms[2].SetMat4(Consts.PROJECTION_UNIFORM_NAME, camera.Projection);
                _drawCallData.Uniforms[3].SetIntArr(Consts.TEX_ARRAY_UNIFORM_NAME, Batch2D.TextureSlotArray);
                _drawCallData.Uniforms[4].SetMat4(Consts.MODEL_UNIFORM_NAME, batch.WorldMatrix);
                _drawCallData.Uniforms[5].SetVec2(Consts.SCREEN_SIZE_UNIFORM_NAME, new vec2(renderTarget.Width, renderTarget.Height));
                _drawCallData.Uniforms[6].SetVec3(Consts.TIME_UNIFORM_NAME, new vec3(Time.UnscaledTimeWrap, Time.TimeCurrentWrap, Time.DeltaTime));


                GfxDeviceManager.Current.Draw(_drawCallData);
            }
            return renderTarget;
        }

        public void OnEnd()
        {
        }
    }
}
