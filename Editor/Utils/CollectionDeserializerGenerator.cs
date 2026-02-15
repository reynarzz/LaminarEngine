using Engine;
using Engine.Serialization;
using Engine.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Editor.Cooker
{
    internal static class CollectionDeserializerGenerator
    {
        private sealed record VariantTypeInfo(
            string Name,
            string ClrType,
            string Field,
            string CollectionDataType,
            SerializedType Type
        );

        private static readonly VariantTypeInfo[] VariantTypes =
        {
            new("Char", "char", "Char", nameof(CollectionDataChar), SerializedType.Char),
            new("String", "string", "String",nameof(CollectionDataString), SerializedType.String),
            new("Bool", "bool", "Bool", nameof(CollectionDataBool), SerializedType.Bool),
            new("Byte", "byte", "Byte", nameof(CollectionDataByte), SerializedType.Byte),
            new("Short", "short", "Short", nameof(CollectionDataShort), SerializedType.Short),
            new("UShort", "ushort", "UShort", nameof(CollectionDataUShort), SerializedType.UShort),
            new("Int", "int", "Int", nameof(CollectionDataInt), SerializedType.Int),
            new("UInt", "uint", "Uint",nameof(CollectionDataUInt), SerializedType.UInt),
            new("Long", "long", "Long", nameof(CollectionDataLong), SerializedType.Long),
            new("ULong", "ulong", "Ulong", nameof(CollectionDataULong), SerializedType.ULong),
            new("Float", "float", "Float", nameof(CollectionDataFloat), SerializedType.Float),
            new("Double", "double", "Double", nameof(CollectionDataDouble), SerializedType.Double),
            new("Vec2", "vec2", "Vec2", nameof(CollectionDataVec2), SerializedType.Vec2),
            new("Vec3", "vec3", "Vec3", nameof(CollectionDataVec3), SerializedType.Vec3),
            new("Vec4", "vec4", "Vec4", nameof(CollectionDataVec4), SerializedType.Vec4),
            new("IVec2", "ivec2", "Ivec2", nameof(CollectionDataIvec2), SerializedType.IVec2),
            new("IVec3", "ivec3", "Ivec3", nameof(CollectionDataIvec3), SerializedType.IVec3),
            new("IVec4", "ivec4", "Ivec4", nameof(CollectionDataIvec4), SerializedType.IVec4),
            new("Quat", "quat", "Quat", nameof(CollectionDataQuat), SerializedType.Quat),
            new("Mat2", "mat2", "Mat2", nameof(CollectionDataMat2), SerializedType.Mat2),
            new("Mat3", "mat3", "Mat3", nameof(CollectionDataMat3), SerializedType.Mat3),
            new("Mat4", "mat4", "Mat4", nameof(CollectionDataMat4), SerializedType.Mat4),
            new("Color", nameof(Color), "Color", nameof(CollectionDataColor), SerializedType.Color),
            new("Color32", nameof(Color32), "Color32", nameof(CollectionDataColor32), SerializedType.Color32)
        };

        internal static string Generate(bool generatedDictionary)
        {
            var cu = GenerateCompilationUnit(generatedDictionary);
            return cu.NormalizeWhitespace().ToFullString();
        }

        private static CompilationUnitSyntax GenerateCompilationUnit(bool generatedDictionary)
        {
            var usings = SyntaxFactory.List(new[]
            {
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Engine")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Engine.Serialization")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("GlmNet")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Engine.Utils")),
            });

            var classDecl = SyntaxFactory.ClassDeclaration("CollectionDeserializer")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddMembers(GenerateDispatchMethod());

            if (generatedDictionary)
            {
                classDecl = classDecl.AddMembers(GenerateDictionaryDispatchMethod());
            }

            // Collection writers
            foreach (var t in VariantTypes)
            {
                classDecl = classDecl.AddMembers(
                    GenerateArrayWriter(t),
                    GenerateListWriter(t),
                    GenerateQueueWriter(t),
                    GenerateStackWriter(t),
                    GenerateHashSetWriter(t)
                );
            }
            if (generatedDictionary)
            {
                // Dictionary writers
                foreach (var key in VariantTypes)
                {
                    foreach (var value in VariantTypes)
                    {
                        classDecl = classDecl.AddMembers(GenerateDictionaryWriter(key, value));
                    }
                }
            }

            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Generated")).AddMembers(classDecl);
            return SyntaxFactory.CompilationUnit().AddUsings(usings.ToArray()).AddMembers(ns);
        }

        private static MemberDeclarationSyntax GenerateDispatchMethod()
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"
            internal static object Deserialize1D(object collection, CollectionData data, SerializedType itemType, CollectionType kind)
            {
                ulong identity = itemType.GetIdentity();");

            foreach (var info in VariantTypes)
            {
                var varName = $"{info.ClrType}Collection";
                sb.AppendLine($"        if (identity == SerializedType.{info.Type}.GetIdentity())");
                sb.AppendLine("        {");
                sb.AppendLine($"           var {varName} = data as {info.CollectionDataType};");
                sb.AppendLine("            if (kind == CollectionType.Array) return ReadToArray_" + info.Name + $"(collection, {varName});");
                sb.AppendLine("            else if (kind == CollectionType.List) return ReadToList_" + info.Name + $"(collection, {varName});");
                sb.AppendLine("            else if (kind == CollectionType.Queue) return ReadToQueue_" + info.Name + $"(collection, {varName});");
                sb.AppendLine("            else if (kind == CollectionType.Stack) return ReadToStack_" + info.Name + $"(collection, {varName});");
                sb.AppendLine("            else if (kind == CollectionType.HashSet) return ReadToHashSet_" + info.Name + $"(collection, {varName});");
                sb.AppendLine("        }");
            }

            sb.AppendLine(@"
                throw new NotSupportedException();
            }");

            return ParseWriter(sb.ToString());
        }

        private static MemberDeclarationSyntax GenerateDictionaryDispatchMethod()
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"
            internal static object Deserialize(object collection, Variant[] keys, Variant[] values, 
                                          SerializedType keyType, SerializedType valueType)
            {
                ulong keyId = keyType.GetIdentity();
                ulong valueId = valueType.GetIdentity();");

            bool firstKey = true;
            foreach (var key in VariantTypes)
            {
                var keyCond = firstKey ? "if" : "else if";
                sb.AppendLine($"    {keyCond} (keyId == SerializedType.{key.Type}.GetIdentity())");
                sb.AppendLine("    {");

                bool firstValue = true;
                foreach (var value in VariantTypes)
                {
                    var valueCond = firstValue ? "if" : "else if";
                    sb.AppendLine($"        {valueCond} (valueId == SerializedType.{value.Type}.GetIdentity()) return ReadToDictionary_{key.Name}_{value.Name}(collection, keys, values);");
                    firstValue = false;
                }

                sb.AppendLine("    }");
                firstKey = false;
            }

            sb.AppendLine(@"
                    throw new NotSupportedException();
            }");

            return ParseWriter(sb.ToString());
        }
        private static MemberDeclarationSyntax GenerateArrayWriter(VariantTypeInfo t)
        {
            return ParseWriter($@"
            private static object ReadToArray_{t.Name}(object collection, {t.CollectionDataType} data)
            {{
                var array = ({t.ClrType}[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
                for (int i = 0; i < data.Value.Length; i++)
                    array[i] = data.Value[i];
                return array;
            }}");
        }

        private static MemberDeclarationSyntax GenerateListWriter(VariantTypeInfo t)
        {
            var isStr = t.Type == SerializedType.String;
            return ParseWriter($@"
            private static object ReadToList_{t.Name}(object collection, {t.CollectionDataType} data)
            {{
                var list = (List<{t.ClrType}>)collection;
                System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
                for (int i = 0; i < data.Value.Length; i++)
                    list[i] = data.Value[i];
                return list;
            }}");
        }

        private static MemberDeclarationSyntax GenerateQueueWriter(VariantTypeInfo t)
        {
            return ParseWriter($@"
            private static object ReadToQueue_{t.Name}(object collection, {t.CollectionDataType} data)
            {{
                var queue = (Queue<{t.ClrType}>)collection;
                queue.Clear();
                for (int i = 0; i < data.Value.Length; i++)
                    queue.Enqueue(data.Value[i]);
                return queue;
            }}");
        }

        private static MemberDeclarationSyntax GenerateStackWriter(VariantTypeInfo t)
        {
            return ParseWriter($@"
            private static object ReadToStack_{t.Name}(object collection, {t.CollectionDataType} data)
            {{
                var stack = (Stack<{t.ClrType}>)collection;
                stack.Clear();
                for (int i = data.Value.Length - 1; i >= 0; i--)
                    stack.Push(data.Value[i]);
                return stack;
            }}");
        }

        private static MemberDeclarationSyntax GenerateHashSetWriter(VariantTypeInfo t)
        {
            return ParseWriter($@"
            private static object ReadToHashSet_{t.Name}(object collection, {t.CollectionDataType} data)
            {{
                var set = (HashSet<{t.ClrType}>)collection;
                set.Clear();
                for (int i = 0; i < data.Value.Length; i++)
                    set.Add(data.Value[i]);
                return set;
            }}");
        }

        private static MemberDeclarationSyntax GenerateDictionaryWriter(VariantTypeInfo key, VariantTypeInfo value)
        {
            var isKStr = key.Type == SerializedType.String;
            var isVStr = value.Type == SerializedType.String;
            return ParseWriter($@"
            private static object ReadToDictionary_{key.Name}_{value.Name}(object collection, {nameof(Variant)}[] keys, {nameof(Variant)}[] values)
            {{
                var dict = (Dictionary<{key.ClrType}, {value.ClrType}>)collection;
                dict.Clear();
                for (int i = 0; i < keys.Length; i++)
                    dict.Add(keys[i]{(isKStr ? "" : ".value")}.{key.Field}, values[i]{(isVStr ? "" : ".value")}.{value.Field});
                return dict;
            }}");
        }

        private static MemberDeclarationSyntax ParseWriter(string src)
        {
            return SyntaxFactory.ParseMemberDeclaration(src)!;
        }
    }
}
