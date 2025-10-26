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
        private StateMachine<KingPigEnemy> _stateMachine;

        public override void Init(CharacterConfig config)
        {
            base.Init(config);

            _stateMachine = new StateMachine<KingPigEnemy>(this, [new PatrolState<KingPigEnemy>()]);
            _stateMachine.SetInitialState<PatrolState<KingPigEnemy>>();

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

            var death = states[5];
            death.Pivot.x = 0.7f;
            states[5] = death;
            InitAnimationStates(states);

            Transform.LocalScale = new vec3(5, 5, 5);
        }

        public override void OnUpdate()
        {
            _stateMachine.OnUpdate();
        }
    }
}
