using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Editor.Cooker.Generator
{
    public class TypeGenerationStage
    {
        public static async Task GenerateTypeRegistry()
        {
            var engineBin = Path.Combine(EditorPaths.AppRoot, "Engine", "bin");
            var engineTypes = GetAssemblyTypes(GetAssemblyPath(engineBin, "Engine.dll"));

            var gameTypes = await ReadProjectTypes(EditorPaths.GameProjectAbsolutePath);

            TypeRegistryClassGenerator.AddTypes(engineTypes);
            TypeRegistryClassGenerator.AddTypesFullNames(gameTypes);

            var str = TypeRegistryClassGenerator.Generate();

            File.WriteAllText($"{EditorPaths.AppRoot}/Generated/TypeRegistry_Generated.cs", str);

        }

        private static string GetAssemblyPath(string root, string name)
        {
            var files = Directory.GetFiles(root, "*.dll", SearchOption.AllDirectories);

            return files.Where(x => x.EndsWith(name)).FirstOrDefault();
        }

        private static Type[] GetAssemblyTypes(string assemblyPath)
        {
            var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

            var refs = Directory.GetFiles(runtimeDir, "*.dll");

            var resolver = new PathAssemblyResolver(refs.Append(assemblyPath));

            var mlc = new MetadataLoadContext(resolver);

            var asm = mlc.LoadFromAssemblyPath(assemblyPath);

            return asm.DefinedTypes.Select(t => t.AsType()).ToArray();
        }

        private static async Task<List<string>> ReadProjectTypes(string projectFullPath)
        {
            var workspace = MSBuildWorkspace.Create();
            var project = await workspace.OpenProjectAsync(projectFullPath);

            var compilation = await project.GetCompilationAsync();

            var typesNames = new List<string>();

            foreach (var tree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(tree);
                var root = await tree.GetRootAsync();

                var typeDeclarations = root.DescendantNodes().Where(x => x is TypeDeclarationSyntax ||
                                                                         x is EnumDeclarationSyntax ||
                                                                         x is RecordDeclarationSyntax);
                foreach (var typeDecl in typeDeclarations)
                {
                    var symbol = model.GetDeclaredSymbol(typeDecl);
                    var name = GetClrTypeName(symbol);
                    typesNames.Add(name);
                    Console.WriteLine(name);
                }
            }

            return typesNames;
        }

        private static string GetClrTypeName(ISymbol symbol)
        {
            var parts = new Stack<string>();

            var current = symbol;

            while (current != null)
            {
                parts.Push(current.MetadataName);
                current = current.ContainingType;
            }

            var typeName = string.Join("+", parts);

            var ns = symbol.ContainingNamespace?.ToDisplayString();

            if (!string.IsNullOrEmpty(ns))
            {
                typeName = ns + "." + typeName;
            }

            return $"{typeName}, {symbol.ContainingAssembly.Name}";
        }
    }
}
