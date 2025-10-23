using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class CelebrateState<T> : StateBase<T> where T : EnemyBase
    {
        public float CelebrationWait { get; set; } = 1.3f;
        private float _celebrationWaitTime = 0;

        public override void OnEnter()
        {
            _celebrationWaitTime = CelebrationWait;
        }
        public override void OnUpdate()
        {
            if ((_celebrationWaitTime -= Time.DeltaTime) > 0)
                return;

            if (!Context.Target.IsCharacterAlive())
            {
                Context.Walk(0);
                Context.Jump();
            }
            else
            {
                ReturnToParent();
            }
        }
    }
    internal class AttackState<T> : StateBase<T> where T : EnemyBase
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

            var dir = (Context.Target.Transform.WorldPosition - Context.Transform.WorldPosition);
            Context.LookAt(Math.Sign(dir.x));


            if (!Context.Target.IsCharacterAlive())
            {
                ChangeSubState<CelebrateState<T>>();
            }

            if (Math.Abs(dir.x) >= 2 || !Context.Target.IsCharacterAlive())
            {
                ReturnToParent();
            }

            if ((_currentTimeToAttack -= Time.DeltaTime) <= 0)
            {
                _currentTimeToAttack = WaitToAttack;
                Context.Attack();

                // Remove this, testing
                Task.Run(async () => { await Task.Delay(100); Context.Target.HitDamage(1);
                    if (!Context.Target.IsCharacterAlive())
                    {
                        ChangeSubState<CelebrateState<T>>();
                    }
                });

            }
        }
    }
    internal class ChaseState<T> : StateBase<T> where T : EnemyBase
    {
        public ChaseState() : base([new AttackState<T>()])
        {

        }
        public override void OnEnter()
        {
            Context.Target = Actor.Find("Player").GetComponent<Character>();
        }

        public override void OnUpdate()
        {
            if (Context.Target)
            {
                var dir = Context.Target.Transform.WorldPosition - Context.Transform.WorldPosition;
                var transitionDist = 1.7f;
                var desengageDist = 8;

                if (Math.Abs(dir.x) >= desengageDist || !Context.Target.IsCharacterAlive())
                {
                    Context.Walk(0);
                    ReturnToParent();
                }

                if (Math.Abs(dir.x) > transitionDist)
                {
                    Context.Walk(Math.Sign(dir.x));
                }
                else
                {
                    Context.Walk(0);
                    Context.LookAt(Math.Sign(dir.x));

                    if (dir.Magnitude <= transitionDist)
                    {
                        ChangeSubState<AttackState<T>>();
                    }
                }

            }
            else
            {
                Context.Jump();
            }
        }
    }
}
