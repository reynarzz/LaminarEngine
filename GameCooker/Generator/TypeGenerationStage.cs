using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace GameCooker.Generator
{
    public class TypeGenerationStage
    {
        public static void GenerateTypeRegistry()
        {

            var engineBin = Path.Combine(CookerPaths.AppRoot, "Engine", "bin");
            var engineTypes = GetAssemblyTypes(GetAssemblyPath(engineBin, "Engine.dll"));
            var gameTypes = GetAssemblyTypes(GetAssemblyPath(Path.Combine(CookerPaths.AppRoot, "Game", "bin"), "Game.dll"));
            var sharedTypes = GetAssemblyTypes(GetAssemblyPath(Path.Combine(CookerPaths.AppRoot, "SharedTypes", "bin"), "SharedTypes.dll"));

            TypeRegistryClassGenerator.AddTypes(engineTypes);
            TypeRegistryClassGenerator.AddTypes(gameTypes);
            TypeRegistryClassGenerator.AddTypes(sharedTypes);

            var str = TypeRegistryClassGenerator.Generate();

            File.WriteAllText($"{CookerPaths.AppRoot}/Generated/TypeRegistry_Generated.cs", str);
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
    }
}
