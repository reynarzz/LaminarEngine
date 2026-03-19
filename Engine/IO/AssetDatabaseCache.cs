using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal class AssetDatabaseCache
    {
        private readonly Dictionary<Guid, WeakReference<AssetResourceBase>> _assets = new();
        private readonly Dictionary<Guid, AssetResourceBase> _assetsHardRefs = new();

        public bool GetAsset<T>(Guid guid, out T asset) where T : AssetResourceBase
        {
            var result = GetAsset(guid, out AssetResourceBase assetOut);
            asset = assetOut as T;
            return result;
        }

        public bool GetAsset(Guid guid, out AssetResourceBase asset)
        {
            bool isValid = false;
            asset = null;

            if (_assets.TryGetValue(guid, out var assetRef))
            {
                if (assetRef.TryGetTarget(out var weakAsset))
                {
                    asset = weakAsset;
                    isValid = true;
                }
                else
                {
                    // Asset was GC collected, remove it.
                    _assets.Remove(guid);
                    isValid = false;
                }
            }
            else
            {
                isValid = _assetsHardRefs.TryGetValue(guid, out asset);
            }

            return isValid;
        }

        public void PushAsset(Guid guid, AssetResourceBase asset)
        {
            if (!asset.IsCacheHardReference)
            {
                _assets.Add(guid, new WeakReference<AssetResourceBase>(asset));
            }
            else if (!_assetsHardRefs.ContainsKey(guid))
            {
                _assetsHardRefs.Add(guid, asset);
            }
        }

        internal void Remove(Guid refId)
        {
            _assetsHardRefs.Remove(refId);
            _assets.Remove(refId);
        }
    }
}
