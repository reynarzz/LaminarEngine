using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    internal class ShaderIR
    {
        public const int Version = 1;
        internal SerializedPropertyIR SourcesCollection { get; set; }
    }
}