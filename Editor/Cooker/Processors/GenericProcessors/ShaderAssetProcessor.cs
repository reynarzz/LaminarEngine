using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SPIRVCross.NET;
using Newtonsoft.Json;
using Engine.Graphics;
using System.Text.RegularExpressions;
using Glslang.NET;
using Editor.Serialization;
using Engine.Serialization;
using Editor.Utils;

namespace Editor.Cooker
{
    internal class ShaderAssetProcessor : TextAssetProcessor
    {
        private static Context _context = new Context();
        private const string INCLUDE_EXTENSION = "#extension GL_GOOGLE_include_directive : enable";
        private const string VERSION = "#version 330 core";
        private const string VERTEX_KEYWORD = "VERTEX_SHADER";
        private const string FRAGMENT_KEYWORD = "FRAGMENT_SHADER";

        private const string SHADER_HEADER = VERSION + "\n" + INCLUDE_EXTENSION + "\n";
        public override AssetProccesResult Process(BinaryReader reader, AssetMeta meta, CookingPlatform platform)
        {
            var shaderFile = Encoding.UTF8.GetString(base.Process(reader, meta, platform).Data);

            if (string.IsNullOrEmpty(shaderFile))
            {
                Console.WriteLine("Error: Empty shader.");
                return default;
            }

            var vertexCode = ExtractShader(shaderFile, VERTEX_KEYWORD);
            var fragmentCode = ExtractShader(shaderFile, FRAGMENT_KEYWORD);

            if (string.IsNullOrEmpty(vertexCode) || string.IsNullOrEmpty(fragmentCode))
            {
                return default;
            }
            else
            {
                vertexCode = SHADER_HEADER + vertexCode;
                fragmentCode = SHADER_HEADER + fragmentCode;
            }

            var spirVs = CompileToSpirV(
            [
                (Glslang.NET.ShaderStage.Vertex, vertexCode),
                (Glslang.NET.ShaderStage.Fragment, fragmentCode)
            ]);

            if (spirVs != null && spirVs.Length >= 2)
            {
                var isMobile = platform == CookingPlatform.Android || platform == CookingPlatform.IOS;

                var sources = new ShaderSource[spirVs.Length];
                for (int i = 0; i < spirVs.Length; i++)
                {
                    sources[i] = CompileGLSL(isMobile, spirVs[i]);
                }

                // Testing remove.
                // File.WriteAllText(path + "v_.glsl", Encoding.UTF8.GetString(sources[0].Shader));
                // File.WriteAllText(path + "f_.glsl", Encoding.UTF8.GetString(sources[1].Shader));

                return new AssetProccesResult()
                {
                    IsSuccess = true,
                    Data = GetAsset(sources)
                };
            }

            return default;
        }

        // NOTE: for now this will write a json, for production ready code, it should be binary.
        protected virtual byte[] GetAsset(ShaderSource[] sources)
        {
            var shaderData = new ShaderData() { Sources = sources };

            var ir = new ShaderIR()
            {
                Properties = Serializer.Serialize(shaderData)
            };
            return Encoding.UTF8.GetBytes(EditorJsonUtils.Serialize(ir));
            // return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(shaderData));
        }

        private (Glslang.NET.ShaderStage stage, byte[] spirv)[] CompileToSpirV((Glslang.NET.ShaderStage stage,
                                                                                string shaderCode)[] shaders)
        {
            var program = new Program();

            for (int i = 0; i < shaders.Length; i++)
            {
                var shaderData = shaders[i];

                var input = new CompilationInput()
                {
                    language = SourceType.GLSL,
                    entrypoint = "main",
                    defaultProfile = ShaderProfile.CoreProfile,
                    client = ClientType.OpenGL,
                    clientVersion = TargetClientVersion.OpenGL_450,
                    messages = MessageType.Default,
                    stage = shaderData.stage,
                    defaultVersion = 330,
                    targetLanguage = TargetLanguage.SPV,
                    forceDefaultVersionAndProfile = true,
                    forwardCompatible = false,
                    code = shaderData.shaderCode,
                    fileIncluder = FileIncluder
                };
                var shader = new Glslang.NET.Shader(input);

                shader.SetOptions(ShaderOptions.AutoMapBindings | ShaderOptions.AutoMapLocations | ShaderOptions.MapUnusedUniforms);

                if (!shader.Preprocess())
                {
                    Console.WriteLine($"shader '{shaderData.stage}' preprocessing failed");
                    Console.WriteLine(shader.GetInfoLog());
                    Console.WriteLine(shader.GetDebugLog());
                    return null;
                }

                if (!shader.Parse())
                {
                    Console.WriteLine($"shader '{shaderData.stage}' parsing failed");
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

            var shadersSpirv = new (Glslang.NET.ShaderStage stage, byte[] spirv)[shaders.Length];

            for (int i = 0; i < shaders.Length; i++)
            {
                var shaderData = shaders[i];

                program.GenerateSPIRV(out uint[] spirv, shaderData.stage);

                var messages = program.GetSPIRVMessages();

                if (!string.IsNullOrWhiteSpace(messages))
                {
                    Console.WriteLine(messages);
                }

                shadersSpirv[i] = (shaderData.stage, UIntArrayToBytes(spirv));
            }

            return shadersSpirv;
        }

        private IncludeResult FileIncluder(string headerName, string includerName, uint includeDepth, bool isSystemFile)
        {
            var includePath = Path.Combine(EditorPaths.CookerPaths.ShadersPath, headerName);
            var includeData = "";
            if (File.Exists(includePath))
            {
                includeData = File.ReadAllText(includePath).Trim();
            }
            return new IncludeResult()
            {
                headerName = headerName,
                headerData = includeData,
            };
        }

        private ShaderSource CompileGLSL(bool isMobile, (Glslang.NET.ShaderStage stage, byte[] spirv) shaderSource)
        {
            var parsedIR = _context.ParseSpirv(shaderSource.spirv);

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
            compiler.glslOptions.emitUniformBufferAsPlainUniforms = false;

            var str = compiler.Compile();

            if (!isMobile)
            {
                str = str.Replace("#version 330", "#version 330 core");
            }
            else
            {
                str = str.Replace("#version 330", "#version 300 es");
            }

            var resources = compiler.CreateShaderResources();

            return new ShaderSource()
            {
                Shader = Encoding.UTF8.GetBytes(str),
                Uniforms = GetUniforms(resources, compiler),
                Stage = (Engine.ShaderStage)shaderSource.stage
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
            elementCount = (int)type.MemberCount;
            switch (type.BaseType)
            {
                //case BaseType.Unknown:
                //    break;
                //case BaseType.Void:
                //    break;
                case BaseType.Boolean:
                    return UniformType.Bool;
                //case BaseType.Int8:
                //    break;
                //case BaseType.UInt8:
                //    break;
                //case BaseType.Int16:
                //    break;
                //case BaseType.UInt16:
                //    break;
                case BaseType.Int32:
                    return UniformType.Int;
                case BaseType.UInt32:
                    return UniformType.Uint;
                //case BaseType.Int64:
                //    break;
                //case BaseType.UInt64:
                //    break;
                //case BaseType.AtomicCounter:
                //    break;
                //case BaseType.Float16:
                //    break;
                case BaseType.Float32:
                    if (type.Columns > 1)
                    {
                        if (type.Columns == 2)
                        {
                            return UniformType.Mat2;
                        }
                        else if (type.Columns == 3)
                        {
                            return UniformType.Mat3;
                        }
                        else if (type.Columns == 4)
                        {
                            return UniformType.Mat4;
                        }
                    }
                    else if (type.VectorSize > 1)
                    {
                        if (type.VectorSize == 2)
                        {
                            return UniformType.Vec2;
                        }
                        else if (type.VectorSize == 3)
                        {
                            return UniformType.Vec3;
                        }
                        else if (type.VectorSize == 4)
                        {
                            return UniformType.Vec4;
                        }
                    }
                    return UniformType.Float;
                case BaseType.Float64:
                    return UniformType.Double;
                case BaseType.Struct:
                    break;
                case BaseType.Image:
                    break;
                case BaseType.SampledImage:
                    var typeImage = compiler.GetTypeHandle(type.ImageSampledType);

                    if (type.ImageDimension == Dimension._2D)
                    {
                        if (type.ArrayDimensions > 0)
                        {
                            elementCount = (int)(uint)type.GetArrayDimension(0);
                            return UniformType.Texture2DArray;
                        }
                        else
                        {
                            return UniformType.Texture2D;
                        }
                    }
                    else if (type.ImageDimension == Dimension._3D)
                    {
                        if (type.ArrayDimensions > 0)
                        {
                            elementCount = (int)(uint)type.GetArrayDimension(0);
                            return UniformType.Texture3DArray;
                        }
                        else
                        {
                            return UniformType.Texture3D;
                        }
                    }
                    else if (type.ImageDimension == Dimension.Cube)
                    {
                        if (type.ArrayDimensions > 0)
                        {
                            elementCount = (int)(uint)type.GetArrayDimension(0);
                            return UniformType.TextureCubeMapArray;
                        }
                        else
                        {
                            return UniformType.TextureCubeMap;
                        }
                    }
                    break;
                case BaseType.Sampler:
                    return UniformType.Sampler;
                case BaseType.AccelerationStructure:
                    break;
                default:
                    break;
            }

            return UniformType.Invalid;
        }

        private string ExtractShader(string source, string keyword)
        {
            int keywordIndex = source.IndexOf(keyword, StringComparison.Ordinal);
            if (keywordIndex == -1)
            {
                Console.WriteLine($"'{keyword}' was not found.");
                return null;
            }

            int braceStart = source.IndexOf('{', keywordIndex);
            if (braceStart == -1)
            {
                Console.WriteLine($"No open braces found, invalid shader.");
                return null;
            }

            int depth = 0;
            for (int i = braceStart; i < source.Length; i++)
            {
                if (source[i] == '{')
                {
                    depth++;
                }
                else if (source[i] == '}')
                {
                    depth--;
                }

                if (depth == 0)
                {
                    // extract contents
                    return source.Substring(braceStart + 1, i - braceStart - 1).Trim();
                }
            }

            Console.WriteLine($"Unmatched braces in ${keyword} block");
            return string.Empty;
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