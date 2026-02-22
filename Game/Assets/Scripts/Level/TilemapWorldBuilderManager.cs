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
        public string LayerIID { get; set; }
        public string LevelIID { get; set; }
        public EntityInstance Entity { get; set; }
        public vec2 WorldPosition { get; set; }
    }

    public class LayerData
    {
        public LayerInstance Layer { get; set; }
        public Dictionary<string, EntityInstanceData> EntitiesData { get; set; }
    }

    public class LevelData
    {
        public Level Level { get; set; }
        public int LevelIndex { get; set; }
        public Dictionary<string, LayerData> Layers { get; set; }
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
    public class WorldData
    {
        public IReadOnlyDictionary<string, LevelData> Levels => _levelsData;
        private readonly Dictionary<string, LevelData> _levelsData;
        public WorldData(Dictionary<string, LevelData> levelsData)
        {
            _levelsData = levelsData;
        }
    }

    public class TilemapWorldBuilderManager
    {
        private readonly GameEntityManager _entityManager;
        private readonly LdtkJson _project;
        private readonly int _pixelsPerUnit = 16;
        private readonly WorldData _worldData;
        public TilemapWorldBuilderManager(string worldPath, GameEntityManager entityManager)
        {
            _entityManager = entityManager;
            _project = LdtkJson.FromJson(Assets.GetText(worldPath).Text);
            _worldData = new WorldData(CreateEntitiesData(_project, _pixelsPerUnit));
        }

        public LevelBuildResult BuildLevel(LevelInstantiateInfo levelData)
        {
            //cam.BackgroundColor = new Color32(project.BackgroundColor.R, project.BackgroundColor.G, project.BackgroundColor.B, project.BackgroundColor.A);
            //cam.BackgroundColor = new Color32(23, 28, 57, project.BackgroundColor.A);

            var tilemaps = InstantiateTilemap(_project, levelData);
            var player = InstantiateEntities(_project, levelData);
            return new LevelBuildResult()
            {
                Player = player,
                Tilemaps = tilemaps
            };
        }

        private TilemapResult[] InstantiateTilemap(LdtkJson project, LevelInstantiateInfo levelData)
        {
            var result = new TilemapResult[levelData.Tilemaps.Count];
            for (int i = 0; i < levelData.Tilemaps.Count; i++)
            {
                var tilemapData = levelData.Tilemaps[i];

                var tilemap = new Actor(tilemapData.Name).AddComponent<TilemapRenderer>();
                tilemap.Material = tilemapData.Material ?? GameMaterials.Instance.SpriteMaterialWorld;
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
                    var collider = tilemap.AddComponent<TilemapCollider2D>();

                    if (tilemapData.IsTriggerCollision)
                    {
                        collider.IsTrigger = true;
                    }

                    collider.Offset = tilemapData.ColliderOffset;
                }

                tilemapData.TilemapAction?.Invoke(tilemap);
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

        private Player InstantiateEntities(LdtkJson project, LevelInstantiateInfo levelData)
        {
            Player player = null;
            var level = _worldData.Levels.FirstOrDefault(x => x.Value.LevelIndex == levelData.LevelIndex).Value;

            foreach (var layer in level.Layers.Values)
            {
                foreach (var entityData in layer.EntitiesData.Values)
                {
                    var gameEntity = _entityManager.BuildEntity(entityData, _worldData, (vec2, isGrid) =>
                    {
                        // TODO: refactor, instead send a helper class that contains convert methods.
                        return ConvertToWorld((int)vec2.x, (int)vec2.y, level.Level, levelData.WorldSpacePixelsPerUnit, layer.Layer, isGrid);
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
        private Dictionary<string, LevelData> CreateEntitiesData(LdtkJson project, int pixelsPerUnit)
        {
            var levels = new Dictionary<string, LevelData>();

            for (int i = 0; i < project.Levels.Length; i++)
            {
                var level = project.Levels[i];

                var layers = new Dictionary<string, LayerData>();
                var levelData = new LevelData()
                {
                    Level = level,
                    LevelIndex = i,
                    Layers = layers
                };
                levels.Add(level.Iid, levelData);
                for (int j = 0; j < level.LayerInstances.Length; j++)
                {
                    var layerInstance = level.LayerInstances[j];
                    var layerData = new LayerData()
                    {
                        Layer = layerInstance,
                        EntitiesData = new Dictionary<string, EntityInstanceData>(),
                    };

                    foreach (var entity in layerInstance.EntityInstances)
                    {
                        layerData.EntitiesData.Add(entity.Iid, new EntityInstanceData()
                        {
                            Entity = entity,
                            LayerIID = layerInstance.Iid,
                            LevelIID = level.Iid,
                            WorldPosition = ConvertToWorld(entity.Px[0], entity.Px[1], level, pixelsPerUnit, layerInstance)
                        });
                    }

                    layers.Add(layerInstance.Iid, layerData);
                }
            }

            return levels;
        }

        private vec2 ConvertToWorld(long x, long y, Level level, float pixelPerUnit, LayerInstance layer, bool isGridPos = false)
        {
            // NOTE: Points from entity fields data are in grid position
            if (isGridPos)
            {
                x *= layer.GridSize;
                y *= layer.GridSize;
            }
            return new vec2(MathF.Floor((level.WorldX + x + layer.PxOffsetX) / pixelPerUnit), MathF.Ceiling((-level.WorldY + -y + -layer.PxOffsetY) / pixelPerUnit));
        }
    }
}