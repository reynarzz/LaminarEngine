using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    internal static class EditorJsonUtils
    {
        private readonly static JsonSerializerSettings _jsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Converters =
            {
                new StringEnumConverter(),
            },
            ContractResolver = new SerializedFieldContractResolver()
        };

        internal static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, _jsonSettings);
        }

        internal static T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
        }
    }
}
