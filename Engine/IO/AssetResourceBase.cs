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

        /// <summary>
        /// it is on disk or just in memory? After deletion this should be false.
        /// </summary>
        internal bool IsPhysicallyAvailable { get; set; }
        public string Path => IOLayer.Database?.GetAssetInfo(GetID()).Path ?? string.Empty;
        private string _name = null;
        public override string Name
        {
            get => !string.IsNullOrEmpty(_name) ? _name : IOLayer.Database?.GetAssetInfo(GetID()).Name ?? string.Empty;
            set
            {
                _name = value;
            }
        }

        internal AssetResourceBase(Guid guid) : base(string.Empty, guid)
        {
        }

        internal void UpdateResource(object data, Guid guid)
        {
            Version++;
            OnUpdateResource(data, guid);
        }

        protected abstract void OnUpdateResource(object data, Guid guid);
    }
}