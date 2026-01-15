using SharedTypes;
using Slangc.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SPIRVCross.NET;

namespace GameCooker
{
    internal class SlangShaderAssetProcessor : IAssetProcessor
    {
        private readonly StringBuilder _sb = new();

        byte[] IAssetProcessor.Process(string path, AssetMetaFileBase meta, CookingPlatform platform)
        {
            using var reader = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            var str = reader.ReadToEnd();

            Console.WriteLine("Compile slang: " + path);

            var target = "spirv";
            var vertexEntryPoint = "vertexMain";
            var fragmentEntryPoint = "fragmentMain";

            var vertexArgs = new string[]
            {
                path,
                "-profile", "glsl_330", // "sm_6_6",
                //"-matrix-layout-column-major",
                "-entry",$"{vertexEntryPoint}", "-stage", "vertex",
                //"-entry","fragmentMain", "-stage", "pixel",
                "-target", $"{target}",
                "-capability", "glsl_spirv_1_6",
                "-capability", "vertex"
            };

            var fragmentArgs = new string[]
            {
                path,
                "-profile", "glsl_330",
                //"-matrix-layout-column-major",
                //"-entry","vertexMain", "-stage", "vertex",
                "-entry",$"{fragmentEntryPoint}", "-stage", "fragment",
                "-target", $"{target}",
                "-capability", "glsl_spirv_1_6",
                "-capability", "fragment"
            };

            // var arg = $"slangc {path} -entry vertexMain -stage vertex -entry fragmentMain -stage fragment -target spirv";

            var vertSpirv = SlangCompiler.CompileWithReflection(vertexArgs, out SlangReflection reflection);
            var fragSpirv = SlangCompiler.CompileWithReflection(fragmentArgs, out reflection);

            // Console.WriteLine($"Compilation Time: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"SPIR-V: {vertSpirv.Length} bytes");
            Console.WriteLine($"Reflection JSON: {reflection.Json}");

            reflection.Deserialize();
            var context = new SPIRVCross.NET.Context();

            string CompileGLSL(byte[] spirv)
            {
                var parsedIR = context.ParseSpirv(spirv);

                var compiler = context.CreateGLSLCompiler(parsedIR);
                compiler.glslOptions.ES = true;
                compiler.glslOptions.version = 300;
                compiler.glslOptions.separateShaderObjects = false;
                compiler.glslOptions.vulkanSemantics = false;
                compiler.glslOptions.enableRowMajorLoadWorkaround = false;
                compiler.glslOptions.enable420PackExtension = false;

                return compiler.Compile();

            }
            var vertex = CompileGLSL(vertSpirv);
            var fragment = CompileGLSL(fragSpirv);

            File.WriteAllText(path + "v_.glsl", vertex);
            File.WriteAllText(path + "f_.glsl", fragment);

            return Encoding.UTF8.GetBytes(_sb.ToString().TrimStart());
        }
    }
}