using Editor.Serialization;
using Engine;
using Engine.Serialization;
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

        private const string REGISTRY_CLASS_NAME = "TypeRegistryRuntime";
        private const string DICTIONARY_CACHED_TYPES_NAME = "_typesCache";
        private const string DICTIONARY_TYPES_STR_NAME = "_typesStr";
        private const string ASSEMBLIES_BY_NAME_CACHE = "_assembliesByName";

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
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Runtime.CompilerServices")),
            ]);

            if (isEditMode)
            {
                usings = usings.Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Editor.Serialization")));
            }

            var kvpCode = new StringBuilder();

            foreach (var kvp in typeNamesMap)
            {
                SerializableGuid abGuid = kvp.Key;
                kvpCode.AppendLine($"{{ ToGuid(0x{abGuid.A:X16}, 0x{abGuid.B:X16}), \"{kvp.Value}\" }},");
            }

            string fieldCode = $@"private static readonly Dictionary<Guid, string> {DICTIONARY_TYPES_STR_NAME};";
            var dictionaryStrTypesField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(fieldCode);

            string ctorCode = $@"
            static {REGISTRY_CLASS_NAME}()
            {{
                PreloadAssemblies();
    
                {DICTIONARY_TYPES_STR_NAME} = new()
                {{
                    {kvpCode}
                }};
            }}
            ";

            var staticCtor = (ConstructorDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(ctorCode);

            string dictionaryCacheCode = $"private static readonly Dictionary<Guid, Type> {DICTIONARY_CACHED_TYPES_NAME} = new();";

            string assembliesByNameCacheCode = $"private static Dictionary<string, Assembly> {ASSEMBLIES_BY_NAME_CACHE} = new();";
            var assembliesByNameCache = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(assembliesByNameCacheCode);

            var dictionaryCachedTypesField = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(dictionaryCacheCode);

            // Build class and implement ITypeRegistry
            var classDecl = SyntaxFactory.ClassDeclaration(REGISTRY_CLASS_NAME)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                              SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .WithLeadingTrivia(SyntaxFactory.Comment(
                    $"/* This class was generated by a tool. Please do not edit manually.\n" +
                    $"Info:\n" +
                    $"Types Count: {_typesLibrary.Count} \n*/"), SyntaxFactory.ElasticCarriageReturnLineFeed)
                .AddMembers(dictionaryCachedTypesField, dictionaryStrTypesField, assembliesByNameCache, staticCtor);

            classDecl = classDecl.AddMembers(GetResolveTypeMethod(),
                                             GenerateGetTypeMethod(),
                                             GenerateResolveAssemblyMethod(),
                                             GenerateGetApplicationTypeMethod(),
                                             PreloadAssemblies(),
                                             GetGuidConverterMethod());

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
            const string methodSource = $@"
            private static Type SearchType(Guid id)
            {{
                if({DICTIONARY_TYPES_STR_NAME}.TryGetValue(id, out var typeStr))
                {{
                    return Type.GetType(typeStr, ResolveAssembly, null, true);
                }}
                
                Console.WriteLine($""[ERROR]: Cannot find type {{id}}"");

                return null;
            }}
            ";

            return SyntaxFactory.ParseMemberDeclaration(methodSource)!;
        }

        private static MemberDeclarationSyntax GetResolveTypeMethod()
        {
            const string methodSrc = $@"
            internal static bool ResolveType(Guid id, out Type type)
            {{
                type = null;

                if(id == Guid.Empty)
                {{
                    Console.WriteLine(""TypeId is empty"");             
                    return false;
                }}
            
                if({DICTIONARY_CACHED_TYPES_NAME}.TryGetValue(id, out type))
                {{
                    return type != null;
                }}
                
                type = SearchType(id);
                {DICTIONARY_CACHED_TYPES_NAME}.Add(id, type);
                return type != null;
            }}";

            return SyntaxFactory.ParseMemberDeclaration(methodSrc)!;
        }
        private static MemberDeclarationSyntax GenerateResolveAssemblyMethod()
        {
            const string methodSource = $@"
            private static Assembly ResolveAssembly(AssemblyName name)
            {{
                if({ASSEMBLIES_BY_NAME_CACHE}.TryGetValue(name.Name!, out var assembly))
                {{
                    return assembly;
                }}
                
                Console.WriteLine($""Assembly: '{{name.Name}}' wasn't found."");             

                return null;
            }}
            ";

            return SyntaxFactory.ParseMemberDeclaration(methodSource)!;
        }

        private static MemberDeclarationSyntax GenerateGetApplicationTypeMethod()
        {
            var name = ReflectionUtils.GetFullTypeName(LaminarTypeRegistryEditor.GameAppType);

            string methodSource = $@"
            internal static Type GetApplicationLayerType()
            {{
                return Type.GetType(""{name}"", ResolveAssembly, null, true);
            }}
            ";
            return SyntaxFactory.ParseMemberDeclaration(methodSource)!;
        }
        private static IEnumerable<string> GetRefsOnly(Assembly asm, bool recursive)
        {
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            IEnumerable<string> Walk(Assembly a)
            {
                foreach (var r in a.GetReferencedAssemblies())
                {
                    if (string.IsNullOrWhiteSpace(r.Name))
                    {
                        continue;
                    }

                    if (!visited.Add(r.Name))
                    {
                        continue;
                    }

                    yield return r.Name;

                    if (recursive)
                    {
                        Assembly? loaded = null;

                        try
                        {
                            loaded = Assembly.Load(r);
                        }
                        catch
                        {
                        }

                        if (loaded != null)
                        {
                            foreach (var child in Walk(loaded))
                            {
                                yield return child;
                            }
                        }
                    }
                }
            }

            return Walk(asm);
        }
        private static MemberDeclarationSyntax PreloadAssemblies()
        {
            var loadedAssemblies = new[]
            { 
                LaminarTypeRegistryEditor.GameAssembly.GetName().Name
            };
            //.Concat(GetRefsOnly(LaminarTypeRegistryEditor.GameAssembly, false))
            //.Where(n => !string.IsNullOrWhiteSpace(n))
            //.Distinct()
            //.OrderBy(n => n)
            //.ToArray();

            var bakedLoads = string.Join("\n", loadedAssemblies.Select(name =>
                    $@"        try
            {{
                Assembly.Load(""{name}"");
            }}
            catch
            {{
            }}"));

            var methodSource = $@"
            private static void PreloadAssemblies()
            {{
            {bakedLoads}

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var item in assemblies)
                {{
                    {ASSEMBLIES_BY_NAME_CACHE}.Add(item.GetName().Name, item);

            #if DEBUG
                    Console.WriteLine(""Assembly: "" + item.FullName);
            #endif
                }}
            }}
            ";

            return SyntaxFactory.ParseMemberDeclaration(methodSource)!;
        }

        private static MemberDeclarationSyntax GetGuidConverterMethod()
        {
            string methodSource = @"
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static Guid ToGuid(ulong a, ulong b)
            {
                return new Guid((int)(a & 0xFFFFFFFF),
                                (short)((a >> 32) & 0xFFFF),
                                (short)((a >> 48) & 0xFFFF),
                                (byte)(b & 0xFF),
                                (byte)((b >> 8) & 0xFF),
                                (byte)((b >> 16) & 0xFF),
                                (byte)((b >> 24) & 0xFF),
                                (byte)((b >> 32) & 0xFF),
                                (byte)((b >> 40) & 0xFF),
                                (byte)((b >> 48) & 0xFF),
                                (byte)((b >> 56) & 0xFF));
            }
            ";
            return SyntaxFactory.ParseMemberDeclaration(methodSource)!;
        }
        private static ExpressionSyntax GetTypeExpression(string typeFullname)
        {
            if (typeFullname.StartsWith("<") && typeFullname.Contains(">"))
                return null;

            return SyntaxFactory.ParseExpression($"_GetType(\"{typeFullname}\")");
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
