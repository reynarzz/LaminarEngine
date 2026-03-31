using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace Editor
{
    internal static class RdXmlGenerator
    {
        public static string Generate(string[] typeStrings)
        {
            if (typeStrings == null)
                throw new ArgumentNullException(nameof(typeStrings));

            var assemblies = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < typeStrings.Length; i++)
            {
                string raw = typeStrings[i];
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                CollectTypes(raw.Trim(), assemblies);
            }

            var sb = new StringBuilder();

            sb.AppendLine("<linker>");

            foreach (var asm in assemblies.OrderBy(x => x.Key))
            {
                sb.AppendLine($"  <assembly fullname=\"{SecurityElement.Escape(asm.Key)}\">");

                foreach (var typeName in asm.Value.OrderBy(x => x))
                {
                    sb.AppendLine($"    <type fullname=\"{SecurityElement.Escape(typeName)}\" preserve=\"all\" />");
                }

                sb.AppendLine("  </assembly>");
            }

            sb.AppendLine("</linker>");

            return sb.ToString();
        }

        private static void CollectTypes(string entry, Dictionary<string, HashSet<string>> assemblies)
        {
            int commaIndex = FindTopLevelAssemblyComma(entry);
            if (commaIndex < 0)
                return;

            string typeName = entry.Substring(0, commaIndex).Trim();
            string assemblyName = entry.Substring(commaIndex + 1).Trim();

            if (string.IsNullOrWhiteSpace(typeName) || string.IsNullOrWhiteSpace(assemblyName))
                return;

            Register(assemblyName, typeName, assemblies);

            int genericStart = typeName.IndexOf("[[", StringComparison.Ordinal);
            if (genericStart < 0)
                return;

            string openGeneric = typeName.Substring(0, genericStart).Trim();
            if (!string.IsNullOrWhiteSpace(openGeneric))
            {
                Register(assemblyName, openGeneric, assemblies);
            }

            for (int i = genericStart; i < typeName.Length; i++)
            {
                if (i + 1 >= typeName.Length)
                    break;

                if (typeName[i] == '[' && typeName[i + 1] == '[')
                {
                    int end = FindMatchingDoubleBracketEnd(typeName, i + 2);
                    if (end < 0)
                        break;

                    string arg = typeName.Substring(i + 2, end - (i + 2)).Trim();
                    CollectTypes(arg, assemblies);

                    i = end + 1;
                }
            }
        }

        private static void Register(string assemblyName, string typeName, Dictionary<string, HashSet<string>> assemblies)
        {
            if (!assemblies.TryGetValue(assemblyName, out var types))
            {
                types = new HashSet<string>(StringComparer.Ordinal);
                assemblies[assemblyName] = types;
            }

            types.Add(typeName);
        }

        private static int FindTopLevelAssemblyComma(string entry)
        {
            int depth = 0;
            int lastComma = -1;

            for (int i = 0; i < entry.Length; i++)
            {
                char c = entry[i];

                if (c == '[')
                {
                    depth++;
                }
                else if (c == ']')
                {
                    depth--;
                }
                else if (c == ',' && depth == 0)
                {
                    lastComma = i;
                }
            }

            return lastComma;
        }

        private static int FindMatchingDoubleBracketEnd(string str, int startIndex)
        {
            int depth = 0;

            for (int i = startIndex; i < str.Length - 1; i++)
            {
                if (str[i] == '[' && str[i + 1] == '[')
                {
                    depth++;
                    i++;
                    continue;
                }

                if (str[i] == ']' && str[i + 1] == ']')
                {
                    if (depth == 0)
                    {
                        return i;
                    }

                    depth--;
                    i++;
                    continue;
                }
            }

            return -1;
        }


    }
}