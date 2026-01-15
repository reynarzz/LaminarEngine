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
                "-entry",$"{vertexEntryPoint}", "-stage", "vertex",
                "-lang", "glsl",
                "-target", $"{target}",
                //"-allow-glsl",
                "-no-mangle",
                "-capability", "glsl_spirv_1_6",
                //"-i", "import_file.slang",
                "-O3",
            };

            var fragmentArgs = new string[]
            {
                path,
                "-profile", "glsl_330",
                "-entry",$"{fragmentEntryPoint}", "-stage", "fragment",
                "-lang", "glsl",
                "-target", $"{target}",
                "-no-mangle",
                "-capability", "glsl_spirv_1_6",
                "-O3",
            };

            // var arg = $"slangc {path} -entry vertexMain -stage vertex -entry fragmentMain -stage fragment -target spirv";

            var vertSpirv = SlangCompiler.CompileWithReflection(vertexArgs, out SlangReflection vertReflection);
            var fragSpirv = SlangCompiler.CompileWithReflection(fragmentArgs, out var fragReflection);

            Console.WriteLine($"SPIR-V: {vertSpirv.Length} bytes");
            Console.WriteLine($"Reflection JSON: {vertReflection.Json}");

            vertReflection.Deserialize();
            var context = new Context();

            string CompileGLSL(byte[] spirv)
            {
                var parsedIR = context.ParseSpirv(spirv);

                var compiler = context.CreateGLSLCompiler(parsedIR);
                bool isMobile = false;

                if (platform == CookingPlatform.Android || platform == CookingPlatform.IOS)
                {
                    compiler.glslOptions.ES = true;
                    compiler.glslOptions.version = 300;
                    isMobile = true;
                }
                else
                {
                    compiler.glslOptions.version = 330;
                }
                compiler.glslOptions.separateShaderObjects = false;
                compiler.glslOptions.vulkanSemantics = false;
                compiler.glslOptions.enableRowMajorLoadWorkaround = false;
                compiler.glslOptions.enable420PackExtension = false;
                compiler.glslOptions.emitUniformBufferAsPlainUniforms = true;
                var str = compiler.Compile();

                if (!isMobile)
                {
                    str = str.Replace("#version 330", "#version 330 core");
                }

                return str;
            }
            var vertex = CompileGLSL(vertSpirv);
            var fragment = CompileGLSL(fragSpirv);

            File.WriteAllText(path + "v_.glsl", vertex);
            File.WriteAllText(path + "f_.glsl", fragment);

            return Encoding.UTF8.GetBytes(_sb.ToString().TrimStart());
        }
    }
}