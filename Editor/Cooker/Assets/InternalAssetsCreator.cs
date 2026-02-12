using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal static class InternalAssetsCreator
    {
        public static void GenerateAll()
        {
            GenerateSpriteDefaultMaterial();
            Generate_TEMP_PortalMaterial_PC();
            Generate_TEMP_PortalMaterial_Mobile();
        }

        private static void GenerateSpriteDefaultMaterial()
        {
            var shader = Assets.GetShader("__InternalAssets__/Shaders/SpriteDefault.shader");
            var material = new Material(shader);

            var spritePass = material.GetPass(0);
            spritePass.Stencil.Enabled = true;
            spritePass.Stencil.Func = StencilFunc.Always;
            spritePass.Stencil.Ref = 3;
            spritePass.Stencil.ZPassOp = StencilOp.Replace;

            var path = Path.Combine(EditorPaths.CookerPaths.InternalAssetsPath, "Materials", "SpriteDefault.material");
            WriteMaterial(material, path);
        }

        private static void Generate_TEMP_PortalMaterial_PC()
        {
            var shader = Assets.GetShader("Shaders/Portal.shader");//new Shader(Assets.GetText("Shaders/VertScreenGrab.vert").Text, Assets.GetText("Shaders/Portal_mobile.frag").Text);
            var material = new Material(shader);
            material.Name = "Portal Material";
            material.AddTexture("uStarsTex", Assets.GetTexture("stars.png"));
            material.AddTexture("uFrameTex", Assets.GetTexture("portal_frame.png"));

            var pass = material.GetPass(0);
            pass.IsScreenGrabPass = true;
            material.SetProperty("uDistortionAmount", 0.009f);
            material.SetProperty("uOutlineColor", new GlmNet.vec3(1.6f, 1.0f, 1.0f));
            material.SetProperty("uOutlineThickness", 0.02f);
            material.SetProperty("uDotted", 0);
            material.SetProperty("uDotSpacing", 0.15f);
            material.SetProperty("uGlitchMaxOffset", 0.03f);
            material.SetProperty("uGlitchFreq", 20.0f);
            material.SetProperty("uColorSplit", 0.001f);
            material.SetProperty("uPixelationAmount", 2.0f);

            WriteMaterial(material, Path.Combine(Paths.GetAssetsFolderPath(), "Materials", "Portal.material"));
        }

        private static void Generate_TEMP_PortalMaterial_Mobile()
        {
            var shader = Assets.GetShader("Shaders/Portal_mobile.shader"); //new Shader(.GetText("Shaders/VertScreenGrab.vert").Text, Assets.GetText("Shaders/Portal_mobile_cheap.frag").Text);
            var material = new Material(shader);
            material.Name = "Portal Material_Mobile";
            material.AddTexture("uStarsTex", Assets.GetTexture("stars.png"));
            material.AddTexture("uFrameTex", Assets.GetTexture("portal_frame.png"));


            WriteMaterial(material, Path.Combine(Paths.GetAssetsFolderPath(), "Materials", "Portal_mobile.material"));
        }

        private static void WriteMaterial(Material material, string path)
        {
            var ir = Serializer.Serialize(material);
            var materialIR = new MaterialIR()
            {
                Version = 1,
                Properties = ir
            };
            var matJson = EditorJsonUtils.Serialize(materialIR);
            File.WriteAllText(path, matJson);
        }
    }
}
