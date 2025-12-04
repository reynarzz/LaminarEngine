using Engine;
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
            if (!Context.IsCharacterAlive())
            {
                FSM.ChangeState<DeadState<T>>();
            }
            Context.Walk(0);

            if ((_celebrationWaitTime -= Time.DeltaTime) > 0)
                return;

            if (!Context.Target.IsCharacterAlive())
            {
                Context.Jump();
            }
            else
            {
                FSM.ChangeState<AttackState<T>>();
            }
        }
    }
}
