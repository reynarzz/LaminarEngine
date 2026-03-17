using Engine.IO;
using Engine.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class AssetResourceBase : EObject
    {
        internal uint Version { get; private set; }
        internal virtual bool IsCacheHardReference { get; protected private set; } = false;

        public string Path => IOLayer.Database?.GetAssetInfo(GetID()).Path ?? string.Empty;
        public override string Name
        {
            get => System.IO.Path.GetFileNameWithoutExtension(IOLayer.Database?.GetAssetInfo(GetID()).Path ?? string.Empty);
            set
            {
            }
        }

        internal AssetResourceBase(string path, Guid guid) : base(System.IO.Path.GetFileNameWithoutExtension(path), guid)
        {
        }

        internal void UpdateResource(object data, string path, Guid guid)
        {
            Version++;
            OnUpdateResource(data, path, guid);
        }

        protected abstract void OnUpdateResource(object data, string path, Guid guid);
    }
}