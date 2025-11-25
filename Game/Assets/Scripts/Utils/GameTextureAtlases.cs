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
    public static class GameTextureAtlases
    {
        private readonly static Dictionary<string, Sprite[]> _tilesets;
        static GameTextureAtlases()
        {
            var size = new vec2(78, 58);
            var pivot = new vec2(0.4f, 0.4f);

            _tilesets = new Dictionary<string, Sprite[]>()
            {
                // Player atlases
                { "player_idle",   GetSprites("KingsAndPigsSprites/01-King Human/Idle (78x58)S.png", 78, 58, new vec2(0.4f, 0.4f)) },
                { "player_run",    GetSprites("KingsAndPigsSprites/01-King Human/Run (78x58)S.png", 78, 58, new vec2(0.4f, 0.4f)) },
                { "player_jump",   GetSprites("KingsAndPigsSprites/01-King Human/Jump (78x58)S.png", 78, 58, new vec2(0.4f, 0.4f)) },
                { "player_fall",   GetSprites("KingsAndPigsSprites/01-King Human/Fall (78x58)S.png", 78, 58, new vec2(0.4f, 0.4f)) },
                { "player_hit",    GetSprites("KingsAndPigsSprites/01-King Human/Hit (78x58).png", 78, 58, new vec2(0.4f, 0.4f)) },
                { "player_dead",   GetSprites("KingsAndPigsSprites/01-King Human/Dead (78x58).png", 78, 58, new vec2(0.4f, 0.4f)) },
                { "player_attack",  GetSprites("KingsAndPigsSprites/01-King Human/Attack (78x58).png", 78, 58, new vec2(0.4f, 0.4f)) },



                { "player_door_in",  GetSprites("KingsAndPigsSprites/01-King Human/Door In (78x58).png", 78, 58, new vec2(0.4f, 0.4f), 2) },
                { "player_door_out", GetSprites("KingsAndPigsSprites/01-King Human/Door Out (78x58).png", 78, 58, new vec2(0.4f, 0.4f), 2) },
                
                
                { "door_opening",    GetSprites("KingsAndPigsSprites/11-Door/Opening (46x56).png", 46, 56, vec2.Half, 1) },




                // King pig atlases
                { "kingpig_enemy_idle",   GetSprites("KingsAndPigsSprites/02-King Pig/Idle (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },
                { "kingpig_enemy_run",    GetSprites("KingsAndPigsSprites/02-King Pig/Run (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },
                { "kingpig_enemy_jump",   GetSprites("KingsAndPigsSprites/02-King Pig/Jump (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },
                { "kingpig_enemy_fall",   GetSprites("KingsAndPigsSprites/02-King Pig/Fall (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },
                { "kingpig_enemy_hit",    GetSprites("KingsAndPigsSprites/02-King Pig/Hit (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },
                { "kingpig_enemy_dead",   GetSprites("KingsAndPigsSprites/02-King Pig/Dead (38x28).png", 38, 28, new vec2(0.7f, 0.34f)) },
                { "kingpig_enemy_attack", GetSprites("KingsAndPigsSprites/02-King Pig/Attack (38x28).png", 38, 28, new vec2(0.52f, 0.34f)) },




                
            };

            _tilesets["door_closing"] = _tilesets["door_opening"].Reverse().ToArray();

        }

        public static Sprite[] GetAtlas(string name)
        {
            if (_tilesets.TryGetValue(name, out var sprite))
            {
                return sprite;
            }

            return null;
        }

        private static Sprite[] GetSprites(string name, int width, int height)
        {
            return GetSprites(name, width, height, vec2.Half);
        }
        private static Sprite[] GetSprites(string name, int width, int height, vec2 pivot, int startIndex = 0, int length = int.MaxValue)
        {
            return TextureAtlasUtils.SliceSprites(Assets.GetTexture(name), width, height, pivot, startIndex, length);
        }
    }
}
