using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    internal class TypeIR
    {
        public int Version = 1;
        public SerializedPropertyIR[] Properties { get; set; }
    }
}
