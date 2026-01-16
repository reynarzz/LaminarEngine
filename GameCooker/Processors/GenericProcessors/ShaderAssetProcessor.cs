using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SPIRVCross.NET;
using Newtonsoft.Json;
using Engine.Graphics;
using System.Text.RegularExpressions;
using Glslang.NET;
using SPIRVCross.NET.GLSL;

namespace GameCooker
{
    internal class ShaderAssetProcessor : IAssetProcessor
    {
        private static Context _context = new Context();

        byte[] IAssetProcessor.Process(string path, AssetMetaFileBase meta, CookingPlatform platform)
        {
            var isMobile = platform == CookingPlatform.Android || platform == CookingPlatform.IOS;

            var shaderFile = File.ReadAllText(path);
            var lines = shaderFile.Split('\n');

            var vertexTag = "##[Vertex]";
            var fragmentTag = "##[Fragment]";

            var vertexIndex = shaderFile.IndexOf(vertexTag);
            var fragmentIndex = shaderFile.IndexOf(fragmentTag);

            if (vertexIndex < 0 || fragmentIndex < 0)
            {
                // TODO: return pink shader.
                throw new Exception("Invalid shader");
            }

            var version = "#version 330 core\n\n";
            var vertexCode = version + shaderFile.Substring(vertexIndex + vertexTag.Length, fragmentIndex - vertexTag.Length).Trim();
            var fragmentCode = version + shaderFile.Substring(fragmentIndex + fragmentTag.Length).Trim();

            File.WriteAllText(path + "v_.glsl", vertexCode);
            File.WriteAllText(path + "f_.glsl", fragmentCode);

            var spirVs = CompileToSpirV(false, (ShaderStage.Vertex, vertexCode),
                                               (ShaderStage.Fragment, fragmentCode));

            //platform = CookingPlatform.Android;
            ShaderSource vertex = null;
            ShaderSource fragment = null;
            if (spirVs != null && spirVs.Length >= 2)
            {
                vertex = CompileGLSL(isMobile, spirVs[0].spirv);
                fragment = CompileGLSL(isMobile, spirVs[1].spirv);

            }
            else
            {
                // Pink shader
            }
            File.WriteAllText(path + "v_.glsl", Encoding.UTF8.GetString(vertex.Shader));
            File.WriteAllText(path + "f_.glsl", Encoding.UTF8.GetString(fragment.Shader));

            return GetAsset([vertex, fragment]);
        }
        // NOTE: for now this will write a json, for production ready code, it should be binary.
        private byte[] GetAsset(ShaderSource[] sources)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ShaderData() { Sources = sources }));
        }

        private (ShaderStage stage, byte[] spirv)[] CompileToSpirV(bool isMobile, params (ShaderStage stage, string shaderCode)[] shaders)
        {
            var shadersSpirv = new (ShaderStage stage, byte[] spirv)[shaders.Length];
            var program = new Program();

            for (int i = 0; i < shaders.Length; i++)
            {
                var shaderData = shaders[i];

                var input = new CompilationInput()
                {
                    language = SourceType.GLSL,
                    entrypoint = "main",
                    defaultProfile = isMobile ? ShaderProfile.ES : ShaderProfile.CoreProfile,
                    client = ClientType.OpenGL,
                    clientVersion = TargetClientVersion.OpenGL_450,
                    messages = MessageType.Default,
                    stage = shaderData.stage,
                    defaultVersion = isMobile ? 310 : 330,
                    targetLanguage = TargetLanguage.SPV,
                    forceDefaultVersionAndProfile = true,
                    forwardCompatible = false,
                    code = shaderData.shaderCode,
                };
                var shader = new Shader(input);
                shader.SetOptions(ShaderOptions.AutoMapBindings | ShaderOptions.AutoMapLocations | ShaderOptions.MapUnusedUniforms);

                if (!shader.Preprocess())
                {
                    Console.WriteLine("shader preprocessing failed");
                    Console.WriteLine(shader.GetInfoLog());
                    Console.WriteLine(shader.GetDebugLog());
                    return null;
                }

                if (!shader.Parse())
                {
                    Console.WriteLine("shader parsing failed");
                    Console.WriteLine(shader.GetInfoLog());
                    Console.WriteLine(shader.GetDebugLog());
                    Console.WriteLine(shader.GetPreprocessedCode());
                    return null;
                }

                if (shader.Parse())
                {
                    program.AddShader(shader);
                }
            }
            if (!program.Link(MessageType.SpvRules))
            {
                Console.WriteLine("shader linking failed");
                Console.WriteLine(program.GetInfoLog());
                Console.WriteLine(program.GetDebugLog());
                return null;
            }

            for (int i = 0; i < shaders.Length; i++)
            {
                var shaderData = shaders[i];

                program.GenerateSPIRV(out uint[] spirv, shaderData.stage);

                string messages = program.GetSPIRVMessages();

                if (!string.IsNullOrWhiteSpace(messages))
                    Console.WriteLine(messages);

                shadersSpirv[i] = (shaderData.stage, UIntArrayToBytes(spirv));
            }

            return shadersSpirv;
        }

        private ShaderSource CompileGLSL(bool isMobile, byte[] spirv)
        {
            var parsedIR = _context.ParseSpirv(spirv);

            var compiler = _context.CreateGLSLCompiler(parsedIR);

            if (isMobile)
            {
                compiler.glslOptions.ES = true;
                compiler.glslOptions.version = 300;
            }
            else
            {
                compiler.glslOptions.ES = false;
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
            else
            {
                str = str.Replace("#version 330", "#version 300 es");
            }

            // Current compiler add this when 'separateShaderObjects' is true, but I need it to be true because it also adds layouts.
            //str = str.Replace("#extension GL_ARB_separate_shader_objects : require", string.Empty);
            // str = RemoveGlPerVertexBlock(str);

            var resources = compiler.CreateShaderResources();

            return new ShaderSource()
            {
                Shader = Encoding.UTF8.GetBytes(str),
                Uniforms = GetUniforms(resources, compiler)
            };
        }


        private ShaderUniform[] GetUniforms(SPIRVCross.NET.Resources resources, SPIRVCross.NET.Compiler compiler)
        {
            var uniforms = new List<ShaderUniform>();

            void PushUniforms(ReadOnlySpan<ReflectedResource> reflected)
            {
                for (int i = 0; i < reflected.Length; i++)
                {
                    var resourceReflected = reflected[i];
                    var typeHandle = compiler.GetTypeHandle(resourceReflected.type_id);

                    var uniform = new ShaderUniform();
                    uniform.Name = resourceReflected.name;
                    uniform.Type = GetUniformType(typeHandle, compiler, out var elementCount);
                    uniform.ElementCount = elementCount;

                    uniforms.Add(uniform);
                }
            }

            PushUniforms(resources.PlainUniforms);
            PushUniforms(resources.SampledImages);

            return uniforms.ToArray();
        }


        private UniformType GetUniformType(SPIRVCross.NET.Type type, SPIRVCross.NET.Compiler compiler, out int elementCount)
        {
            elementCount = 0;

            //switch (type.Kind)
            //{
            //    case SlangTypeKind.Unknown:
            //        break;
            //    //case SlangTypeKind.Struct:
            //    //    break;
            //    case SlangTypeKind.Array:
            //        if (GetUniformType(type.Array.ElementType, out elementCount) == UniformType.Int)
            //        {
            //            elementCount = (int)type.Array.ElementCount;
            //            return UniformType.IntArr;
            //        }
            //        break;
            //    case SlangTypeKind.Matrix:
            //        if (type.Matrix.ColumnCount == 2)
            //        {
            //            // TODO:
            //        }
            //        else if (type.Matrix.ColumnCount == 3)
            //        {
            //            // TODO:
            //        }
            //        else if (type.Matrix.ColumnCount == 4)
            //        {
            //            return UniformType.Mat4;
            //        }
            //        break;
            //    case SlangTypeKind.Vector:
            //        if (type.Vector.ElementCount == 2)
            //        {
            //            return UniformType.Vec2;
            //        }
            //        else if (type.Vector.ElementCount == 3)
            //        {
            //            return UniformType.Vec3;
            //        }
            //        else if (type.Vector.ElementCount == 4)
            //        {
            //            return UniformType.Vec4;
            //        }
            //        break;
            //    case SlangTypeKind.Scalar:
            //        if (type.Scalar.ScalarType == SlangScalarType.Int32)
            //        {
            //            return UniformType.Int;
            //        }
            //        else if (type.Scalar.ScalarType == SlangScalarType.UInt32)
            //        {
            //            return UniformType.Uint;
            //        }
            //        else if (type.Scalar.ScalarType == SlangScalarType.Float32)
            //        {
            //            return UniformType.Float;
            //        }
            //        break;
            //    //case SlangTypeKind.ConstantBuffer:
            //    //    break;
            //    //case SlangTypeKind.ConstantBuffer:
            //    //    break;
            //    //case SlangTypeKind.Resource:
            //    //    break;
            //    //case SlangTypeKind.SamplerState:
            //    //    break;
            //    //case SlangTypeKind.TextureBuffer:
            //    //    break;
            //    //case SlangTypeKind.ShaderStorageBuffer:
            //    //    break;
            //    //case SlangTypeKind.ParameterBlock:
            //    //    break;
            //    //case SlangTypeKind.GenericTypeParameter:
            //    //    break;
            //    //case SlangTypeKind.Interface:
            //    //    break;
            //    //case SlangTypeKind.Feedback:
            //    //    break;
            //    //case SlangTypeKind.Pointer:
            //    //    break;
            //    //case SlangTypeKind.DynamicResource:
            //    //    break;
            //    //case SlangTypeKind.OutputStream:
            //    //    break;
            //    //case SlangTypeKind.MeshOutput:
            //    //    break;
            //    //case SlangTypeKind.Specialized:
            //    //    break;
            //    default:
            //        break;
            //}

            return UniformType.Invalid;
        }

        private string RemoveGlPerVertexBlock(string glsl)
        {
            var pattern = @"out\s+gl_PerVertex\s*\{[\s\S]*?\};";
            return Regex.Replace(glsl, pattern, "");
        }

        public static byte[] UIntArrayToBytes(uint[] uintArray, bool littleEndian = true)
        {
            return uintArray.SelectMany(u =>
            {
                var bytes = BitConverter.GetBytes(u);
                if (BitConverter.IsLittleEndian != littleEndian)
                {
                    Array.Reverse(bytes);
                }
                return bytes;
            }).ToArray();
        }

    }
}