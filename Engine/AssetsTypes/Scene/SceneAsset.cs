using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal class SceneAsset : AssetResourceBase
    {
        internal SceneIR SceneIR { get; private set; } = new();

        internal SceneAsset(SceneIR sceneIR, string path, Guid guid) : base(path, guid)
        {
            SceneIR = sceneIR;
        }

        protected override void OnUpdateResource(object data, string path, Guid guid)
        {
            SceneIR = data as SceneIR;
        }
    }
}