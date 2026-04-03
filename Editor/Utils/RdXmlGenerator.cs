using System;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

namespace Editor
{
    internal static class RdXmlGenerator
    {
        public static string Generate(params string[] assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            var sb = new StringBuilder();

            sb.AppendLine("<linker>");

            foreach (var asm in assemblies)
            {
                string asmName = asm;

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