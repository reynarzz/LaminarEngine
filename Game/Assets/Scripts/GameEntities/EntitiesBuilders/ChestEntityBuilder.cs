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
        public override GameEntity Build(EntityInstanceData entityData, IReadOnlyDictionary<string, LayerData> layers, Func<vec2, bool, vec2> positionConverter)
        {
            var lootItems = default(ItemAmountPair[]);
        
            if (Deserialize<int[]>(entityData.Entity.FieldInstances, "items_amount", out var lootAmount))
            {
                lootItems = new ItemAmountPair[lootAmount.Length];

                for (int j = 0; j < lootAmount.Length; j++)
                {
                    var loot = lootItems[j];
                    loot.Amount = lootAmount[j];
                    lootItems[j] = loot;
                }
            }

            if (GetEnumArray<ItemId>(entityData.Entity.FieldInstances, "items_loot", out var items))
            {
                for (int j = 0; j < items.Length; j++)
                {
                    ref var loot = ref lootItems[j];
                    loot.Item = items[j];
                }
            }

            ItemId lockedByItem = ItemId.none;
            if (GetEnum<ItemId>(entityData.Entity.FieldInstances, "locked_by", out var lockedItem))
            {
                lockedByItem = lockedItem;
            }

            return GamePrefabs.Items.InstantiateChest(entityData.WorldPosition, new ChestData()
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