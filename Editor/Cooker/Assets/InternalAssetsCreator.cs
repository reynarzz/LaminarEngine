using Editor.Serialization;
using Editor.Utils;
using Engine;
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
            var ir = Serializer.Serialize(material);
            var matJson = EditorJsonUtils.Serialize(ir);
            File.WriteAllText(Path.Combine(EditorPaths.CookerPaths.InternalAssetsPath, "Materials", "SpriteDefault.material"), matJson);
        }
    }
}
