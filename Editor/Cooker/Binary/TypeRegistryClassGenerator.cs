using Editor.Serialization;
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
                return x.Contains(y) || y.Contains(x);
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

        private static readonly HashSet<string> _exclusionList = new(new ContainsAComparer())
        {
            { "Box2D" },
            { "ldtk" },
            { "GLFW" },
            { "Engine.Graphics.OpenGL" },
            { "OpenGL" },
            { "JetBrains" },
            { "Generated" },
            { "Engine.Serialization" }
        };

        private static HashSet<string> _typesLibrary = new();
        internal static string Generate(bool isEditMode = false)
        {
            if (_typesLibrary.Count == 0)
            {
                Console.WriteLine("Can't generate TypeRegistry, no types were added to the list.");
                return string.Empty;
            }

            var ids = new Dictionary<Guid, string>();

            foreach (var typeFullName in _typesLibrary)
            {
                if (typeFullName.TrimStart().StartsWith("<") ||
                    _exclusionList.Contains(typeFullName))
                {
                    continue;
                }

                var id = ReflectionUtils.GetStableGuid(typeFullName);
                if (!ids.ContainsKey(id))
                {
                    ids.Add(id, typeFullName);
                }
            }

            return GenerateTypeRegistry(ids, isEditMode).ToFullString();
        }

        internal static void AddTypes(params Type[] types)
        {
            if (types == null || types.Length == 0)
                return;

            foreach (var type in types)
            {
                if (type.IsSpecialName)
                    continue;

                _typesLibrary.Add(ReflectionUtils.GetFullTypeName(type));
            }
        }
        internal static void AddTypeFullName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return;

            _typesLibrary.Add(typeName);
        }

        internal static void AddTypesFullNames(IList<string> typesNames)
        {
            if (typesNames == null || typesNames.Count == 0)
                return;

            foreach (var name in typesNames)
            {
                _typesLibrary.Add(name);
            }
        }
        internal static bool AddType(Type type)
        {
            if (type == null)
            {
                Console.WriteLine("Can't add null type to registry generator");
                return false;
            }
            return _typesLibrary.Add(ReflectionUtils.GetFullTypeName(type));
        }

        /// <summary>
        /// Clear all types that will be generated.
        /// </summary>
        internal static void ClearTypesLibrary()
        {
            _typesLibrary.Clear();
        }

        private static CompilationUnitSyntax GenerateTypeRegistry(Dictionary<Guid, string> typeNamesMap, bool isEditMode)
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
            foreach (var kvp in typeNamesMap)
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
            foreach (var kvp in typeNamesMap)
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



            // Build class and implement ITypeRegistry
            var classDecl = SyntaxFactory.ClassDeclaration("TypeRegistryRuntime")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                              SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .WithLeadingTrivia(SyntaxFactory.Comment(
                    $"/* This class was generated by a tool. Please do not edit manually.\n" +
                    $"Info:\n" +
                    $"Types Count: {_typesLibrary.Count} \n*/"), SyntaxFactory.ElasticCarriageReturnLineFeed)
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

                classDecl = classDecl.AddMembers(dictionaryReverseField, getIDMethod);
            }

            classDecl = classDecl.AddMembers(GetResolveTypeMethod(), 
                                             GenerateGetTypeMethod(), 
                                             GenerateResolveAssemblyMethod(),
                                             GenerateGetApplicationTypeMethod());

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
                try
                {
                    return Type.GetType(typeStr, ResolveAssembly, null, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(""[ERROR]:"" + e.ToString());
                    return null;
                }
            }
            ";

            return SyntaxFactory.ParseMemberDeclaration(methodSource)!;
        }

        private static MemberDeclarationSyntax GetResolveTypeMethod()
        {
            const string methodSrc = @"
            internal static bool ResolveType(Guid id, out Type type)
            {
                type = null;
                try
                {
                    return _types.TryGetValue(id, out type);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return false;
                }
            }";

            return SyntaxFactory.ParseMemberDeclaration(methodSrc)!;
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

        private static MemberDeclarationSyntax GenerateGetApplicationTypeMethod()
        {
            var name = ReflectionUtils.GetFullTypeName(LaminarTypeRegistryEditor.GameAppType);

            string methodSource = $@"
            internal static Type GetApplicationLayerType()
            {{
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var item in assemblies)
                {{
                    Console.WriteLine(""Assembly: "" + item.FullName);
                }}

                return _GetType(""{name}"");
            }}
            ";
            return SyntaxFactory.ParseMemberDeclaration(methodSource)!;
        }

        private static ExpressionSyntax GetTypeExpression(string typeFullname)
        {
            if (typeFullname.StartsWith("<") && typeFullname.Contains(">"))
                return null;

            return SyntaxFactory.ParseExpression($"_GetType(\"{typeFullname}\")");
        }

        //private static string GetReflectionFullName(Type type)
        //{
        //    string name;

        //    if (type.IsGenericTypeDefinition)
        //    {
        //        int arity = type.GetGenericArguments().Length;
        //        name = type.Name.Split('`')[0] + "`" + arity;
        //    }
        //    else
        //    {
        //        name = type.Name;
        //    }

        //    if (type.IsNested)
        //    {
        //        return GetReflectionFullName(type.DeclaringType!) + "+" + name;
        //    }

        //    return (type.Namespace != null ? type.Namespace + "." : "") + name;
        //}

        //private static string GetCSharpFullName(Type type)
        //{
        //    string name;

        //    if (type.IsGenericTypeDefinition)
        //    {
        //        int arity = type.GetGenericArguments().Length;
        //        string commas = new string(',', Math.Max(0, arity - 1));
        //        name = type.Name.Split('`')[0] + $"<{commas}>";
        //    }
        //    else
        //    {
        //        name = type.Name;
        //    }

        //    if (type.IsNested)
        //    {
        //        return GetCSharpFullName(type.DeclaringType!) + "." + name;
        //    }

        //    return (type.Namespace != null ? type.Namespace + "." : "") + name;
        //}


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
