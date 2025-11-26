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
            ItemId lockedByItem = ItemId.none;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (GetEnum<ItemId>(field, "locked_by", out var lockedItem))
                {
                    lockedByItem = lockedItem;
                }
                if (GetEnumArray<ItemId>(field, "items_loot", out var items))
                {
                    lootItems = new ItemAmountPair[items.Length];

                    for (int j = 0; j < items.Length; j++)
                    {
                        ref var loot = ref lootItems[j];
                        loot.Item = items[j];
                    }
                }
                else if (field.Identifier.Equals("items_amount", StringComparison.OrdinalIgnoreCase))
                {
                    var value = JsonConvert.DeserializeObject<int[]>(field.Value.ToString());
                    for (int j = 0; j < value.Length; j++)
                    {
                        var loot = lootItems[j];
                        loot.Amount = value[j];
                        lootItems[j] = loot;
                    }
                }
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