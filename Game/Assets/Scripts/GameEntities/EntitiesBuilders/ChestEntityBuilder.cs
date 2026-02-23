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

                for (int j = 0; j < items.Length; j++)
                {
                    ref var loot = ref lootItems[j];
                    loot.Item = items[j];
                    loot.Amount = 1;
                }
            }

            if (entityData.Properties.TryGetValue("items_amount", out var lootAmount))
            {
                if(lootAmount.Value.IntArray != null && lootItems != null && lootAmount.Value.IntArray.Length == lootItems.Length)
                {
                    for (int j = 0; j < lootAmount.Value.IntArray.Length; j++)
                    {
                        ref var loot = ref lootItems[j];
                        loot.Amount = lootAmount.Value.IntArray[j];
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