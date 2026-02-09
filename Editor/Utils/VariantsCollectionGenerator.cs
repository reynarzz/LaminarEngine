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
            string PayloadField
        );

        private static readonly VariantTypeInfo[] VariantTypes =
        {
            new("Char", "char", "Char"),
            new("Bool", "bool", "Bool"),
            new("Byte", "byte", "Byte"),
            new("Short", "short", "Short"),
            new("UShort", "ushort", "UShort"),
            new("Int", "int", "Int"),
            new("UInt", "uint", "Uint"),
            new("Long", "long", "Long"),
            new("ULong", "ulong", "Ulong"),
            new("Float", "float", "Float"),
            new("Double", "double", "Double"),

            new("Vec2", "vec2", "Vec2"),
            new("Vec3", "vec3", "Vec3"),
            new("Vec4", "vec4", "Vec4"),
            new("IVec2", "ivec2", "Ivec2"),
            new("IVec3", "ivec3", "Ivec3"),
            new("IVec4", "ivec4", "Ivec4"),
            new("Quat", "quat", "Quat"),
            new("Mat2", "mat2", "Mat2"),
            new("Mat3", "mat3", "Mat3"),
            new("Mat4", "mat4", "Mat4"),

            new("Color", "Color", "Color"),
            new("Color32", "Color32", "Color32")
        };

        internal static string Generate()
        {
            var cu = GenerateCompilationUnit();
            return cu.NormalizeWhitespace().ToFullString();
        }

        private static CompilationUnitSyntax GenerateCompilationUnit()
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

            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Generated")).AddMembers(classDecl);
            return SyntaxFactory.CompilationUnit().AddUsings(usings.ToArray()).AddMembers(ns);
        }

        private static MemberDeclarationSyntax GenerateDispatchMethod()
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"
internal static object Write(object collection, VariantIRValue[] values, SerializedType itemType, CollectionType kind)
{
    switch (itemType)
    {");
            foreach (var t in VariantTypes)
            {
                sb.AppendLine($"        case SerializedType.{t.Name}:");
                sb.AppendLine("            switch (kind)");
                sb.AppendLine("            {");

                foreach (var kind in new[] { "Array", "List", "Queue", "Stack", "HashSet" })
                {
                    sb.AppendLine(
                        $"                case CollectionType.{kind}:" +
                        $" return VariantArrayTo{kind}_{t.Name}(collection, values);");
                }

                sb.AppendLine("            }");
                sb.AppendLine("            break;");
            }

            sb.AppendLine(@"
                }

                throw new NotSupportedException();
            }");

            return SyntaxFactory.ParseMemberDeclaration(sb.ToString())!;
        }

        private static MemberDeclarationSyntax GenerateArrayWriter(VariantTypeInfo t)
        {
            return ParseWriter($@"
            private static object VariantArrayToArray_{t.Name}(object collection, VariantIRValue[] values)
            {{
                var array = ({t.ClrType}[])ReflectionUtils.EnsureCount(collection, values.Length);

                for (int i = 0; i < values.Length; i++)
                {{
                    array[i] = values[i].Payload.{t.PayloadField};
                }}

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
                Span<{t.ClrType}> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);

                for (int i = 0; i < values.Length; i++)
                {{
                    list[i] = values[i].Payload.{t.PayloadField};
                }}

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
                {{
                    queue.Enqueue(values[i].Payload.{t.PayloadField});
                }}

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
                {{
                    stack.Push(values[i].Payload.{t.PayloadField});
                }}

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
                {{
                    set.Add(values[i].Payload.{t.PayloadField});
                }}

                return set;
            }}");
        }

        private static MemberDeclarationSyntax ParseWriter(string src)
        {
            return SyntaxFactory.ParseMemberDeclaration(src)!;
        }
    }
}
