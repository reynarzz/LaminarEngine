using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharedTypes
{
    internal static class ReflectionUtilsShared
    {
        public static string GetFullTypeName(Type type)
        {
            if (type == null)
                return string.Empty;

            var str = StripAssemblyMetadata(type.AssemblyQualifiedName);


            return str;
        }

        public static string StripAssemblyMetadata(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return typeName;

            return Regex.Replace(typeName, @",\s*Version=[^,\]]+|\s*,\s*Culture=[^,\]]+|\s*,\s*PublicKeyToken=[^,\]]+", string.Empty);
        }

        public static Guid GetStableGuid(Type type)
        {
            if (type == null)
            {
                return Guid.Empty;
            }

            // Deterministic GUID based on type full name
            var key = GetFullTypeName(type);
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            return new Guid(hash);
        }
    }
}
