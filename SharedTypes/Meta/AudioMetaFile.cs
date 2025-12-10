using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypes
{
    [Serializable]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public struct AudioMetaFile : AssetMetaFileBase
    {
        public Guid GUID { get; set; }
    }
}