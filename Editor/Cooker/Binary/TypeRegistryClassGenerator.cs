using Engine;
using Engine.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Editor.Cooker
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
            { "Box2D" },
            { "ldtk" },
            { "GLFW" },
            { "Engine.Graphics.OpenGL" },
            { "OpenGL" },
            { "JetBrains" },
            { "Generated" },
        };

        private static HashSet<Type> _typesLibrary = new();
        internal static string Generate(bool isEditMode = false)
        {
            if (_typesLibrary.Count == 0)
            {
                Console.WriteLine("Can't generate TypeRegistry, no types were added to the list.");
                return string.Empty;
            }

            var ids = new Dictionary<Guid, Type>();

            foreach (var type in _typesLibrary)
            {
                if (type.IsSpecialName ||
                    type.FullName.TrimStart().StartsWith("<") ||
                    _forbiddenNameSpaces.Contains(type.FullName))
                {
                    continue;
                }

                ids.Add(ReflectionUtils.GetStableGuid(type), type);
            }

            return GenerateTypeRegistry(ids, isEditMode).ToFullString();
        }

        internal static bool ContainsType(Type type)
        {
            return _typesLibrary.Contains(type);
        }

        internal static void AddTypes(params Type[] types)
        {
            if (types == null || types.Length == 0)
                return;

            foreach (var type in types)
            {
                _typesLibrary.Add(type);
            }
        }
        internal static void AddType(Type type)
        {
            if (type == null)
            {
                Console.WriteLine("Can't add null type to registry generator");
                return;
            }
            _typesLibrary.Add(type);
        }

        /// <summary>
        /// Clear all types that will be generated.
        /// </summary>
        internal static void ClearTypesLibrary()
        {
            _typesLibrary.Clear();
        }

        private static CompilationUnitSyntax GenerateTypeRegistry(Dictionary<Guid, Type> typeMap, bool isEditMode)
        {
            // Using statements
            var usings = SyntaxFactory.List(
            [
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reflection")),
            ]);

            if (isEditMode)
            {
                usings = usings.Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Editor.Serialization")));
            }

            // Build dictionary entries: <Guid, Type>
            var initializerExpressions = new SeparatedSyntaxList<ExpressionSyntax>();
            foreach (var kvp in typeMap)
            {
                var guidLiteral = SyntaxFactory.ParseExpression($"new Guid(\"{kvp.Key:N}\")");
                var typeOfExpression = GetTypeExpression(kvp.Value);

                if (typeOfExpression == null)
                {
                    continue;
                }
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
                if (typeOfExpression == null)
                {
                    continue;
                }
                var guidLiteral = SyntaxFactory.ParseExpression($"new Guid(\"{kvp.Key:N}\")");

                var kvpExpression = SyntaxFactory.InitializerExpression(
                    SyntaxKind.ComplexElementInitializerExpression,
                    SyntaxFactory.SeparatedList<ExpressionSyntax>(new[] { typeOfExpression, guidLiteral })
                );

                reverseInitializerExpressions = reverseInitializerExpressions.Add(kvpExpression);
            }

            // Build GetType(Guid id, out Type type) method
            var getTypeMethod = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    "ResolveType"
                )
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                              SyntaxFactory.Token(SyntaxKind.StaticKeyword))
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
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                              SyntaxFactory.Token(SyntaxKind.StaticKeyword))
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
            var classDecl = SyntaxFactory.ClassDeclaration("TypeRegistryRuntime")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                              SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                //.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(isEditMode ? "ITypeRegistryEditor" : "ITypeRegistry")))
                .WithLeadingTrivia(SyntaxFactory.Comment("// This class was generated by a tool. Please do not edit manually."))
                .AddMembers(dictionaryField);


            if (isEditMode)
            {
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

                classDecl = classDecl.AddMembers(dictionaryReverseField, getIDMethod);
            }

            classDecl = classDecl.AddMembers(getTypeMethod, GenerateGetTypeMethod(), GenerateResolveAssemblyMethod());

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

        private static MemberDeclarationSyntax GenerateGetTypeMethod()
        {
            const string methodSource = @"
            private static Type _GetType(string typeStr)
            {
                return Type.GetType(typeStr, ResolveAssembly, null, true);
            }
            ";

            return SyntaxFactory.ParseMemberDeclaration(methodSource)!;
        }
        private static MemberDeclarationSyntax GenerateResolveAssemblyMethod()
        {
            const string methodSource = @"
            private static Assembly ResolveAssembly(AssemblyName name)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    var asm = assemblies[i];
                    if (AssemblyName.ReferenceMatchesDefinition(asm.GetName(), name))
                    {
                        return asm;
                    }
                }
                return null;
            }
            ";

            return SyntaxFactory.ParseMemberDeclaration(methodSource)!;
        }

        private static ExpressionSyntax GetTypeExpression(Type type)
        {
            // Only private or protected nested types
            // if (type.IsNestedPrivate || type.IsNestedFamily || type.IsGenericType)
            {
                // Compiler generated types are not taken into account.
                if (type.Name.StartsWith("<") && type.Name.Contains(">"))
                    return null;

                string typeString = ReflectionUtils.GetFullTypeName(type);// GetReflectionFullName(type) + ", " + type.Assembly.GetName().Name;
                //return SyntaxFactory.ParseExpression($"Type.GetType(\"{typeString}\", throwOnError: true)");

                return SyntaxFactory.ParseExpression($"_GetType(\"{typeString}\")");
            }

            // Public or internal nested types, or top-level types
            string fullName = GetCSharpFullName(type);
            return SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName(fullName));
        }

        private static string GetReflectionFullName(Type type)
        {
            string name;

            if (type.IsGenericTypeDefinition)
            {
                int arity = type.GetGenericArguments().Length;
                name = type.Name.Split('`')[0] + "`" + arity;
            }
            else
            {
                name = type.Name;
            }

            if (type.IsNested)
            {
                return GetReflectionFullName(type.DeclaringType!) + "+" + name;
            }

            return (type.Namespace != null ? type.Namespace + "." : "") + name;
        }

        private static string GetCSharpFullName(Type type)
        {
            string name;

            if (type.IsGenericTypeDefinition)
            {
                int arity = type.GetGenericArguments().Length;
                string commas = new string(',', Math.Max(0, arity - 1));
                name = type.Name.Split('`')[0] + $"<{commas}>";
            }
            else
            {
                name = type.Name;
            }

            if (type.IsNested)
            {
                return GetCSharpFullName(type.DeclaringType!) + "." + name;
            }

            return (type.Namespace != null ? type.Namespace + "." : "") + name;
        }


        private static MemberDeclarationSyntax WrapWithIf(MemberDeclarationSyntax member, string symbol)
        {
            var ifDirective = SyntaxFactory.Trivia(SyntaxFactory.IfDirectiveTrivia(
                    SyntaxFactory.IdentifierName(symbol), isActive: true, branchTaken: true, conditionValue: true));

            var endIfDirective = SyntaxFactory.Trivia(SyntaxFactory.EndIfDirectiveTrivia(isActive: true));

            return member.WithLeadingTrivia(SyntaxFactory.TriviaList(ifDirective, SyntaxFactory.ElasticCarriageReturnLineFeed))
                         .WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ElasticCarriageReturnLineFeed, endIfDirective));
        }
    }
}
