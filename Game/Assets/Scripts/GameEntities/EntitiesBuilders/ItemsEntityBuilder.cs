using Engine;
using GlmNet;
using ldtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class ItemsEntityBuilder : GameEntityBuilderBase
    {
        public override GameEntity Build(vec2 position, FieldInstance[] fields, Func<vec2, bool, vec2> positionConverter)
        {
            foreach (var field in fields)
            {
                if (field.Identifier.Equals("ItemType", StringComparison.OrdinalIgnoreCase))
                {
                    if(Enum.TryParse<ItemId>(field.Value.ToString(), true, out var item))
                    {
                        GamePrefabs.Items.InstantiateCollectible(item, position);
                    }
                    else
                    {
                        Debug.Error($"Can't create item: {field.Value.ToString()}, is not in the enum.");
                    }
                }
            }

            return null;
        }

        private void SimpleCollectible()
        {

        }
    }
}