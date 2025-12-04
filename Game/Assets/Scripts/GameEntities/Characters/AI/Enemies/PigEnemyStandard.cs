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
            
            string[] atlasId = [
                                
                                
                                
                                
                                
                                ];

            var states = new AnimationsStates();
            for (int i = 0; i < AnimationsStates.Length; i++)
            {
                states[i] = new AnimationStateInfo()
                {
                    IsEnabled = true,
                    Fps = fps,
                    SpriteAtlasId = atlasId[i],
                };
            }
            InitAnimationStates(states);
        }

        protected override void OnUpdate()
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
                HitDamage(this, 1);
            }
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

        }
    }
}
