using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    internal class SerializedItem
    {
        public SerializedType Type { get; set; }
    }

    internal class SerializedItem<T> : SerializedItem
    {
        public T Data { get; set; }
    }
}