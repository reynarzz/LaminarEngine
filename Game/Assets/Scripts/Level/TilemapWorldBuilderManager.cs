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
    public class LevelBuildResult
    {
        public Player Player { get; set; }
        public TilemapResult[] Tilemaps { get; set; }
    }
    public struct TilemapResult
    {
        public Bounds Bounds;
    }
  
    public class TilemapWorldBuilderManager
    {
        private readonly GameEntityManager _entityManager;
        private readonly TilemapAsset _tilemapData;
        public TilemapWorldBuilderManager(string worldPath, GameEntityManager entityManager)
        {
            _entityManager = entityManager;
            _tilemapData = Assets.GetTilemap(worldPath);
        }

        public LevelBuildResult BuildLevel(LevelInstantiateInfo levelData)
        {
            //cam.BackgroundColor = new Color32(project.BackgroundColor.R, project.BackgroundColor.G, project.BackgroundColor.B, project.BackgroundColor.A);
            //cam.BackgroundColor = new Color32(23, 28, 57, project.BackgroundColor.A);

            var tilemaps = InstantiateTilemap(_tilemapData, levelData);
            var player = InstantiateEntities(_tilemapData, levelData);
            return new LevelBuildResult()
            {
                Player = player,
                Tilemaps = tilemaps
            };
        }

        private TilemapResult[] InstantiateTilemap(TilemapAsset tilemapAsset, LevelInstantiateInfo levelInstanceInfo)
        {
            var result = new TilemapResult[levelInstanceInfo.Tilemaps.Count];
            for (int i = 0; i < levelInstanceInfo.Tilemaps.Count; i++)
            {
                var tilemapData = levelInstanceInfo.Tilemaps[i];
                var tilemapLevelData = tilemapAsset.GetData().Levels.FirstOrDefault(x => x.Value.LevelIndex == levelInstanceInfo.LevelIndex).Value;
                var tilemap = new Actor(tilemapData.Name).AddComponent<TilemapRenderer>();
                tilemap.Material = tilemapData.Material ?? GameMaterials.Instance.SpriteMaterialWorld;
                tilemap.Sprite = levelInstanceInfo.TilemapSprites[tilemapData.SpriteIndex];
                tilemap.SortOrder = tilemapData.SortingOrder;

                tilemap.SetTilemap(tilemapAsset, new TilemapRenderingOptions()
                {
                    LayerIndex = tilemapData.LayerIndex,
                    LevelIndex = levelInstanceInfo.LevelIndex
                });

                if (tilemapData.EnableCollision)
                {
                    var collider = tilemap.AddComponent<TilemapCollider2D>();

                    if (tilemapData.IsTriggerCollision)
                    {
                        collider.IsTrigger = true;
                    }

                    collider.Offset = tilemapData.ColliderOffset;
                }

                tilemapData.TilemapAction?.Invoke(tilemap);
                result[i] = new TilemapResult()
                {
                    Bounds = tilemapLevelData.Layers.FirstOrDefault(x => x.Value.LayerIndex == tilemapData.LayerIndex).Value.Bounds,
                };
            }
            
            return result;
        }

        private Player InstantiateEntities(TilemapAsset tilemapAsset, LevelInstantiateInfo levelData)
        {
            Player player = null;
            var tilemapData = tilemapAsset.GetData();

            var level = tilemapData.Levels.FirstOrDefault(x => x.Value.LevelIndex == levelData.LevelIndex).Value;

            foreach (var layer in level.Layers.Values)
            {
                if (layer.Entities == null)
                    continue;

                foreach (var entityData in layer.Entities.Values)
                {
                    var gameEntity = _entityManager.BuildEntity(entityData, tilemapData);

                    if (entityData.Identifier.Equals("Player"))
                    {
                        GamePrefabs.World.InstantiateDoor(entityData.WorldPosition + new vec2(0, 0.5f), new DoorData() { InteractCondition = x => false });

                        player = gameEntity.GetComponent<Player>();
                    }
                }
            }

            return player;
        }

        //private vec2 ConvertToWorld(long x, long y, Level level, float pixelPerUnit, LayerInstance layer, bool isGridPos = false)
        //{
        //    // NOTE: Points from entity fields data are in grid position
        //    if (isGridPos)
        //    {
        //        x *= layer.GridSize;
        //        y *= layer.GridSize;
        //    }
        //    return new vec2(MathF.Floor((level.WorldX + x + layer.PxOffsetX) / pixelPerUnit), MathF.Ceiling((-level.WorldY + -y + -layer.PxOffsetY) / pixelPerUnit));
        //}
    }
}