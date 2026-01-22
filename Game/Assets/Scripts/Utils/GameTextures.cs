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
    public static class GameTextures
    {
        private readonly static Dictionary<string, Sprite[]> _tilesets;
        private readonly static Dictionary<string, Sprite> _sprites;
        static GameTextures()
        {
            var playerPivot = new vec2(0.4f, 0.42f);

            _tilesets = new Dictionary<string, Sprite[]>()
            {
                // Player atlases
                { "player_idle",     SliceSprites("KingsAndPigsSprites/01-King Human/Idle (78x58)S.png", 78, 58, playerPivot) },
                { "player_run",      SliceSprites("KingsAndPigsSprites/01-King Human/Run (78x58)S.png", 78, 58,     playerPivot) },
                { "player_jump",     SliceSprites("KingsAndPigsSprites/01-King Human/Jump (78x58)S.png", 78, 58,    playerPivot) },
                { "player_fall",     SliceSprites("KingsAndPigsSprites/01-King Human/Fall (78x58)S.png", 78, 58,    playerPivot) },
                { "player_hit",      SliceSprites("KingsAndPigsSprites/01-King Human/Hit (78x58).png", 78, 58,      playerPivot) },
                { "player_dead",     SliceSprites("KingsAndPigsSprites/01-King Human/Dead (78x58).png", 78, 58,     playerPivot) },
                { "player_attack",   SliceSprites("KingsAndPigsSprites/01-King Human/Attack (78x58).png", 78, 58,   playerPivot) },
                { "player_door_in",  SliceSprites("KingsAndPigsSprites/01-King Human/Door In (78x58).png", 78, 58,  playerPivot, 2) },
                { "player_door_out", SliceSprites("KingsAndPigsSprites/01-King Human/Door Out (78x58).png", 78, 58, playerPivot, 1, 4) },

                { "door_opening",    SliceSprites("KingsAndPigsSprites/11-Door/Opening (46x56).png", 46, 56, vec2.Half, 1) },

                // Enemy: King pig
                { "kingpig_enemy_idle",   SliceSprites("KingsAndPigsSprites/02-King Pig/Idle (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },
                { "kingpig_enemy_run",    SliceSprites("KingsAndPigsSprites/02-King Pig/Run (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },
                { "kingpig_enemy_jump",   SliceSprites("KingsAndPigsSprites/02-King Pig/Jump (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },
                { "kingpig_enemy_fall",   SliceSprites("KingsAndPigsSprites/02-King Pig/Fall (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },
                { "kingpig_enemy_hit",    SliceSprites("KingsAndPigsSprites/02-King Pig/Hit (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },
                { "kingpig_enemy_dead",   SliceSprites("KingsAndPigsSprites/02-King Pig/Dead (38x28).png", 38, 28, new vec2(0.7f, 0.34f)) },
                { "kingpig_enemy_attack", SliceSprites("KingsAndPigsSprites/02-King Pig/Attack (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },

                // Enemy: Pig
                { "pig_enemy_idle",   SliceSprites("KingsAndPigsSprites/03-Pig/Idle (34x28).png",   34, 28, new vec2(0.58f, 0.34f)) },
                { "pig_enemy_run",    SliceSprites("KingsAndPigsSprites/03-Pig/Run (34x28).png",    34, 28, new vec2(0.58f, 0.34f)) },
                { "pig_enemy_jump",   SliceSprites("KingsAndPigsSprites/03-Pig/Jump (34x28).png",   34, 28, new vec2(0.58f, 0.34f)) },
                { "pig_enemy_fall",   SliceSprites("KingsAndPigsSprites/03-Pig/Fall (34x28).png",   34, 28, new vec2(0.58f, 0.34f)) },
                { "pig_enemy_hit",    SliceSprites("KingsAndPigsSprites/03-Pig/Hit (34x28).png",    34, 28, new vec2(0.58f, 0.34f)) },
                { "pig_enemy_dead",   SliceSprites("KingsAndPigsSprites/03-Pig/Dead (34x28).png",   34, 28, new vec2(0.58f, 0.34f)) },
                { "pig_enemy_attack", SliceSprites("KingsAndPigsSprites/03-Pig/Attack (34x28).png", 34, 28, new vec2(0.58f, 0.34f)) },

                // Full tilesets
                { "stark_full_tileset", SliceSprites("starkTileset.png", 16, 16, new vec2(0.5f, 0.5f)) },
                { "sunny_land_tileset", SliceSprites("Tilemap/SunnyLand_by_Ansimuz-extended.png", 16, 16, vec2.Half) },
                { "raven_ui_tileset", SliceSprites("RavenUI/RavenFantasy16x16.png", 16, 16, vec2.Half) },
                { "keyboard", SliceSprites("Keyboard Letters and Symbols.png", 16, 16, vec2.Half) },
                
                // Heart
                { "small_heart_idle", SliceSprites("KingsAndPigsSprites/12-Live and Coins/Small Heart Idle (18x14).png", 8, 7, vec2.Half) },
                { "health_bar_frame", SliceSprites("KingsAndPigsSprites/12-Live and Coins/Live Bar_atlas(143x34).png", 143, 34, vec2.Half) },

                // UI
                { "ui_buttons_long", SliceSprites("pixel-ui_buttons_long_47x14.png", 46, 14, vec2.Half) },

            };

            _tilesets["door_closing"] = _tilesets["door_opening"].Reverse().ToArray();

            // Collectible
            _tilesets["coin_currency"] = TakeSprites(_tilesets["stark_full_tileset"], 281, 4);
            _tilesets["coin_gray"] = TakeSprites(_tilesets["stark_full_tileset"], 277, 4);

            // Chest
            _tilesets["chest_normal_idle"] = TakeSprites(_tilesets["stark_full_tileset"], 160, 1);
            _tilesets["chest_normal_fill_open"] = TakeSprites(_tilesets["stark_full_tileset"], 164, 2);
            _tilesets["chest_normal_empty_open"] = TakeSprites(_tilesets["stark_full_tileset"], 166, 2);
            _tilesets["chest_small_idle"] = TakeSprites(_tilesets["stark_full_tileset"], 162, 1);
            _tilesets["chest_small_fill_open"] = TakeSprites(_tilesets["stark_full_tileset"], 168, 1);
            _tilesets["chest_small_empty_open"] = TakeSprites(_tilesets["stark_full_tileset"], 169, 1);

            // Spikes
            _tilesets["spikes_ground"] = TakeSprites(_tilesets["stark_full_tileset"], 183, 4);
            _tilesets["spikes_wall"] = TakeSprites(_tilesets["stark_full_tileset"], 215, 4);

            // Init sprites
            _sprites = new Dictionary<string, Sprite>()
            {
                { ItemId.coin_currency.ToString(),  _tilesets["raven_ui_tileset"][135] },
                { ItemId.boss_key.ToString(),  _tilesets["raven_ui_tileset"][183] },
                { ItemId.chest_key.ToString(),  _tilesets["raven_ui_tileset"][177] },
                { ItemId.simple_key.ToString(),  _tilesets["raven_ui_tileset"][176] },
                { ItemId.big_potion.ToString(),  _tilesets["raven_ui_tileset"][271] },
                { ItemId.normal_potion.ToString(),  _tilesets["raven_ui_tileset"][266] },
                { ItemId.small_potion.ToString(),  _tilesets["raven_ui_tileset"][264] },
                { "e_interactable", new Sprite(Assets.GetTexture("eInteract.png")) },
                { "e_interactable2", new Sprite(Assets.GetTexture("eInteract2.png")) },
                { "e_interactable3", new Sprite(Assets.GetTexture("eInteract3.png")) },
                { "inventory_slot", new Sprite(Assets.GetTexture("InventorySlot.png")) },
                { "outlineCircle", new Sprite(Assets.GetTexture("outlineCircle.png")) },
                { "portal_frame", new Sprite(Assets.GetTexture("portal_frame.png")) },
            };


        }

        public static Sprite[] GetAtlas(string atlasId)
        {
            return GetValueSafe(_tilesets, atlasId);
        }

        public static Sprite GetSprite(string spriteId)
        {
            return GetValueSafe(_sprites, spriteId);
        }

        private static T GetValueSafe<T>(Dictionary<string, T> dict, string key)
        {
            if (dict.TryGetValue(key, out var sprite))
            {
                return sprite;
            }

            return default;
        }

        private static Sprite[] TakeSprites(Sprite[] tileset, int startIndex, int length = int.MaxValue)
        {
            return GetSprites(tileset[0].Texture, startIndex, length);
        }

        private static Sprite[] GetSprites(Texture2D texture, int startIndex, int length = int.MaxValue)
        {
            length = int.Min(Assets.GetSpriteAtlas(texture.Path).SpriteCount - startIndex, length);

            var sprites = new Sprite[length];

            for (int i = 0; i < sprites.Length; ++i)
            {
                sprites[i] = new Sprite(i + startIndex, texture);
            }

            return sprites;
        }
        private static Sprite[] SliceSprites(string name, int width, int height, vec2 pivot, int startIndex = 0, int length = int.MaxValue)
        {
            var spriteAtlas = Assets.GetSpriteAtlas(name);

            if(length == int.MaxValue)
            {
                length = spriteAtlas.SpriteCount;
            }
            var sprites = new Sprite[length];

            for (int i = 0; i < length; i++)
            {
                sprites[i] = spriteAtlas.GetSprite(i + startIndex);
            }
            return sprites;
        }
    }
}