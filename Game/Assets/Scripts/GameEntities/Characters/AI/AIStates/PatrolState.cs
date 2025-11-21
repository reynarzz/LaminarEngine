using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class PatrolState<T> : StateBase<T> where T : AICharacter
    {
        private float _timeToMove = 1;
        private float _timeMoving = 1;
        public PatrolState() : base([new ChaseState<T>()])
        {
            
        }

        public override void OnInit()
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
                //var engageDist = 5f;
                // var dir = (Context.Target.Transform.WorldPosition - Context.Transform.WorldPosition);
                
                if (Context.Detector.IsTargetDetected && Context.Target.IsCharacterAlive())
                {
                    FSM.ChangeState<ChaseState<T>>();
                }
                else
                {
                    // TODO: patrol here
                    Context.Walk(0);
                }
            }
        }

        public void OnExit(T context)
        {
        }
    }
}
