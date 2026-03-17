using Engine.Graphics;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Material : AssetResourceBase
    {
        [SerializedField] private List<RenderPass> _passes = new();
        internal List<RenderPass> Passes => _passes;
        [SerializedField] private Dictionary<string, Texture> _textures = new();
        internal Dictionary<string, Texture> Textures => _textures;
        internal ulong MaterialInstanceId { get; private set; }
        public Shader Shader
        {
            get => _passes.FirstOrDefault()?.Shader; 
            set
            {
                if(_passes.Count == 0)
                {
                    _passes.Add(new RenderPass(value));
                }
                else
                {
                    _passes[0].Shader = value;
                }
            }
        }

        private const string _defaultTypeName = "Material";

        public Material(Shader shader) : this(_defaultTypeName, shader)
        {
        }

        public Material(string name, Shader shader) : this(Guid.Empty, name, shader)
        {
        }

        //Serializer
        internal Material(string name, Guid guid) : base(guid)
        {
            MaterialInstanceId++;
        }
        public Material(Guid guid, string name, Shader shader) : base(guid)
        {
            MaterialInstanceId++;

            _textures = new();
            _passes = new List<RenderPass>()
            {
                new RenderPass(shader)
            };
        }
        public RenderPass PushPass(Shader shader)
        {
            var pass = new RenderPass(shader);
            _passes.Add(pass);

            return pass;
        }

        public RenderPass GetPass(int index)
        {
            GetPassSafe(index, out var renderPass);

            return renderPass;
        }

        public void RemovePass(RenderPass pass)
        {
            _passes.Remove(pass);
        }

        public void RemovePass(int index)
        {
            _passes.RemoveAt(index);
        }

        public void AddTexture(string name, Texture texture)
        {
            var maxTextures = GfxDeviceManager.Current.GetDeviceInfo().MaxHardwareTextureUnits;
            if (_textures.Count < maxTextures)
            {
                _textures[name] = texture;
            }
            else
            {
                Debug.Error($"Max textures per slot was reached: {maxTextures}");
            }
        }

        public void SetProperty<T>(string name, T value) where T : unmanaged
        {
            _passes[0].SetProperty(name, value);
        }

        public void SetProperty<T>(int pass, string name, T value) where T : unmanaged
        {
            if (GetPassSafe(pass, out var passObj))
            {
                passObj.SetProperty(name, value);
            }
        }

        private bool GetPassSafe(int index, out RenderPass pass)
        {
            pass = null;

            if (_passes.Count > index)
            {
                pass = _passes[index];
                return true;
            }

            Debug.Error($"Render pass index is out of range: {index}");

            return false;
        }

        protected override void OnUpdateResource(object data, Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}