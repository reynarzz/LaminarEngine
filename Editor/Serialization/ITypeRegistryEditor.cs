using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    internal interface ITypeRegistryEditor : ITypeRegistry
    {
        internal bool GetID(Type type, out Guid id);
    }
}
