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
    internal class PlatformEntityBuilder : GameEntityBuilderBase
    {
        public override GameEntity Build(TilemapEntity entityData, TilemapData worldData)
        {
            vec2[] yPositions = null;

            if (entityData.Properties.TryGetValue("positions", out var data))
            {
                yPositions = data.Value.Vec2Array;
            }

            return GamePrefabs.World.InstantiatePlatform(entityData.WorldPosition, yPositions);
        }
    }
}
