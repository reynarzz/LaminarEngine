using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class AssetResourceBase : EObject
    {
        public string Path { get; }
        internal AssetResourceBase(string path, Guid guid) : base(System.IO.Path.GetFileNameWithoutExtension(path), guid)
        {
            Path = path;
        }

        internal abstract void UpdateResource(object data, string path, Guid guid);
    }

    //public abstract class AssetResourceBase<T> : AssetResourceBase where T : class
    //{
    //    internal AssetResourceBase(string path, Guid guid) : base(path, guid) { }
    //    internal override void UpdateResource(object data, string path, Guid guid)
    //    {
    //        OnResourceUpdate(data as T, path, guid);
    //    }

    //    internal abstract void OnResourceUpdate(T data, string path, Guid guid);
    //}
}