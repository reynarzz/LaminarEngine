using System;
using System.Linq;
using System.Reflection;
using Engine;
using Engine.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Editor
{
    public class SerializedFieldContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = new List<JsonProperty>(base.CreateProperties(type, memberSerialization));

            var existingNames = new HashSet<string>(props.Count);
            foreach (var p in props)
            {
                existingNames.Add(p.PropertyName);
            }

            foreach (var member in ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(type))
            {
                if (existingNames.Contains(member.Name))
                    continue;

                var prop = base.CreateProperty(member, memberSerialization);
                prop.Readable = true;
                prop.Writable = true;

                props.Add(prop);
                existingNames.Add(prop.PropertyName);
            }

            return props;
        }
    }
}