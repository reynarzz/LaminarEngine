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
        public static Material UIMaterial { get; }
        public static Material FontMaterial { get; }

        static MaterialUtils()
        {
            SpriteMaterial = GetMaterial("SpriteMaterial", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            UIMaterial = GetMaterial("SpriteMaterial", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            FontMaterial = GetMaterial("FontMaterial", "Shaders/Font/FontVert.vert", "Shaders/Font/FontFrag.frag");


            var spritePass = SpriteMaterial.Passes.ElementAt(0);
            spritePass.Stencil.Enabled = true;
            spritePass.Stencil.Func = StencilFunc.Always;
            spritePass.Stencil.Ref = 3;
            spritePass.Stencil.ZPassOp = StencilOp.Replace;
        }

        private static Material GetMaterial(string name, string vertexCode, string shaderCode)
        {
            var material = new Material(new Shader(Assets.GetText(vertexCode).Text, Assets.GetText(shaderCode).Text));
            material.Name = name;

            return material;
        }
    }
}
