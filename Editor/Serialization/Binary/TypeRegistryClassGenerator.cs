using Engine;
using Engine.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            { nameof(OpenGL) },
            { nameof(JetBrains) },
            { "Generated" },
        };

        internal static string Generate(Assembly[] assemblies)
        {
            var ids = new Dictionary<Guid, Type>();

            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                {
                    Debug.Error("Can't generate type for null assembly.");
                    continue;
                }

                foreach (var type in assembly.DefinedTypes)
                {
                    // Skip compiler generated or internal types
                    if (type.IsNestedPrivate || type.IsSpecialName ||
                        type.FullName.TrimStart().StartsWith("<") ||
                        _forbiddenNameSpaces.Contains(type.FullName))
                    {
                        continue;
                    }

                    ids.Add(StableGuid(type.AsType()), type.AsType());
                }
            }

            return GenerateTypeRegistry(ids).ToFullString();
        }

        private static CompilationUnitSyntax GenerateTypeRegistry(Dictionary<Guid, Type> typeMap)
        {
            // Using statements
            var usings = SyntaxFactory.List(
            [
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Engine.Serialization")),
            ]);

            // Build dictionary entries: <Guid, Type>
            var initializerExpressions = new SeparatedSyntaxList<ExpressionSyntax>();
            foreach (var kvp in typeMap)
            {
                var guidLiteral = SyntaxFactory.ParseExpression($"new Guid(\"{kvp.Key:N}\")");
                var typeOfExpression = GetTypeExpression(kvp.Value);

                var kvpExpression = SyntaxFactory.InitializerExpression(
                    SyntaxKind.ComplexElementInitializerExpression,
                    SyntaxFactory.SeparatedList<ExpressionSyntax>(new[] { guidLiteral, typeOfExpression })
                );

                initializerExpressions = initializerExpressions.Add(kvpExpression);
            }

            var dictionaryField = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName("Dictionary<Guid, Type>"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator("_types")
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
            ).AddModifiers(
                SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
            );

            // Build reverse dictionary: <Type, Guid>
            var reverseInitializerExpressions = new SeparatedSyntaxList<ExpressionSyntax>();
            foreach (var kvp in typeMap)
            {
                var typeOfExpression = GetTypeExpression(kvp.Value);
                var guidLiteral = SyntaxFactory.ParseExpression($"new Guid(\"{kvp.Key:N}\")");

                var kvpExpression = SyntaxFactory.InitializerExpression(
                    SyntaxKind.ComplexElementInitializerExpression,
                    SyntaxFactory.SeparatedList<ExpressionSyntax>(new[] { typeOfExpression, guidLiteral })
                );

                reverseInitializerExpressions = reverseInitializerExpressions.Add(kvpExpression);
            }

            var dictionaryReverseField = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName("Dictionary<Type, Guid>"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator("_typesReverse")
                                         .WithInitializer(
                                             SyntaxFactory.EqualsValueClause(
                                                 SyntaxFactory.ObjectCreationExpression(
                                                     SyntaxFactory.ParseTypeName("Dictionary<Type, Guid>"))
                                                 .WithInitializer(
                                                     SyntaxFactory.InitializerExpression(
                                                         SyntaxKind.CollectionInitializerExpression,
                                                         reverseInitializerExpressions
                                                     )
                                                 )
                                             )
                                         )
                        )
                    )
            ).AddModifiers(
                SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
            );

            // Build GetType(Guid id, out Type type) method
            var getTypeMethod = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    "GetType"
                )
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("id"))
                                 .WithType(SyntaxFactory.ParseTypeName("Guid")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("type"))
                                 .WithType(SyntaxFactory.ParseTypeName("Type"))
                                 .WithModifiers(
                                     SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword))
                                 )
                )
                .WithBody(SyntaxFactory.Block(
                    SyntaxFactory.ParseStatement("return _types.TryGetValue(id, out type);")
                ));

            // Build GetID(Type type, out Guid id) method
            var getIDMethod = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    "GetID"
                )
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("type"))
                                 .WithType(SyntaxFactory.ParseTypeName("Type")),
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("id"))
                                 .WithType(SyntaxFactory.ParseTypeName("Guid"))
                                 .WithModifiers(
                                     SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword))
                                 )
                )
                .WithBody(SyntaxFactory.Block(
                    SyntaxFactory.ParseStatement("return _typesReverse.TryGetValue(type, out id);")
                ));

            // Build class and implement ITypeRegistry
            var classDecl = SyntaxFactory.ClassDeclaration("TypeRegistry")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword))
                .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("ITypeRegistry")))
                .WithLeadingTrivia(SyntaxFactory.Comment("// This is the generated TypeRegistry class. Do not edit manually."))
                .AddMembers(dictionaryField, dictionaryReverseField, getTypeMethod, getIDMethod);

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

        private static ExpressionSyntax GetTypeExpression(Type type)
        {
            if (type.IsNested && !type.IsNestedPublic)
            {
                // Private nested type: generate reflection call
                var declaringFullName = GetCSharpFullName(type.DeclaringType);
                return SyntaxFactory.ParseExpression(
                    $"typeof({declaringFullName}).GetNestedType(\"{GetCSharpFullName(type)}\", System.Reflection.BindingFlags.NonPublic)"
                );
            }

            // Public type or accessible nested type
            string fullName = GetCSharpFullName(type);
            return SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName(fullName));
        }

        private static string GetCSharpFullName(Type type)
        {
            if (type.IsGenericTypeDefinition)
            {
                // Open generic: build commas for arity
                int arity = type.GetGenericArguments().Length;
                var commas = new string(',', Math.Max(0, arity - 1));
                var name = type.Name.Split('`')[0];

                if (type.IsNested)
                {
                    return GetCSharpFullName(type.DeclaringType!) + "." + name + $"<{commas}>";
                }
                else
                {
                    return (type.Namespace != null ? type.Namespace + "." : "") + name + $"<{commas}>";
                }
            }
            else if (type.IsNested)
            {
                // Nested type without generics
                return GetCSharpFullName(type.DeclaringType!) + "." + type.Name;
            }
            else
            {
                // Normal type
                return (type.Namespace != null ? type.Namespace + "." : "") + type.Name;
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
