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
    public class Material : EObject
    {
        private readonly List<RenderPass> _passes;
        internal List<RenderPass> Passes => _passes;
        private readonly OrderedDictionary<string, Texture> _textures;
        internal OrderedDictionary<string, Texture> Textures => _textures;
        public Shader Shader { get; }
        public Material(Shader shader) : base("Material")
        {
            Shader = shader;
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

        public int AddTexture(string name, Texture texture)
        {
            if(_textures.Count < GfxDeviceManager.Current.GetDeviceInfo().MaxHardwareTextureUnits)
            {
                _textures[name] = texture;

                return _textures.IndexOf(name);
            }

            return -1; 
        }

        public void SetProperty<T>(string name, T value) where T : unmanaged
        {
            _passes[0].SetProperty(name, value);
        }

        public void SetProperty<T>(int pass, string name, T value) where T: unmanaged
        {
            if(GetPassSafe(pass, out var passObj))
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
    }
}