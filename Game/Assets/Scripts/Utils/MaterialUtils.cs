using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utils
{
    public static class MaterialUtils
    {
        public static Material SpriteMaterial { get; }
        public static Material SpriteMaterialOverlay { get; }
        public static Material SpriteMaterialWorld { get; }
        public static Material UIMaterial { get; }
        public static Material FontMaterial { get; }

        static MaterialUtils()
        {
            SpriteMaterial = GetMaterial("SpriteMaterial", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            SpriteMaterialOverlay = GetMaterial("SpriteMaterial", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            SpriteMaterialWorld = GetMaterial("SpriteMaterial", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            UIMaterial = GetMaterial("SpriteMaterial", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            FontMaterial = GetMaterial("FontMaterial", "Shaders/Font/FontVert.vert", "Shaders/Font/FontFrag.frag");

            var spritePass = SpriteMaterial.GetPass(0);
            spritePass.Stencil.Enabled = true;
            spritePass.Stencil.Func = StencilFunc.Always;
            spritePass.Stencil.Ref = 3;
            spritePass.Stencil.ZPassOp = StencilOp.Replace;

            var overlayPass = SpriteMaterialOverlay.PushPass(new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/SpriteFragOver.frag").Text));
            overlayPass.Stencil.Enabled = true;
            overlayPass.Stencil.Func = StencilFunc.Equal;
            overlayPass.Stencil.Ref = 3;
            overlayPass.Stencil.ZPassOp = StencilOp.Keep;
        }

        private static Material GetMaterial(string name, string vertexCode, string shaderCode)
        {
            var material = new Material(new Shader(Assets.GetText(vertexCode).Text, Assets.GetText(shaderCode).Text));
            material.Name = name;

            return material;
        }
    }
}
