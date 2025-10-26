using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class CelebrateState<T> : StateBase<T> where T : AICharacter
    {
        public float CelebrationWait { get; set; } = 1.3f;
        private float _celebrationWaitTime = 0;

        public override void OnEnter()
        {
            _celebrationWaitTime = CelebrationWait;
        }
        public override void OnUpdate()
        {
            Context.Walk(0);

            if ((_celebrationWaitTime -= Time.DeltaTime) > 0)
                return;

            if (!Context.Target.IsCharacterAlive())
            {
                Context.Jump();
            }
            else
            {
                ReturnToParent();
            }
        }
    }
    
    internal class ChaseState<T> : StateBase<T> where T : AICharacter
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
                var dir = Context.Transform.WorldPosition - Context.Target.Transform.WorldPosition;
                var transitionDist = 1.7f;

                if (!Context.Detector.IsTargetDetected || !Context.Target.IsCharacterAlive())
                {
                    Context.Walk(0);
                    ReturnToParent();
                }
                Context.LookAt(Math.Sign(dir.x));

                if (Math.Abs(dir.x) > transitionDist)
                {
                    Context.Walk(Math.Sign(dir.x));
                }
                else
                {
                    Context.Walk(0);

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
