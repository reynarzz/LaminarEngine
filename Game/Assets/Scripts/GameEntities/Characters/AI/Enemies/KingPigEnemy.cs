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
            Detector.Size = 7;
            _stateMachine = new StateMachine<KingPigEnemy>(this, 
                [new PatrolState<KingPigEnemy>(), new ChaseState<KingPigEnemy>(),
                 new AttackState<KingPigEnemy>(), new CelebrateState<KingPigEnemy>(),
                 new DeadState<KingPigEnemy>()]);

            _stateMachine.SetInitialState<PatrolState<KingPigEnemy>>();

            string[] atlasId = ["kingpig_enemy_idle",
                                "kingpig_enemy_run",
                                "kingpig_enemy_jump",
                                "kingpig_enemy_fall",
                                "kingpig_enemy_hit",
                                "kingpig_enemy_dead",
                                "kingpig_enemy_attack",
                                ];

            var states = new AnimationsStates();
            for (int i = 0; i < AnimationsStates.Length; i++)
            {
                states[i] = new AnimationStateInfo()
                {
                    IsEnabled = true,
                    Fps = 10.0f,
                    SpriteAtlasId = atlasId[i],
                };
            }

            InitAnimationStates(states);
        }

        public override bool HitDamage(vec3 aggressorPos, int amount)
        {
            var isHit = base.HitDamage(aggressorPos, amount);

            if(isHit)
            _stateMachine.ChangeState<ChaseState<KingPigEnemy>>();

            return isHit;
        }
        protected override void OnUpdate()
        {
            _stateMachine?.OnUpdate();
        }
    }
}
