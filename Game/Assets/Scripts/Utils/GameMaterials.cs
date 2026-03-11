using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class GameMaterials
    {
        public Material SpriteMaterial { get; private set; }
        public Material SpriteMaterialOverlay { get; private set; }
        public Material SpriteMaterialWorld { get; private set; }
        public Material UIMaterial { get; private set; }
        public Material FontAnimatedMaterial { get; private set; }
        public Material FontMaterial { get; private set; }
        public Material TilemapMaterial { get; private set; }
        public Material PortalMaterial { get; private set; }
        public Material WobbleMaterial { get; private set; }
        private static GameMaterials _instance;
        public static GameMaterials Instance => _instance ?? (_instance = new());

        private GameMaterials()
        {
            SpriteMaterial = Assets.GetMaterial("__InternalAssets__/Materials/SpriteDefault.material");
            FontMaterial = Assets.GetMaterial("__InternalAssets__/Materials/UITextDefault.material");
            TilemapMaterial = Assets.GetMaterial("__InternalAssets__/Materials/TilemapDefault.material");
            SpriteMaterialOverlay = GetMaterial("SpriteMaterialOverlay", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            SpriteMaterialWorld = GetMaterial("SpriteMaterialWorld", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            WobbleMaterial = GetMaterial("WobbleMaterial", "Shaders/VertScreenGrab.vert", "Shaders/ScreenGrabWobble.frag");
            UIMaterial = GetMaterial("UIMaterial", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            FontAnimatedMaterial = GetMaterial("FontMaterial", "Shaders/Font/FontVert.vert", "Shaders/Font/FontFrag.frag");
            
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

#if DESKTOP
            PortalMaterial = Assets.GetMaterial("Materials/Portal.material"); 
#else
            PortalMaterial = Assets.GetMaterial("Materials/Portal_mobile.material");
#endif

            WobbleMaterial.GetPass(0).IsScreenGrabPass = true;
            WobbleMaterial.SetProperty(0, "uDistortionAmount", 0.0003f);
            WobbleMaterial.SetProperty(0, "uColorSplit", 0.0017f);
            WobbleMaterial.SetProperty(0, "uPixelationAmount", 0.0f);
            
            FontAnimatedMaterial.SetProperty(0, "uAmplitude", 2.0f);
            FontAnimatedMaterial.SetProperty(0, "uFrequency", 6.0f);
        }
        
      
        private static Material GetMaterial(string name, string vertexPath, string fragmentPath)
        {
            var material = new Material(new Shader(Assets.GetText(vertexPath).Text, Assets.GetText(fragmentPath).Text, vertexPath, fragmentPath));
            material.Name = name;

            return material;
        }
    }
}
