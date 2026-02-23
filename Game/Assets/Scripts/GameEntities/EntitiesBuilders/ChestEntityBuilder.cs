using Engine;
using GlmNet;
using ldtk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class ChestEntityBuilder : GameEntityBuilderBase
    {
        public override GameEntity Build(TilemapEntity entityData, TilemapData worldData)
        {
            var lootItems = default(ItemAmountPair[]);

            if (GetEnumArray<ItemId>(entityData, "items_loot", out var items))
            {
                lootItems = new ItemAmountPair[items.Length];

                for (int i = 0; i < items.Length; i++)
                {
                    ref var loot = ref lootItems[i];
                    loot.Item = items[i];
                    loot.Amount = 1;
                }
            }

            if (entityData.Properties.TryGetValue("items_amount", out var lootAmount))
            {
                if(lootAmount.Value.IntArray != null && lootItems != null && lootAmount.Value.IntArray.Length == lootItems.Length)
                {
                    for (int i = 0; i < lootAmount.Value.IntArray.Length; i++)
                    {
                        ref var loot = ref lootItems[i];
                        loot.Amount = lootAmount.Value.IntArray[i];
                    }
                }
                else
                {
                    var len = lootAmount.Value.IntArray?.Length ?? 0;
                    Debug.Error($"Items amount {len} for chest are not in sync for items type: {lootItems.Length}: " + entityData.IID);
                }
            }

            if(!GetEnum<ItemId>(entityData, "locked_by", out var lockedItem))
            {
                lockedItem = ItemId.none;
            }

            return GamePrefabs.Items.InstantiateChest(entityData.WorldPosition, new ChestData()
            {
                LockedBy = lockedItem,
                ChestLoot = lootItems,
                InteractCondition = PlayerHasItem_Condition(lockedItem, 1)
            });
        }

        private void SimpleCollectible()
        {

        }
    }
}