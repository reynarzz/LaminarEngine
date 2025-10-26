using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class Player : Character
    {
        public override void Init(CharacterConfig config)
        {
            base.Init(config);
            var box = AddComponent<BoxCollider2D>();
            box.Size = new vec2(0.95f, 0.6f);
            box.Offset = new vec2(0, 0.8f);
            box.Friction = 0;

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

            states.Attack.Events = [new AnimEvent { Time = 0, Callback = PlayAttackSoundFx }];
            states.Walk.Events = [new AnimEvent { Time = 0, Callback = PlayWalkSoundFx }, new AnimEvent { Time = (1.0f / fps) * 4, Callback = PlayWalkSoundFx }];
            states.Jump.Events = [new AnimEvent { Time = 0, Callback = PlayJumpSoundFx }];

            InitAnimationStates(states);
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            var origin = Transform.WorldPosition + new vec3(Transform.LocalScale.x + Math.Sign(Transform.LocalScale.x) * 0.5f, 0.2f);
            var size = new vec2(2.5f, 3);

            if (Input.GetKeyDown(KeyCode.F))
            {
                var hit = Physics2D.BoxCast(origin, size, 0, LayerMask.NameToBit(GameLayers.ENEMY));
                if (hit.isHit)
                {
                    Debug.Log("Attack enemy");
                    hit.Collider.GetComponent<EnemyBase>().HitDamage(1);
                }
                Attack();
            }

            Debug.DrawBox(origin, size, Color.Green);

            if (Input.GetKeyDown(KeyCode.X))
            {
                Death();
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                HitDamage(1);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Restart();
            }

            if (Input.GetKey(KeyCode.A))
            {
                Walk(-1);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Walk(1);
            }
            else
            {
                Walk(0);
            }

        }

        public override void OnFixedUpdate()
        {

            base.OnFixedUpdate();
        }
    }
}
