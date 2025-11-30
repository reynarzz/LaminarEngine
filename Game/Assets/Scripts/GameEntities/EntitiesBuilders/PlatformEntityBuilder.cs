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
        private struct PlatformPoints
        {
            public long cx { get; set; }
            public long cy { get; set; }
        }
        public override GameEntity Build(vec2 position, FieldInstance[] fields, Func<vec2, bool, vec2> positionConverter)
        {
            var yPositions = default(vec2[]);
            if (Deserialize<PlatformPoints[]>(fields, "positions", out var value))
            {
                yPositions = value.Select(x => positionConverter(new vec2(x.cx, x.cy), true)).ToArray();
            }

            return GamePrefabs.World.InstantiatePlatform(position, yPositions);
        }
    }
}
