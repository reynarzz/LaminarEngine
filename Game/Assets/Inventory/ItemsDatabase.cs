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
        unknown,

        // Currency
        coin_currency,

        // Throwables
        confusion_grenade,

        // Health
        small_potion,

        // Ammo
        normal_ammo,
        toxic_ammo,
        fire_ammo,

        // Keys
        simple_key,

    }

    public class ItemsDatabase
    {
        private readonly Dictionary<ItemId, Item> _itemsDatabase;
        private Dictionary<ItemId, Type> _itemTypeMapper = new()
        {
            { ItemId.coin_currency, typeof() },
            { ItemId.coin_currency, typeof() },
            { ItemId.confusion_grenade, typeof() },
            { ItemId.small_potion, typeof(SmallPotion) },
            { ItemId.normal_ammo, typeof() },
            { ItemId.toxic_ammo, typeof() },
            { ItemId.fire_ammo, typeof() },
            { ItemId.simple_key, typeof() }
        };

        public ItemsDatabase(string databaseCsvPath)
        {
            var csv = Assets.GetText(databaseCsvPath);

            if (!csv)
            {
                Debug.Error($"Items database cannot be found at path '{databaseCsvPath}'");
                return;
            }
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
                    items.Add(new ItemFeatures()
                    {
                        Id = id,
                        Amount = 1,
                        MaxDefault = 1,
                        UserCanUseIt = true,
                        AutoConsumable = false,
                        SecondsToDisappear = 7,
                        UserCanRemove = true,
                        RemoveAfterUse = true
                    });
                }

                csvWritter.WriteRecords(items);
                writer.Flush();

                memStream.Position = 0;
                return Encoding.UTF8.GetString(memStream.ToArray());
            }
        }

        public Item GetItem(ItemId id)
        {
            _itemsDatabase.TryGetValue(id, out var item);
            return item;
        }
    }
}
