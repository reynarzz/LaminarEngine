using GlmNet;
using ldtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class EnemyEntityBuilder : GameEntityBuilderBase
    {
        public override GameEntity Build(vec2 position, FieldInstance[] fields)
        {
            var enemy = GamePrefabs.Enemies.InstantiateKingPig(position, -1);

            return enemy;
        }
    }
}
