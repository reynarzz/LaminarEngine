using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class AttackState<T> : StateBase<T> where T : AICharacter
    {
        public float WaitToAttack { get; set; } = 0.5f;
        private float _currentTimeToAttack;

        public AttackState() : base([new CelebrateState<T>()])
        {

        }
        public override void OnEnter()
        {
            _currentTimeToAttack = 0;
        }

        public override void OnUpdate()
        {
            Context.Walk(0);

            if (!Context.IsCharacterAlive())
            {
                return;
            }

            var dir = (Context.Target.Transform.WorldPosition - Context.Transform.WorldPosition);
            Context.LookAt(Math.Sign(dir.x));

            if (!Context.Target.IsCharacterAlive())
            {
                ChangeSubState<CelebrateState<T>>();
            }
            if (MathF.Abs(dir.y) > 2 && Context.Detector.IsTargetDetected)
            {
                Context.Jump();
            }
            else if (Math.Abs(dir.x) >= 2 || !Context.Target.IsCharacterAlive())
            {
                ReturnToParent();
            }

            var origin = Context.Transform.WorldPosition + new vec3(Context.SpriteLookDir * Context.Transform.LocalScale.x +
                                                                    Math.Sign(Context.Transform.LocalScale.x) * 0.5f, 0.2f);
            var size = new vec2(1.4f, 0.5f);

            if (Physics2D.DrawColliders)
            {
                Debug.DrawBox(origin, size, Color.Red);
            }

            if ((_currentTimeToAttack -= Time.DeltaTime) <= 0)
            {
                _currentTimeToAttack = WaitToAttack;
                var hit = Physics2D.BoxCast(origin, size, 0, LayerMask.NameToBit(GameLayers.PLAYER));

                if (hit.isHit)
                {
                    Context.Attack();
                }

                // Remove this, testing
                Task.Run(async () =>
                {
                    await Task.Delay(110);

                    if (hit.isHit)
                    {
                        Debug.Log("Attack Player");
                        hit.Collider.GetComponent<Player>().HitDamage(1);
                    }

                    if (!Context.Target.IsCharacterAlive())
                    {
                        ChangeSubState<CelebrateState<T>>();
                    }
                });

            }
        }
    }
}
