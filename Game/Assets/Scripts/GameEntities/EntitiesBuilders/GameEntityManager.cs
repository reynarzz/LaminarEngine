using Engine;
using GlmNet;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ldtk;
using System;

namespace Game
{
    public class GameEntityManager
    {
        private const string DONT_DESTROY_ON_LOAD = "Persistant";
        private readonly Dictionary<string, GameEntityBuilderBase> _entityBuilders;

        public static Player Player { get; private set; }
        public GameEntityManager()
        {
            _entityBuilders = new Dictionary<string, GameEntityBuilderBase>()
            {
                { "Door", new DoorEntityBuilder() },
                { "Collectible", new ItemsEntityBuilder() },
                { "Enemy", new EnemyEntityBuilder() },
                { "Chest", new ChestEntityBuilder() },
                { "Platform", new PlatformEntityBuilder() },
                { "Player", new PlayerEntityBuilder() },
                { "Portal", new PortalEntityBuilder() },
            };
        }

        public GameEntity BuildEntity(TilemapEntity entityData, TilemapData worldData)
        {
            if (_entityBuilders.TryGetValue(entityData.Identifier, out var builder))
            {
                var gameEntity = builder.Build(entityData, worldData);

                foreach (var tag in entityData.Tags)
                {
                    if (tag.Equals(DONT_DESTROY_ON_LOAD))
                    {
                        Actor.DontDestroyOnLoad(gameEntity.Actor);
                    }
                }

                return gameEntity;
            }

            return null;
        }
    }
}
