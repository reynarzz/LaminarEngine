using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{

    internal class ChaseState<T> : StateBase<T> where T : AICharacter
    {
        private float _timeToSearch = 3;
        public ChaseState() : base([])
        {
        }
        public override void OnEnter()
        {
            Context.Target = Actor.Find("Player").GetComponent<Character>();
            Context.Detector.Size = 13;
        }

        public override void OnUpdate()
        {
            if (!Context.IsCharacterAlive())
            {
                FSM.ChangeState<DeadState<T>>();
            }
            if (Context.Target)
            {
                var dir = Context.Transform.WorldPosition - Context.Target.Transform.WorldPosition;
                var transitionDist = 1.4f;

                if (Context.Detector.IsTargetDetected)
                {
                    _timeToSearch = 3;
                }
                else
                {
                    _timeToSearch -= Time.DeltaTime;
                }
                if ((!Context.Detector.IsTargetDetected && _timeToSearch <= 0) || !Context.Target.IsCharacterAlive())
                {
                    Context.Walk(0);
                    // ReturnToParent();

                    FSM.ChangeState<PatrolState<T>>();
                }
                // Context.LookAt(Math.Sign(dir.x));

                if (Math.Abs(dir.x) > transitionDist)
                {
                    Context.Walk(Math.Sign(dir.x));
                }
                else
                {
                    Context.Walk(0);
                    FSM.ChangeState<AttackState<T>>();
                }
            }
            else
            {
                Context.Jump();
            }
        }
    }
}
