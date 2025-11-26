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
            var lootItems = default(ChestLoot[]);

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.Identifier.Equals("items_loot", StringComparison.OrdinalIgnoreCase))
                {

                    var value = JsonConvert.DeserializeObject<string[]>(field.Value.ToString());
                    for (int j = 0; j < value.Length; j++)
                    {
                        if (lootItems == null)
                        {
                            lootItems = new ChestLoot[value.Length];
                        }
                        Enum.TryParse<ItemId>(value[j], true, out var item);
                        var loot = lootItems[j];
                        loot.Item = item;
                        lootItems[j] = loot;

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

            return GamePrefabs.Items.InstantiateChest(position, lootItems);
        }

        private void SimpleCollectible()
        {

        }
    }
}