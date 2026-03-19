using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class TilemapAsset : Asset
    {
        private TilemapData _data;
        internal TilemapAsset(Guid refId, TilemapData data) : base(refId)
        {
            _data = data;
        }

        // TODO: do not let that the client get direct access to this.
        public TilemapData GetData()
        {
            return _data;
        }
        protected override void OnUpdateResource(object data, Guid guid)
        {
            _data = data as TilemapData;

        }
    }
}
