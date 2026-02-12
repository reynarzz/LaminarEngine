using Engine.IO;
using Newtonsoft.Json;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    public abstract class IOLayer : LayerBase
    {
        private static AssetDatabase _assetDatabase;
        internal static AssetDatabase Database => _assetDatabase; // Remove this.

        private protected void InitializeIO(DiskBase disk, Dictionary<AssetType, IAssetBuilder> assetsBuilder)
        {
            _assetDatabase = new AssetDatabase(assetsBuilder);

            disk.Initialize();

           _assetDatabase.Initialize(disk);
        }

        private protected void Reload(DiskBase disk)
        {
            _assetDatabase.Initialize(disk);
        }

        internal static AssetDatabase GetDatabase() // Refactor
        {
            return _assetDatabase;
        }

        public override void Close()
        {
        }
    }
}
