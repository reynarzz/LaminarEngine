using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class KingPigEnemy : EnemyBase
    {
        public override void Init(CharacterConfig config)
        {
            base.Init(config);

            const float fps = 11.5f;
            var size = new vec2(38, 28);
            var pivot = new vec2(0.52f, 0.34f);

            string[] pathSprites = ["KingsAndPigsSprites/02-King Pig/Idle (38x28).png",
                                    "KingsAndPigsSprites/02-King Pig/Run (38x28).png",
                                    "KingsAndPigsSprites/02-King Pig/Jump (38x28).png",
                                    "KingsAndPigsSprites/02-King Pig/Fall (38x28).png",
                                    "KingsAndPigsSprites/02-King Pig/Hit (38x28).png",
                                    "KingsAndPigsSprites/02-King Pig/Dead (38x28).png",
                                    "KingsAndPigsSprites/02-King Pig/Attack (38x28).png"];

            var states = new AnimationsStates();
            for (int i = 0; i < AnimationsStates.Length; i++)
            {
                states[i] = new AnimationStateInfo()
                {
                    IsEnabled = true,
                    Fps = fps,
                    Pivot = pivot,
                    Size = size,
                    SpriteAtlasPath = pathSprites[i],
                };
            }
            InitAnimationStates(states);
        }

        public override void OnUpdate()
        {
            Walk(0);
        }
    }
}
