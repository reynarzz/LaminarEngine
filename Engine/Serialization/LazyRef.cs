using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public interface ILazyRef
    {
        internal AssetType GetAssetType();
        internal SerializableGuid GetRefId();

        internal void SetAssetType(AssetType type);
        internal void SetRefId(SerializableGuid refId);
        internal bool HasRef();
    }
    public struct LazyRef<T> : ILazyRef where T : AssetResourceBase
    {
        [SerializedField] private SerializableGuid _refId;
        [SerializedField] private AssetType _type;

        private T _asset;
        public T GetRef()
        {
            if (!_asset)
            {
                _asset = Assets.GetAssetFromGuid(_refId.Guid) as T;
            }

            return _asset;
        }

        public void GetRefAsync(Action<T> onCompleted)
        {
            // TODO: implement real async.
            var asset = GetRef();
            onCompleted(asset);
        }
      
        public void Destroy()
        {
            _asset = null;
            Assets.DestroyAsset(_refId.Guid);
        }

        AssetType ILazyRef.GetAssetType()
        {
            return _type;
        }

        SerializableGuid ILazyRef.GetRefId()
        {
            return _refId;
        }

        bool ILazyRef.HasRef()
        {
            return _refId != Guid.Empty && _type != AssetType.Invalid;
        }

        void ILazyRef.SetAssetType(AssetType type)
        {
            _type = type;
        }

        void ILazyRef.SetRefId(SerializableGuid refId)
        {
            _refId = refId;
        }
    }
}
