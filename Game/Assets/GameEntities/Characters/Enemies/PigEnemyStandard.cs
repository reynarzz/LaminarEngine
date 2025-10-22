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
            var size = new vec2(78, 58);
            var pivot = new vec2(0.4f, 0.4f);

            string[] pathSprites = ["KingsAndPigsSprites/01-King Human/Idle (78x58).png",
                                    "KingsAndPigsSprites/01-King Human/Run (78x58).png",
                                    "KingsAndPigsSprites/01-King Human/Jump (78x58).png",
                                    "KingsAndPigsSprites/01-King Human/Fall (78x58).png",
                                    "KingsAndPigsSprites/01-King Human/Hit (78x58).png",
                                    "KingsAndPigsSprites/01-King Human/Dead (78x58).png",
                                    "KingsAndPigsSprites/01-King Human/Attack (78x58).png"];

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

            if (Input.GetKeyDown(KeyCode.Enter))
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
