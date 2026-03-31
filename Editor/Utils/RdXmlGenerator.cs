using System;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

namespace Editor
{
    internal static class RdXmlGenerator
    {
        public static string Generate(params Assembly[] assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            var sb = new StringBuilder();

            sb.AppendLine("<linker>");

            foreach (var asm in assemblies.Where(x => x != null).Distinct().OrderBy(x => x.GetName().Name))
            {
                string asmName = asm.GetName().Name;

                if (string.IsNullOrWhiteSpace(asmName))
                    continue;

                sb.AppendLine($"  <assembly fullname=\"{SecurityElement.Escape(asmName)}\">");
                sb.AppendLine("    <type fullname=\"*\" preserve=\"all\" />");
                sb.AppendLine("  </assembly>");
            }

            sb.AppendLine("</linker>");

            return sb.ToString();
        }
    }
}