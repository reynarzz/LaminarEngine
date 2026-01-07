using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class SceneAsset : AssetResourceBase
    {
        internal List<ActorDataSceneAsset> Actors { get; private set; } = new();

        internal SceneAsset(string path, Guid guid) : base(path, guid)
        {
        }
    }
}