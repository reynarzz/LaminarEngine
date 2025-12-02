using CsvHelper;
using Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public enum ItemId
    {
        none,

        // Currency
        coin_currency,

        // Throwables
        confusion_grenade,

        // Health
        big_potion,
        normal_potion,
        small_potion,

        // Ammo
        normal_ammo,
        poison_ammo,
        fire_ammo,

        // Keys
        simple_key,
        chest_key,
        boss_key
    }

    public class ItemsDatabase
    {
        private static readonly Dictionary<ItemId, Item> _itemsDatabase = new();
        private static Dictionary<ItemId, Type> _itemTypeMapper = new()
        {
            { ItemId.coin_currency, typeof(CoinItem) },
            { ItemId.confusion_grenade, typeof(ConfusionGrenadeItem) },
            { ItemId.big_potion, typeof(PotionItem) },
            { ItemId.normal_potion, typeof(PotionItem) },
            { ItemId.small_potion, typeof(PotionItem) },
            { ItemId.normal_ammo, typeof(NormalAmmoItem) },
            { ItemId.poison_ammo, typeof(PoisonAmmoItem) },
            { ItemId.fire_ammo, typeof(FireAmmoItem) },
            { ItemId.simple_key, typeof(KeyItem) },
            { ItemId.chest_key, typeof(KeyItem) },
            { ItemId.boss_key, typeof(KeyItem) },
        };

        public ItemsDatabase(string databaseCsvPath)
        {
            var csv = Assets.GetText(databaseCsvPath);

            if (!csv)
            {
                Debug.Error($"Items database cannot be found at path '{databaseCsvPath}'");
                return;
            }
            try
            {
                using (var strReader = new StringReader(csv.Text))
                using (var csvReader = new CsvReader(strReader, CultureInfo.InvariantCulture))
                {
                    var records = csvReader.GetRecords<ItemFeatures>();

                    foreach (var itemFeatures in records)
                    {
                        if (_itemTypeMapper.TryGetValue(itemFeatures.Id, out var type))
                        {
                            _itemsDatabase[itemFeatures.Id] = Activator.CreateInstance(type, itemFeatures) as Item;
                        }
                        else
                        {
                            Debug.Error($"Item id '{itemFeatures.Id}' need to be defined");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Error(e);
            }
            
        }

        public static string GetDatabaseSchemaCsv()
        {
            using var memStream = new MemoryStream();
            using (var writer = new StreamWriter(memStream))
            using (var csvWritter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var ids = Enum.GetValues<ItemId>();

                var items = new List<ItemFeatures>(ids.Length);
                foreach (var id in ids)
                {
                    if(id == ItemId.none)
                    {
                        continue;
                    }
                    items.Add(new ItemFeatures()
                    {
                        Id = id,
                        Amount = 1,
                        MaxPerSlot = 999,
                        UserCanUseIt = true,
                        AutoConsumable = false,
                        SecondsToDisappear = 7,
                        UserCanRemove = true,
                        RemoveAfterUse = true,
                        MultipleSlots = true,
                        DecreasesOnUse = true,
                        IsStackable = true,
                    });
                }

                csvWritter.WriteRecords(items);
                writer.Flush();

                memStream.Position = 0;
                return Encoding.UTF8.GetString(memStream.ToArray());
            }
        }

        public static Item GetItem(ItemId id)
        {
            _itemsDatabase.TryGetValue(id, out var item);
            return item;
        }
    }
}
