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
        public override void OnStart()
        {
            const float fps = 11;
            var size = new vec2(78, 58);
            var pivot = new vec2(0.4f, 0.4f);

            var toJump = new AnimatorTransition("Jump", [new IntCondition(VEL_Y_PROP_NAME, 0, IntOp.GreaterThan), 
                                                         new BoolCondition(ON_GROUND_PROPERTY_NAME, false)]);

            var toFall = new AnimatorTransition("Fall", [new IntCondition(VEL_Y_PROP_NAME, 0, IntOp.LessThan), 
                                                         new BoolCondition(ON_GROUND_PROPERTY_NAME, false)]);

            var toIdle = new AnimatorTransition("Idle", [new IntCondition(VEL_X_PROP_NAME, 0, IntOp.Equal), 
                                                         new BoolCondition(ON_GROUND_PROPERTY_NAME, true)]);

            var toRun = new AnimatorTransition("Run",  [new IntCondition(VEL_X_PROP_NAME, 0, IntOp.NotEqual),
                                                        new BoolCondition(ON_GROUND_PROPERTY_NAME, true)]);

            var toAttack = new AnimatorTransition("Attack", new TriggerCondition(Attacks[0]));
            var toDeath = new AnimatorTransition("Death", new TriggerCondition(DEATH_PROPERTY_NAME));
            var toHit = new AnimatorTransition("Hit", new TriggerCondition(HIT_DAMAGE_PROPERTY_NAME));

            AddSpriteAnimState("Idle", true,true,false, [toRun, toJump, toFall, toAttack, toDeath, toHit], "KingsAndPigsSprites/01-King Human/Idle (78x58).png", fps, size, pivot);
            AddSpriteAnimState("Run", false, true, false, [toIdle, toJump, toFall, toAttack, toDeath, toHit], "KingsAndPigsSprites/01-King Human/Run (78x58).png", fps, size, pivot);
            AddSpriteAnimState("Jump", false, true, false, [toIdle, toFall, toAttack, toHit], "KingsAndPigsSprites/01-King Human/Jump (78x58).png", fps, size, pivot);
            AddSpriteAnimState("Fall", false, true, false, [toIdle, toRun, toAttack, toHit], "KingsAndPigsSprites/01-King Human/Fall (78x58).png", fps, size, pivot);
            AddSpriteAnimState("Attack", false, false, true, [toIdle, toRun, toFall, toDeath, toHit], "KingsAndPigsSprites/01-King Human/Attack (78x58).png", fps, size, pivot);
            AddSpriteAnimState("Death", false, false, true, null, "KingsAndPigsSprites/01-King Human/Dead (78x58).png", fps, size, pivot);
            AddSpriteAnimState("Hit", false, false, true, [toIdle, toRun, toJump, toFall, toAttack, toDeath], "KingsAndPigsSprites/01-King Human/Hit (78x58).png", fps, size, pivot);


        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
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

            if (Input.GetKeyDown(KeyCode.F))
            {
                Attack();
            }

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

        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            var length = 0.7f;
            var yOffset = 0;//-0.55f;
            var origin1 = Transform.WorldPosition + new vec3(-0.45f, yOffset, 0);
            var origin2 = Transform.WorldPosition + new vec3(0.45f, yOffset, 0);

            var hitA = Physics2D.Raycast(origin1, Transform.Down * length, LayerMask.NameToBit("Floor") | LayerMask.NameToBit("Platform"));
            var hitB = Physics2D.Raycast(origin2, Transform.Down * length, LayerMask.NameToBit("Floor") | LayerMask.NameToBit("Platform"));

            var color1 = Color.White;
            var color2 = Color.White;

            IsOnGround = hitA.isHit || hitB.isHit;

            if (hitA.isHit || hitB.isHit)
            {
                if (hitA.isHit)
                {
                    color1 = Color.Red;
                }

                if (hitB.isHit)
                    color2 = Color.Red;
            }

            if (Physics2D.DrawColliders)
            {
                Debug.DrawRay(origin1, Transform.Down * length, color1);
                Debug.DrawRay(origin2, Transform.Down * length, color2);
            }
        }
    }
}
