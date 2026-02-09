using Engine.Serialization;
using Engine.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Editor.Cooker
{
    internal static class VariantCollectionWriterGenerator
    {
        private sealed record VariantTypeInfo(
            string Name,
            string ClrType,
            string PayloadField,
            SerializedType Type
        );

        private static readonly VariantTypeInfo[] VariantTypes =
        {
            new("Char", "char", "Char", SerializedType.Char),
            new("Bool", "bool", "Bool", SerializedType.Bool),
            new("Byte", "byte", "Byte", SerializedType.Byte),
            new("Short", "short", "Short", SerializedType.Short),
            new("UShort", "ushort", "UShort", SerializedType.UShort),
            new("Int", "int", "Int", SerializedType.Int),
            new("UInt", "uint", "Uint", SerializedType.UInt),
            new("Long", "long", "Long", SerializedType.Long),
            new("ULong", "ulong", "Ulong", SerializedType.ULong),
            new("Float", "float", "Float", SerializedType.Float),
            new("Double", "double", "Double", SerializedType.Double),
            new("Vec2", "vec2", "Vec2", SerializedType.Vec2),
            new("Vec3", "vec3", "Vec3", SerializedType.Vec3),
            new("Vec4", "vec4", "Vec4", SerializedType.Vec4),
            new("IVec2", "ivec2", "Ivec2", SerializedType.IVec2),
            new("IVec3", "ivec3", "Ivec3", SerializedType.IVec3),
            new("IVec4", "ivec4", "Ivec4", SerializedType.IVec4),
            new("Quat", "quat", "Quat", SerializedType.Quat),
            new("Mat2", "mat2", "Mat2", SerializedType.Mat2),
            new("Mat3", "mat3", "Mat3", SerializedType.Mat3),
            new("Mat4", "mat4", "Mat4", SerializedType.Mat4),
            new("Color", "Color", "Color", SerializedType.Color),
            new("Color32", "Color32", "Color32", SerializedType.Color32)
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

            var classDecl = SyntaxFactory.ClassDeclaration("VariantCollectionWriter")
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
            internal static object Write(object collection, VariantIRValue[] values, SerializedType itemType, CollectionType kind)
            {
                ulong identity = itemType.GetIdentity();");

            foreach (var t in VariantTypes)
            {
                sb.AppendLine($"        if (identity == SerializedType.{t.Type}.GetIdentity())");
                sb.AppendLine("        {");
                sb.AppendLine("            if (kind == CollectionType.Array) return VariantArrayToArray_" + t.Name + "(collection, values);");
                sb.AppendLine("            else if (kind == CollectionType.List) return VariantArrayToList_" + t.Name + "(collection, values);");
                sb.AppendLine("            else if (kind == CollectionType.Queue) return VariantArrayToQueue_" + t.Name + "(collection, values);");
                sb.AppendLine("            else if (kind == CollectionType.Stack) return VariantArrayToStack_" + t.Name + "(collection, values);");
                sb.AppendLine("            else if (kind == CollectionType.HashSet) return VariantArrayToHashSet_" + t.Name + "(collection, values);");
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
            internal static object WriteDictionary(object collection, ICollection dictObj)
            {
                ulong keyId = keyType.GetIdentity();
                ulong valueId = valueType.GetIdentity();");

            foreach (var key in VariantTypes)
            {
                sb.AppendLine($"        if (keyId == SerializedType.{key.Type}.GetIdentity())");
                sb.AppendLine("        {");
                foreach (var value in VariantTypes)
                {
                    sb.AppendLine($"            if (valueId == SerializedType.{value.Type}.GetIdentity()) return VariantArrayToDictionary_{key.Name}_{value.Name}(collection, keys, values);");
                }
                sb.AppendLine("        }");
            }

            sb.AppendLine(@"
                throw new NotSupportedException();
            }");

            return ParseWriter(sb.ToString());
        }

        private static MemberDeclarationSyntax GenerateArrayWriter(VariantTypeInfo t)
        {
            return ParseWriter($@"
            private static object VariantArrayToArray_{t.Name}(object collection, VariantIRValue[] values)
            {{
                var array = ({t.ClrType}[])ReflectionUtils.EnsureCount(collection, values.Length);
                for (int i = 0; i < values.Length; i++)
                    array[i] = values[i].Payload.{t.PayloadField};
                return array;
            }}");
        }

        private static MemberDeclarationSyntax GenerateListWriter(VariantTypeInfo t)
        {
            return ParseWriter($@"
            private static object VariantArrayToList_{t.Name}(object collection, VariantIRValue[] values)
            {{
                var list = (List<{t.ClrType}>)collection;
                System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
                for (int i = 0; i < values.Length; i++)
                    list[i] = values[i].Payload.{t.PayloadField};
                return list;
            }}");
        }

        private static MemberDeclarationSyntax GenerateQueueWriter(VariantTypeInfo t)
        {
            return ParseWriter($@"
            private static object VariantArrayToQueue_{t.Name}(object collection, VariantIRValue[] values)
            {{
                var queue = (Queue<{t.ClrType}>)collection;
                queue.Clear();
                for (int i = 0; i < values.Length; i++)
                    queue.Enqueue(values[i].Payload.{t.PayloadField});
                return queue;
            }}");
        }

        private static MemberDeclarationSyntax GenerateStackWriter(VariantTypeInfo t)
        {
            return ParseWriter($@"
            private static object VariantArrayToStack_{t.Name}(object collection, VariantIRValue[] values)
            {{
                var stack = (Stack<{t.ClrType}>)collection;
                stack.Clear();
                for (int i = values.Length - 1; i >= 0; i--)
                    stack.Push(values[i].Payload.{t.PayloadField});
                return stack;
            }}");
        }

        private static MemberDeclarationSyntax GenerateHashSetWriter(VariantTypeInfo t)
        {
            return ParseWriter($@"
            private static object VariantArrayToHashSet_{t.Name}(object collection, VariantIRValue[] values)
            {{
                var set = (HashSet<{t.ClrType}>)collection;
                set.Clear();
                for (int i = 0; i < values.Length; i++)
                    set.Add(values[i].Payload.{t.PayloadField});
                return set;
            }}");
        }

        private static MemberDeclarationSyntax GenerateDictionaryWriter(VariantTypeInfo key, VariantTypeInfo value)
        {
            return ParseWriter($@"
            private static object VariantArrayToDictionary_{key.Name}_{value.Name}(object collection, VariantIRValue[] keys, VariantIRValue[] values)
            {{
                var dict = (Dictionary<{key.ClrType}, {value.ClrType}>)collection;
                dict.Clear();
                for (int i = 0; i < keys.Length; i++)
                    dict.Add(keys[i].Payload.{key.PayloadField}, values[i].Payload.{value.PayloadField});
                return dict;
            }}");
        }

        private static MemberDeclarationSyntax ParseWriter(string src)
        {
            return SyntaxFactory.ParseMemberDeclaration(src)!;
        }
    }
}
