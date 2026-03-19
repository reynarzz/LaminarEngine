using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class FontAsset : Asset
    {
        internal byte[] Data { get; }

        public FontAsset(Guid refId, byte[] data) : base(refId)
        {
            Data = data;
        }

        protected override void OnUpdateResource(object data, Guid refId)
        {
            throw new NotImplementedException();
        }
    }
}
