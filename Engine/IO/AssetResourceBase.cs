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
        public string Path { get; }
        internal AssetResourceBase(string path, Guid guid) : base(System.IO.Path.GetFileNameWithoutExtension(path), guid)
        {
            Path = path;
        }

        internal void UpdateResource(object data, string path, Guid guid)
        {
            Version++;
            OnUpdateResource(data, path, guid);
        }

        protected abstract void OnUpdateResource(object data, string path, Guid guid);
    }
}