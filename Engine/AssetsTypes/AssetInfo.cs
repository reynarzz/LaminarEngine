using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct AssetInfo
    {
        public AssetType Type;
        public DateTime LastWriteTime;
        public string Path;
        public bool IsCompressed;
        public bool IsEncrypted;
        public DateTime MetaWriteTime;
    }

}
