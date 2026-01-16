using SharedTypes;
using Slangc.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SPIRVCross.NET;
using Newtonsoft.Json;
using Engine.Graphics;

namespace GameCooker
{
    internal class SlangShaderAssetProcessor : IAssetProcessor
    {
        private const string _target = "spirv";
        private const string _vertexEntryPoint = "vertexMain";
        private const string _fragmentEntryPoint = "fragmentMain";
        private static Context _context = new Context();

        string[] vertexArgs =
        [
            "",
            "-lang", "slang",
            "-profile", "glsl_330", // "sm_6_6",
            "-entry", $"{_vertexEntryPoint}", "-stage", "vertex",
            "-target", $"{_target}",
            //"-allow-glsl",
            "-capability", "glsl_spirv_1_6",
            //"-i", "import_file.slang",
            "-O3",
        ];

        string[] fragmentArgs =
        [
            "",
            "-lang", "slang",
            "-profile", "glsl_330",
            "-entry", $"{_fragmentEntryPoint}", "-stage", "fragment",
            "-target", $"{_target}",
            "-capability", "glsl_spirv_1_6",
            //"-i", "import_file.slang",
            "-O3",
        ];

        byte[] IAssetProcessor.Process(string path, AssetMetaFileBase meta, CookingPlatform platform)
        {
            // var arg = $"slangc {path} -entry vertexMain -stage vertex -entry fragmentMain -stage fragment -target spirv";

            vertexArgs[0] = path;
            fragmentArgs[0] = path;

            var vertSpirv = SlangCompiler.CompileWithReflection(vertexArgs, out var vertReflection);
            var fragSpirv = SlangCompiler.CompileWithReflection(fragmentArgs, out var fragReflection);

            var vertex = CompileGLSL(platform, vertSpirv);
            var fragment = CompileGLSL(platform, fragSpirv);

            File.WriteAllText(path + "v_.glsl", vertex);
            File.WriteAllText(path + "f_.glsl", fragment);

            return GetAsset(vertex, fragment, vertReflection, fragReflection);
        }

        private string CompileGLSL(CookingPlatform platform, byte[] spirv)
        {
            var parsedIR = _context.ParseSpirv(spirv);

            var compiler = _context.CreateGLSLCompiler(parsedIR);
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

        // NOTE: for now this will write a json, for production ready code, it should be binary.
        private byte[] GetAsset(string vertexSrc, string fragmentSrc,
                                SlangReflection vertReflection, SlangReflection fragReflection)
        {
            ShaderUniform[] GetUniforms(SlangReflection reflection)
            {
                reflection.Deserialize();
                var uniforms = new ShaderUniform[reflection.Parameters.Length];

                for (int i = 0; i < reflection.Parameters.Length; i++)
                {
                    var uniform = new ShaderUniform();
                    var parameter = reflection.Parameters[i];
                    uniform.Name = parameter.Name;
                    uniform.Type = GetUniformType(parameter.Type, out var elementCount);
                    uniform.ElementCount = elementCount;
                    uniforms[i] = uniform;
                }

                return uniforms;
            }
            var shaderData = new ShaderData();

            shaderData.Sources =
            [
                new ShaderSource()
                {
                    Shader = Encoding.UTF8.GetBytes(vertexSrc),
                    Uniforms = GetUniforms(vertReflection)
                },
                new ShaderSource()
                {
                    Shader = Encoding.UTF8.GetBytes(fragmentSrc),
                    Uniforms = GetUniforms(fragReflection)
                }
            ];

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(shaderData));
        }

        private UniformType GetUniformType(SlangType type, out int elementCount)
        {
            elementCount = 0;
            switch (type.Kind)
            {
                case SlangTypeKind.Unknown:
                    break;
                //case SlangTypeKind.Struct:
                //    break;
                case SlangTypeKind.Array:
                    if (GetUniformType(type.Array.ElementType, out elementCount) == UniformType.Int)
                    {
                        elementCount = (int)type.Array.ElementCount;
                        return UniformType.IntArr;
                    }
                    break;
                case SlangTypeKind.Matrix:
                    if (type.Matrix.ColumnCount == 2)
                    {
                        // TODO:
                    }
                    else if (type.Matrix.ColumnCount == 3)
                    {
                        // TODO:
                    }
                    else if (type.Matrix.ColumnCount == 4)
                    {
                        return UniformType.Mat4;
                    }
                    break;
                case SlangTypeKind.Vector:
                    if (type.Vector.ElementCount == 2)
                    {
                        return UniformType.Vec2;
                    }
                    else if (type.Vector.ElementCount == 3)
                    {
                        return UniformType.Vec3;
                    }
                    else if (type.Vector.ElementCount == 4)
                    {
                        return UniformType.Vec4;
                    }
                    break;
                case SlangTypeKind.Scalar:
                    if (type.Scalar.ScalarType == SlangScalarType.Int32)
                    {
                        return UniformType.Int;
                    }
                    else if (type.Scalar.ScalarType == SlangScalarType.UInt32)
                    {
                        return UniformType.Uint;
                    }
                    else if (type.Scalar.ScalarType == SlangScalarType.Float32)
                    {
                        return UniformType.Float;
                    }
                    break;
                //case SlangTypeKind.ConstantBuffer:
                //    break;
                //case SlangTypeKind.Resource:
                //    break;
                //case SlangTypeKind.SamplerState:
                //    break;
                //case SlangTypeKind.TextureBuffer:
                //    break;
                //case SlangTypeKind.ShaderStorageBuffer:
                //    break;
                //case SlangTypeKind.ParameterBlock:
                //    break;
                //case SlangTypeKind.GenericTypeParameter:
                //    break;
                //case SlangTypeKind.Interface:
                //    break;
                //case SlangTypeKind.Feedback:
                //    break;
                //case SlangTypeKind.Pointer:
                //    break;
                //case SlangTypeKind.DynamicResource:
                //    break;
                //case SlangTypeKind.OutputStream:
                //    break;
                //case SlangTypeKind.MeshOutput:
                //    break;
                //case SlangTypeKind.Specialized:
                //    break;
                default:
                    break;
            }

            return UniformType.Invalid;
        }
    }
}