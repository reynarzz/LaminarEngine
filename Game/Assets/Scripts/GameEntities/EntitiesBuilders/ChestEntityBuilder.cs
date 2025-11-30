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
        public override GameEntity Build(vec2 position, FieldInstance[] fields, Func<vec2, bool, vec2> positionConverter)
        {
            var lootItems = default(ItemAmountPair[]);
        
            if (Deserialize<int[]>(fields, "items_amount", out var lootAmount))
            {
                lootItems = new ItemAmountPair[lootAmount.Length];

                for (int j = 0; j < lootAmount.Length; j++)
                {
                    var loot = lootItems[j];
                    loot.Amount = lootAmount[j];
                    lootItems[j] = loot;
                }
            }

            if (GetEnumArray<ItemId>(fields, "items_loot", out var items))
            {
                for (int j = 0; j < items.Length; j++)
                {
                    ref var loot = ref lootItems[j];
                    loot.Item = items[j];
                }
            }

            ItemId lockedByItem = ItemId.none;
            if (GetEnum<ItemId>(fields, "locked_by", out var lockedItem))
            {
                lockedByItem = lockedItem;
            }

            return GamePrefabs.Items.InstantiateChest(position, new ChestData()
            {
                LockedBy = lockedByItem,
                ChestLoot = lootItems,
                InteractCondition = PlayerHasItem_Condition(lockedByItem, 1)
            });
        }

        private void SimpleCollectible()
        {

        }
    }
}