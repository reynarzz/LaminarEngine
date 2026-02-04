using Engine.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Editor.Serialization
{
    internal class TypeRegistryClassGenerator
    {
        private class ContainsAComparer : IEqualityComparer<string>
        {
            public bool Equals(string? x, string? y)
            {
                if (x == null || y == null)
                {
                    return false;
                }
                return x.StartsWith(y) || y.StartsWith(x);
            }

            public int GetHashCode(string obj)
            {
                if (string.IsNullOrEmpty(obj))
                {
                    return 0;
                }
                var prefix = obj.Length <= 3 ? obj : obj.Substring(0, 3);
                return prefix.GetHashCode();
            }
        }

        private static readonly HashSet<string> _forbiddenNameSpaces = new(new ContainsAComparer())
        {
            { nameof(Box2D) },
            { nameof(ldtk) },
            { nameof(GLFW) },
            { nameof(Engine.Graphics.OpenGL) },
            { nameof(JetBrains) },
        };
        internal static string Generate(Assembly[] assemblies)
        {
            var ids = new Dictionary<Guid, Type>();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.DefinedTypes)
                {
                    // Skip compiler-generated or internal types
                    if (type.IsNestedPrivate || type.IsSpecialName || type.FullName.TrimStart().StartsWith("<") ||
                        _forbiddenNameSpaces.Contains(type.FullName))
                        continue;

                    ids.Add(StableGuid(type.AsType()), type.AsType());
                }
            }

            var compiledUnit = GenerateTypeRegistry(ids);
            return compiledUnit.ToFullString();
        }

        private static CompilationUnitSyntax GenerateTypeRegistry(Dictionary<Guid, Type> typeMap)
        {
            // Using statements
            var usings = SyntaxFactory.List(new UsingDirectiveSyntax[]
            {
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic"))
            });

            // Build dictionary entries
            var initializerExpressions = new SeparatedSyntaxList<ExpressionSyntax>();
            foreach (var kvp in typeMap)
            {
                var guidLiteral = SyntaxFactory.ParseExpression($"new Guid(\"{kvp.Key:N}\")");
                var typeOfExpression = SyntaxFactory.TypeOfExpression(GetTypeSyntax(kvp.Value));

                var kvpExpression = SyntaxFactory.InitializerExpression(
                    SyntaxKind.ComplexElementInitializerExpression,
                    SyntaxFactory.SeparatedList<ExpressionSyntax>(new ExpressionSyntax[] { guidLiteral, typeOfExpression })
                );

                initializerExpressions = initializerExpressions.Add(kvpExpression);
            }

            // Create dictionary field
            var dictionaryField = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName("Dictionary<Guid, Type>"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator("Types")
                                         .WithInitializer(
                                             SyntaxFactory.EqualsValueClause(
                                                 SyntaxFactory.ObjectCreationExpression(
                                                     SyntaxFactory.ParseTypeName("Dictionary<Guid, Type>"))
                                                 .WithInitializer(
                                                     SyntaxFactory.InitializerExpression(
                                                         SyntaxKind.CollectionInitializerExpression,
                                                         initializerExpressions
                                                     )
                                                 )
                                             )
                                         )
                        )
                    )
                ).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                               SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                               SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));

            // Build class
            var classDecl = SyntaxFactory.ClassDeclaration("TypeRegistry")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                              SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddMembers(dictionaryField);

            // Build namespace
            var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Generated"))
                                  .AddMembers(classDecl);

            // Build compilation unit
            var compilationUnit = SyntaxFactory.CompilationUnit()
                                               .AddUsings(usings.ToArray())
                                               .AddMembers(ns)
                                               .NormalizeWhitespace();

            return compilationUnit;
        }

        private static TypeSyntax GetTypeSyntax(Type type)
        {
            if (type.IsGenericTypeDefinition)
            {
                // Open generic e.g., PatrolState<>
                int arity = type.GetGenericArguments().Length;
                string commas = new string(',', Math.Max(0, arity - 1));
                string name = (type.Namespace != null ? type.Namespace + "." : "") + type.Name.Split('`')[0];
                return SyntaxFactory.ParseTypeName($"{name}<{commas}>");
            }
            else if (type.IsArray)
            {
                // Handle arrays
                var elementType = GetTypeSyntax(type.GetElementType()!);
                return SyntaxFactory.ArrayType(elementType,
                    SyntaxFactory.SingletonList(
                        SyntaxFactory.ArrayRankSpecifier(
                            SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                SyntaxFactory.OmittedArraySizeExpression()
                            )
                        )
                    )
                );
            }
            else if (type.IsNested)
            {
                // Nested type: Outer.Inner
                string fullName = type.FullName?.Replace('+', '.') ?? type.Name;
                return SyntaxFactory.ParseTypeName(fullName);
            }
            else
            {
                return SyntaxFactory.ParseTypeName(type.FullName ?? type.Name);
            }
        }

        private static Guid StableGuid(Type type)
        {
            // Deterministic GUID based on type full name
            string key = type.FullName ?? type.Name;
            using var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            return new Guid(hash);
        }
    }
}
