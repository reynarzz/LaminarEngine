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
    public class EntityInstanceData
    {
        public string LayerId { get; set; }
        public EntityInstance Entity { get; set; }
        public vec2 WorldPosition { get; set; }
    }

    public class LayerData
    {
        public Level ParentLevel { get; set; }
        public LayerInstance Layer { get; set; }
        public Dictionary<string, EntityInstanceData> EntitiesData { get; set; }
    }
    public class LevelBuildResult
    {
        public Player Player { get; set; }
        public TilemapResult[] Tilemaps { get; set; }
    }
    public class TilemapResult
    {
        public Bounds Bounds;
    }
    public class LevelBuilderManager
    {
        private readonly GameEntityManager _entityManager;
        public LevelBuilderManager(GameEntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public LevelBuildResult BuildLevel(LevelData levelData)
        {
            var project = LdtkJson.FromJson(Assets.GetText(levelData.TilemapPath).Text);
            //cam.BackgroundColor = new Color32(project.BackgroundColor.R, project.BackgroundColor.G, project.BackgroundColor.B, project.BackgroundColor.A);
            //cam.BackgroundColor = new Color32(23, 28, 57, project.BackgroundColor.A);

            var tilemaps = InstantiateTilemap(project, levelData);
            var player = InstantiateEntities(project, levelData);
            return new LevelBuildResult()
            {
                Player = player,
                Tilemaps = tilemaps
            };
        }

        private TilemapResult[] InstantiateTilemap(LdtkJson project, LevelData levelData)
        {
            var result = new TilemapResult[levelData.Tilemaps.Count];
            for (int i = 0; i < levelData.Tilemaps.Count; i++)
            {
                var tilemapData = levelData.Tilemaps[i];

                var tilemap = new Actor(tilemapData.Name).AddComponent<TilemapRenderer>();
                tilemap.Material = tilemapData.Material ?? MaterialUtils.SpriteMaterialWorld;
                tilemap.Sprite = levelData.TilemapSprites[tilemapData.SpriteIndex];
                tilemap.SortOrder = tilemapData.SortingOrder;

                tilemap.SetTilemapLDtk(project, new LDtkOptions()
                {
                    RenderIntGridLayer = true,
                    RenderTilesLayer = true,
                    RenderAutoLayer = true,
                    LayersToLoadMask = tilemapData.LayersToDraw,
                    WorldDepth = 0,
                    LevelToLoad = levelData.LevelIndex
                });

                if (tilemapData.EnableCollision)
                {
                    tilemap.AddComponent<TilemapCollider2D>();
                }

                var tilemapResult = new TilemapResult()
                {
                    Bounds = new Bounds()
                    {
                        Min = vec3.One * int.MaxValue,
                        Max = vec3.One * int.MinValue
                    }
                };

                foreach (var position in tilemap.TilesPositions)
                {
                    tilemapResult.Bounds.Min = new vec3(Math.Min(tilemapResult.Bounds.Min.x, position.x), Math.Min(tilemapResult.Bounds.Min.y, position.y));
                    tilemapResult.Bounds.Max = new vec3(Math.Max(tilemapResult.Bounds.Max.x, position.x), Math.Max(tilemapResult.Bounds.Max.y, position.y));
                }

                tilemapResult.Bounds.Min -= vec3.One * 0.5f;
                tilemapResult.Bounds.Max += vec3.One * 0.5f;
                result[i] = tilemapResult;
            }

            return result;
        }

        private Player InstantiateEntities(LdtkJson project, LevelData levelData)
        {
            var layersData = CreateEntitiesData(project.Levels[levelData.LevelIndex], levelData.WorldSpacePixelsPerUnit);

            Player player = null;

            foreach (var layer in layersData.Values)
            {
                foreach (var entityData in layer.EntitiesData.Values)
                {
                    var gameEntity = _entityManager.BuildEntity(entityData, layersData, (vec2, isGrid) =>
                    {
                        // TODO: refactor, instead send a helper class that contains convert methods.
                        return ConvertToWorld((int)vec2.x, (int)vec2.y, layer.ParentLevel, levelData.WorldSpacePixelsPerUnit, layer.Layer, isGrid);
                    });

                    if (entityData.Entity.Identifier.Equals("Player"))
                    {
                        GamePrefabs.World.InstantiateDoor(entityData.WorldPosition + new vec2(0, 1), new DoorData() { InteractCondition = x => false });

                        player = gameEntity.GetComponent<Player>();
                    }
                }
            }

            return player;
        }

        // TODO: The asset cooker should serialize this into a json, or binary.
        private Dictionary<string, LayerData> CreateEntitiesData(Level level, int pixelsPerUnit)
        {
            var layers = new Dictionary<string, LayerData>();
            for (int i = 0; i < level.LayerInstances.Length; i++)
            {
                var layerInstance = level.LayerInstances[i];
                var layerData = new LayerData()
                {
                    Layer = layerInstance,
                    EntitiesData = new Dictionary<string, EntityInstanceData>(),
                    ParentLevel = level
                };

                foreach (var entity in layerInstance.EntityInstances)
                {
                    layerData.EntitiesData.Add(entity.Iid, new EntityInstanceData()
                    {
                        Entity = entity,
                        LayerId = layerInstance.Identifier,
                        WorldPosition = ConvertToWorld(entity.Px[0], entity.Px[1], level, pixelsPerUnit, layerInstance)
                    });
                }

                layers.Add(layerInstance.Iid, layerData);
            }

            return layers;
        }

        private vec2 ConvertToWorld(long x, long y, Level level, float pixelPerUnit, LayerInstance layer, bool isGridPos = false)
        {
            if (isGridPos)
            {
                x *= layer.GridSize;
                y *= layer.GridSize;
            }
            return new vec2(MathF.Floor((level.WorldX + x + layer.PxOffsetX) / pixelPerUnit), MathF.Ceiling((-level.WorldY + -y + -layer.PxOffsetY) / pixelPerUnit));
        }
    }
}