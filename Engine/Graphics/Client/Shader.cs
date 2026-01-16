using Engine.Graphics;
using Engine.IO;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine
{
    public class Shader : AssetResourceBase
    {
        [ShowFieldNoSerialize(isReadOnly: true)] private readonly ShaderUniform[] _uniforms;
        private readonly ShaderSource[] _sources;
        private GfxResource _nativeShader;
        internal GfxResource NativeShader
        {
            get
            {
                if (_nativeShader == null && !HasErrors)
                {
                    var vertex = _sources.FirstOrDefault(x => x.Stage == ShaderStage.Vertex);
                    var fragment = _sources.FirstOrDefault(x => x.Stage == ShaderStage.Fragment);

                    _nativeShader = UploadShader(vertex.Shader, fragment.Shader);
                }

                return _nativeShader;
            }
        }

        internal IReadOnlyList<ShaderUniform> Uniforms => _uniforms;
        internal IReadOnlyList<ShaderSource> ShaderSources => _sources;
        internal bool HasErrors { get; }
        internal Shader(ShaderSource[] sources, string path, Guid guid) : base(path, guid)
        {
            _sources = sources;
            HasErrors = sources == null || sources.Length < 2 || (sources?.Any(x => x?.HasErrors ?? true) ?? true);

            if (!HasErrors)
            {
                _uniforms = sources.SelectMany(x => x.Uniforms).DistinctBy(x => x.Name).ToArray();
            }
        }

        [Obsolete("Load shaders with Assets.GetShader(\\\"pathToShader.slang\\\")")]
        public Shader(string vertexCode, string fragmentCode) : this(vertexCode, fragmentCode, string.Empty, string.Empty)
        {

        }

        [Obsolete]
        public Shader(string vertexCode, string fragmentCode, string vertName, string fragName) :
            base(string.Empty, Guid.NewGuid()) // TODO: load shaders from file.
        {
            _nativeShader = UploadShader(Encoding.UTF8.GetBytes(vertexCode), Encoding.UTF8.GetBytes(fragmentCode));
        }

        private GfxResource UploadShader(byte[] vertex, byte[] fragment/*, byte[] geometry, byte[] compute*/)
        {
            var shaderDescriptor = new ShaderDescriptor()
            {
                VertexSource = vertex,
                FragmentSource = fragment,

                // VertName = vertName,
                // FragName = fragName
            };

            return GfxDeviceManager.Current.CreateShader(shaderDescriptor);
        }

        [Obsolete]
        public static Shader FromPath(string vertex, string fragment)
        {
            var vertexCode = Assets.GetText(vertex).Text;
            var fragCode = Assets.GetText(fragment).Text;
            return new Shader(vertexCode, fragCode, System.IO.Path.GetFileName(vertex), System.IO.Path.GetFileName(fragment));
        }

        protected internal override void OnDestroy()
        {
            GfxDeviceManager.Current.DestroyResource(NativeShader);
        }
    }
}
