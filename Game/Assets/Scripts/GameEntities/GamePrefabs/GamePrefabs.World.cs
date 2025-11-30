using Engine;
using Engine.Utils;
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
            public static Door InstantiateDoor(vec2 position, DoorData data)
            {
                var door = new Actor("Door").AddComponent<Door>();
                door.Init(data);
                door.Transform.WorldPosition = position;
                return door;
            }

            public static Platform InstantiatePlatform(vec2 position, vec2[] positions)
            {
                if(positions == null)
                {
                    Debug.Error("No position set for platform");
                    return null;
                }
                var platform = new Actor<SpriteRenderer>("Platform").AddComponent<Platform>();
                platform.GetComponent<SpriteRenderer>().Material = MaterialUtils.SpriteMaterial;
                platform.Actor.Layer = LayerMask.NameToLayer(GameConsts.PLATFORM);
                platform.Init(position, positions);

                return platform;
            }
        }
    }
}
