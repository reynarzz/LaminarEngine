using Engine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    internal static class EditorJsonUtils
    {
        private readonly static JsonSerializerSettings _jsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto, // Do not remove this.
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

        internal static T Deserialize<T>(string json, [CallerMemberName] string caller = "")
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    return default;
                }
                return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
            }
            catch (Exception e)
            {
                Debug.Warn($"Couldn't deserialize json, caller: {caller}()");
            }

            return default;
        }
    }
}
