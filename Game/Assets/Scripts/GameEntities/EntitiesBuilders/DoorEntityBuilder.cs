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
    public class DoorEntityBuilder : GameEntityBuilderBase
    {
        public override GameEntity Build(vec2 position, FieldInstance[] fields)
        {
            foreach (var field in fields)
            {
                Debug.Log(field.Identifier + ", " + field.Value);
            }

            return GamePrefabs.World.InstantiateDoor(position);
        }
    }
}
