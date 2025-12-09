using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public static class MaterialUtils
    {
        public static Material SpriteMaterial { get; }
        public static Material SpriteMaterialOverlay { get; }
        public static Material SpriteMaterialWorld { get; }
        public static Material UIMaterial { get; }
        public static Material FontMaterial { get; }
        public static Material PortalMaterial { get; }
        public static Material WobbleMaterial { get; }

        static MaterialUtils()
        {
            SpriteMaterial = GetMaterial("SpriteMaterial", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            SpriteMaterialOverlay = GetMaterial("SpriteMaterialOverlay", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            SpriteMaterialWorld = GetMaterial("SpriteMaterialWorld", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            WobbleMaterial = GetMaterial("WobbleMaterial", "Shaders/VertScreenGrab.vert", "Shaders/ScreenGrabWobble.frag");
            UIMaterial = GetMaterial("UIMaterial", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
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

            PortalMaterial = InitPortalMaterial();
            WobbleMaterial.GetPass(0).IsScreenGrabPass = true;
            WobbleMaterial.SetProperty(0, "uDistortionAmount", 0.0003f);
            WobbleMaterial.SetProperty(0, "uColorSplit", 0.0017f);
            WobbleMaterial.SetProperty(0, "uPixelationAmount", 0.0f);
        }

        private static Material InitPortalMaterial()
        {
            var screenShader = new Shader(Assets.GetText("Shaders/VertScreenGrab.vert").Text, Assets.GetText("Shaders/Portal.frag").Text);
            var material = new Material(screenShader);
            material.Name = "Portal Material";
            material.AddTexture("uStarsTex", Assets.GetTexture("stars.png"));
            var pass = material.GetPass(0);
            pass.IsScreenGrabPass = true;
            material.SetProperty("uDistortionAmount", 0.009f);
            material.SetProperty("uOutlineColor", new vec3(1.6f, 1.0f, 1.0f));
            material.SetProperty("uOutlineThickness", 0.01f);
            material.SetProperty("uDotted", 0);
            material.SetProperty("uDotSpacing", 0.15f);
            material.SetProperty("uGlitchMaxOffset", 0.03f);
            material.SetProperty("uGlitchFreq", 20.0f);
            material.SetProperty("uColorSplit", 0.001f);

            return material;
        }
        private static Material GetMaterial(string name, string vertexPath, string fragmentPath)
        {
            var material = new Material(new Shader(Assets.GetText(vertexPath).Text, Assets.GetText(fragmentPath).Text, vertexPath, fragmentPath));
            material.Name = name;

            return material;
        }
    }
}
