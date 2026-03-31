using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utils
{
    internal class _NATIVE_AOT_PRESERVER_
    {
        // Add all the calls that the nativeAot procedure should see so it doesn't get trimmed.
        public void Preserve()
        {
            _ = new List<TilemapLayerConfig>();
            _ = new List<TextureAtlasCell>();
            _ = new TextureAtlasCell();
            _ = new TilemapLayerConfig();
        }
    }
}
