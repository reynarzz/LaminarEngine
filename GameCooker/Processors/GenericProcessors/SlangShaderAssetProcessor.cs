using SharedTypes;
using Slangc.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

            var args = new string[]
            {
                path,
                "-profile", "sm_6_6",
                "-matrix-layout-column-major",
                "-entry","vertexMain", "-stage", "vertex",
                "-entry","fragmentMain", "-stage", "pixel",
                "-target", "spirv"
            };

           // var arg = $"slangc {path} -entry vertexMain -stage vertex -entry fragmentMain -stage fragment -target spirv";


            byte[] spv = SlangCompiler.CompileWithReflection(args, out SlangReflection reflection);


           // Console.WriteLine($"Compilation Time: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"SPIR-V: {spv.Length} bytes");
            Console.WriteLine($"Reflection JSON: {reflection.Json}");

            reflection.Deserialize();

            //SPIRVCross.NET.
           

            return Encoding.UTF8.GetBytes(_sb.ToString().TrimStart());
        }
    }
}