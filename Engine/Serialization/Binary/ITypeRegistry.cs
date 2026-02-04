using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    internal interface ITypeRegistry
    {
        Type GetType(Guid id);
        Guid GetID(Type type);
    }
}