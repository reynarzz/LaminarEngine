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

        public bool GetAsset<T>(Guid guid, out T asset) where T : AssetResourceBase
        {
            bool isValid = false;
            asset = null;

            if (_assets.TryGetValue(guid, out var assetRef))
            {
                if (assetRef.TryGetTarget(out var weakAsset))
                {
                    asset = weakAsset as T;
                    isValid = true;
                }
                else
                {
                    // Asset was GC collected, remove it.
                    _assets.Remove(guid);
                    isValid = false;
                }
            }

            return isValid;
        }

        public void PushAsset(Guid guid, AssetResourceBase asset) 
        {
            _assets.Add(guid, new WeakReference<AssetResourceBase>(asset));
        }
    }
}
