using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypes
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public struct DefaultMetaFile : AssetMetaFileBase
    {
        public Guid GUID { get; set; }

        public DefaultMetaFile() { }
    }
}