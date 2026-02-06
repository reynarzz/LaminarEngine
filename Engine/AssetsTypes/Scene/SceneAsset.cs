using Engine;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal class SceneAsset : AssetResourceBase
    {
        internal List<ActorIR> Actors { get; private set; } = new();

        internal SceneAsset(string path, Guid guid) : base(path, guid)
        {
        }

        internal override void UpdateResource(object data, string path, Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}