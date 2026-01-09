using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    internal class SerializedCollectionElement
    {
        public SerializableType Type { get; set; }
    }

    internal class SerializedCollectionElement<T> : SerializedCollectionElement
    {
        public T Value { get; set; }
    }
}
