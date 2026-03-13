using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal interface IAssetFileSaver
    {
        void Write(Guid refId, string relativePath);
    }
}
