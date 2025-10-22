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

            var toJump = new AnimatorTransition("Jump", new IntCondition(VELOCITY_Y_PROPERTY_NAME, 0, IntOp.GreaterThan));
            var toFall = new AnimatorTransition("Fall", new IntCondition(VELOCITY_Y_PROPERTY_NAME, 0, IntOp.LessThan));
            var toIdle = new AnimatorTransition("Idle", new IntCondition(VELOCITY_Y_PROPERTY_NAME, 0, IntOp.LessThanOrEqual));
            var toRun = new AnimatorTransition("Run",  [new IntCondition(VELOCITY_X_PROPERTY_NAME, 0, IntOp.NotEqual),
                                                        new IntCondition(VELOCITY_Y_PROPERTY_NAME, 0, IntOp.LessThanOrEqual)]);

            AddSpriteAnimState("Idle", true, [toRun, toJump], "KingsAndPigsSprites/01-King Human/Idle (78x58).png", fps, size, pivot);
            AddSpriteAnimState("Run", false, [toIdle, toJump], "KingsAndPigsSprites/01-King Human/Run (78x58).png", fps, size, pivot);
            AddSpriteAnimState("Jump", false, [toFall], "KingsAndPigsSprites/01-King Human/Jump (78x58).png", fps, size, pivot);
            AddSpriteAnimState("Fall", false, [toIdle, toRun], "KingsAndPigsSprites/01-King Human/Fall (78x58).png", fps, size, pivot);
            AddSpriteAnimState("Attack", false, null, "KingsAndPigsSprites/01-King Human/Attack (78x58).png", fps, size, pivot);
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

            if (Input.GetKey(KeyCode.F))
            {
                Walk(-1);
            }

            base.OnUpdate();


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
            if (hitA.isHit || hitB.isHit)
            {
                if (hitA.isHit)
                {
                }
            }
        }
    }
}
