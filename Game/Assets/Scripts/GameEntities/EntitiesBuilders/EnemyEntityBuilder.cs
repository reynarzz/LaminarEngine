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
    public enum RandomItemType
    {
        Invalid,
        All,
        Health,
        Ammo,
        HealthOrAmmo
    }
    internal class EnemyEntityBuilder : GameEntityBuilderBase
    {
        public override GameEntity Build(TilemapEntity entityData, TilemapData worldData)
        {
            var enemyData = new EnemyInstanceData()
            {
                Position = entityData.WorldPosition,
            };

            GetBool(entityData, "look_to_right", out bool lookToRight);
            enemyData.LookDir = lookToRight ? 1 : -1;

            GetEnum(entityData, "enemy_type", out enemyData.Type);

            if (GetBool(entityData, "random_loot", out var isRandomLoot))
            {
                if (isRandomLoot)
                {
                    if (GetInt(entityData, "random_loot_range", out var randomRange))
                    {
                        var lootCount = Random.Shared.Next(0, randomRange + 1);

                        if (lootCount > 0)
                        {
                            enemyData.StartInventoryItems = new ItemAmountPair[lootCount];
                        }
                    }

                    if (enemyData.StartInventoryItems != null)
                    {
                        if (GetEnum<RandomItemType>(entityData, "random_item_type", out var randomItemType))
                        {
                            for (int i = 0; i < enemyData.StartInventoryItems.Length; i++)
                            {
                                // TODO: implement random item type
                                enemyData.StartInventoryItems[i] = new ItemAmountPair() { Item = ItemId.small_potion, Amount = 1 };
                            }
                        }

                        else
                        {
                            Debug.Error("Random item type error");
                        }
                    }
                }
                else
                {
                    if (GetEnumArray<ItemId>(entityData, "loot", out var loot))
                    {
                        enemyData.StartInventoryItems = new ItemAmountPair[loot.Length];

                        for (int i = 0; i < loot.Length; i++)
                        {
                            enemyData.StartInventoryItems[i] = new ItemAmountPair() { Item = loot[i], Amount = 1 };
                        }
                    }
                }
            }

            return GamePrefabs.Enemies.InstantiateEnemy(enemyData);
        }
    }
}