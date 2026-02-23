using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class TilemapAsset : AssetResourceBase
    {
        private TilemapData _data;
        internal TilemapAsset(string path, Guid guid, TilemapData data) : base(path, guid)
        {
            _data = data;
        }

        // TODO: do not let that the client get direct access to this.
        public TilemapData GetData()
        {
            return _data;
        }
        internal override void UpdateResource(object data, string path, Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
