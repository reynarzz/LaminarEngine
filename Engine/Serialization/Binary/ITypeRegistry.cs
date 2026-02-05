using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    internal interface ITypeRegistry
    {
        internal bool GetType(Guid id, out Type type);
    }
}