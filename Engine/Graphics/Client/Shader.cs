using Engine.Graphics;
using Engine.IO;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine
{
    public class Shader : Asset
    {
        [ShowFieldNoSerialize(isReadOnly: true)] private ShaderUniform[] _uniforms;
        private ShaderSource[] _sources;
        private GfxResource _nativeShader;
        internal GfxResource NativeShader
        {
            get
            {
                if (_nativeShader == null && !HasErrors)
                {
                    UploadShaderSources(_sources);
                }

                return _nativeShader;
            }
        }

        internal IReadOnlyList<ShaderUniform> Uniforms => _uniforms;
        internal IReadOnlyList<ShaderSource> ShaderSources => _sources;
        internal bool HasErrors { get; private set; }
        internal Shader(ShaderSource[] sources, Guid guid) : base(guid)
        {
            Initialize(sources);
        }

        private bool Initialize(ShaderSource[] sources)
        {
            _sources = sources;
            HasErrors = sources == null || sources.Length < 2 || (sources?.Any(x => x?.HasErrors ?? true) ?? true);

            if (!HasErrors)
            {
                _uniforms = sources.SelectMany(x => x.Uniforms).DistinctBy(x => x.Name).ToArray();

                return true;
            }

            return false;
        }

        [Obsolete("Load shaders with Assets.GetShader(\\\"pathToShader.slang\\\")")]
        public Shader(string vertexCode, string fragmentCode) : this(vertexCode, fragmentCode, string.Empty, string.Empty)
        {

        }

        [Obsolete]
        public Shader(string vertexCode, string fragmentCode, string vertName, string fragName) : base(Guid.NewGuid())
        {
            UploadShader(Encoding.UTF8.GetBytes(vertexCode), Encoding.UTF8.GetBytes(fragmentCode));
        }

        protected override void OnUpdateResource(object data, Guid guid)
        {
            var sources = data as ShaderSource[];
            if (Initialize(sources))
            {
                UploadShaderSources(sources);
            }
            else
            {
                DestroyNativeShader();
            }
        }

        private void UploadShaderSources(ShaderSource[] sources)
        {
            if (!HasErrors)
            {
                var vertex = sources.FirstOrDefault(x => x.Stage == ShaderStage.Vertex);
                var fragment = sources.FirstOrDefault(x => x.Stage == ShaderStage.Fragment);

                UploadShader(vertex.Shader, fragment.Shader);
            }
        }

        private void UploadShader(byte[] vertex, byte[] fragment/*, byte[] geometry, byte[] compute*/)
        {
            DestroyNativeShader();

            if (!HasErrors)
            {
                var shaderDescriptor = new ShaderDescriptor()
                {
                    VertexSource = vertex,
                    FragmentSource = fragment,

                    // VertName = vertName,
                    // FragName = fragName
                };

                _nativeShader = GfxDeviceManager.Current.CreateShader(shaderDescriptor);
            }
        }

        [Obsolete]
        public static Shader FromPath(string vertex, string fragment)
        {
            var vertexCode = Assets.GetText(vertex).Text;
            var fragCode = Assets.GetText(fragment).Text;
            return new Shader(vertexCode, fragCode, System.IO.Path.GetFileName(vertex), System.IO.Path.GetFileName(fragment));
        }

        private void DestroyNativeShader()
        {
            GfxDeviceManager.Current.DestroyResource(_nativeShader);
            _nativeShader = null;

        }
        protected internal override void OnDestroy()
        {
            DestroyNativeShader();
        }

    }
}
