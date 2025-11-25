using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class PigEnemyStandard : EnemyBase
    {
        public override void Init(CharacterConfig config)
        {
            base.Init(config);

            const float fps = 11.5f;
            var size = new vec2(34, 28);
            var pivot = new vec2(0.58f, 0.34f);

            string[] atlasId = ["KingsAndPigsSprites/03-Pig/Idle (34x28).png",
                                    "KingsAndPigsSprites/03-Pig/Run (34x28).png",
                                    "KingsAndPigsSprites/03-Pig/Jump (34x28).png",
                                    "KingsAndPigsSprites/03-Pig/Fall (34x28).png",
                                    "KingsAndPigsSprites/03-Pig/Hit (34x28).png",
                                    "KingsAndPigsSprites/03-Pig/Dead (34x28).png",
                                    "KingsAndPigsSprites/03-Pig/Attack (34x28).png"];

            var states = new AnimationsStates();
            for (int i = 0; i < AnimationsStates.Length; i++)
            {
                states[i] = new AnimationStateInfo()
                {
                    IsEnabled = true,
                    Fps = fps,
                    SpriteAtlasPath = atlasId[i],
                };
            }
            InitAnimationStates(states);
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Jump();
            }

            if (Input.GetKey(KeyCode.J))
            {
                Walk(-1);
            }
            else if (Input.GetKey(KeyCode.L))
            {
                Walk(1);
            }
            else
            {
                Walk(0);
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                Attack();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Death();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                HitDamage(1);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                Restart();
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

        }
    }
}
