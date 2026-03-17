using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AssetsDatabaseInfo
    {
        public int TotalAssets { get; set; }
        public DateTime CreationDate { get; set; }
        public Dictionary<Guid, AssetInfo> Assets { get; set; } = new();

        [JsonIgnore] public List<Guid> UpdatedAssets { get; private set; } = new();
        [JsonIgnore] public int ChangedCount { get; set; }

        public AssetsDatabaseInfo Copy()
        {
            return new AssetsDatabaseInfo()
            {
                TotalAssets = TotalAssets,
                CreationDate = CreationDate,
                ChangedCount = ChangedCount,
                UpdatedAssets = UpdatedAssets.ToList(),
                Assets = new Dictionary<Guid, AssetInfo>(Assets)
            };
        }
    }
}
