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
        public override GameEntity Build(EntityInstanceData entityData, WorldData worldData, Func<vec2, bool, vec2> positionConverter)
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

            if (Deserialize<int[]>(entityData, "items_amount", out var lootAmount))
            {
                if(lootAmount.Length == lootItems.Length)
                {
                    for (int j = 0; j < lootAmount.Length; j++)
                    {
                        ref var loot = ref lootItems[j];
                        loot.Amount = lootAmount[j];
                    }
                }
                else
                {
                    Debug.Error($"Items amount {lootAmount.Length} for chest are not in sync for items type: {lootItems.Length}: " + entityData.Entity.Iid);
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