using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class DefaultMetaGenerator<T> : IAssetMetaGenerator where T: AssetMeta, new()
    {
        public AssetMeta GetDefaultMeta(BinaryReader reader)
        {
            return new T();
        }
    }
}
