using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal partial class GamePrefabs
    {
        public static class World
        {
            public static Door InstantiateDoor(vec2 position)
            {
                var door = new Actor("Door").AddComponent<Door>();
                door.Transform.WorldPosition = position;
                return door;
            }
        }
    }
}
