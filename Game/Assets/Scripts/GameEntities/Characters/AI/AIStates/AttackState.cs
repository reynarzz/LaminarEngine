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
        public AttackState() : base([]) { }
        public override void OnEnter()
        {
            _currentTimeToAttack = 0;
        }

        public override void OnUpdate()
        {
            Context.Walk(0);

            if (!Context.IsCharacterAlive())
            {
                FSM.ChangeState<DeadState<T>>();
                return;
            }

            var dir = Context.Target.Transform.WorldPosition - Context.Transform.WorldPosition;
            Context.LookAt(Math.Sign(dir.x));

            if (!Context.Target.IsCharacterAlive())
            {
                FSM.ChangeState<CelebrateState<T>>();
                return;

            }
            if (MathF.Abs(dir.y) > 2 && Context.Detector.IsTargetDetected)
            {
                Context.Jump();
            }
            else if (Math.Abs(dir.x) >= 2 || !Context.Target.IsCharacterAlive())
            {
                FSM.ChangeState<ChaseState<T>>();
                return;
            }

            var origin = Context.Transform.WorldPosition + new vec3(Context.LookDir -
                                                                    Math.Sign(Context.LookDir) * 0.5f, 0.0f);
            var size = new vec2(1.5f, 1.2f);

            if (Physics2D.DrawColliders)
            {
                Debug.DrawBox(origin, size, Color.Red);
            }

            if ((_currentTimeToAttack -= Time.DeltaTime) <= 0)
            {
                _currentTimeToAttack = WaitToAttack;

                bool TryFindPlayerInRange(out Player player)
                {
                    player = null;
                    var hits = Physics2D.BoxCastAll(origin, size, LayerMask.NameToBit(GameConsts.PLAYER));
                    for (int i = 0; i < hits.Length; i++)
                    {
                        var hit = hits[i];
                        if (hit.isHit)
                        {
                            player = hit.Collider.GetComponent<Player>();

                            if (player && !player.IsEnteringThroughDoor)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }

                if (TryFindPlayerInRange(out var player))
                {
                    Context.Attack();

                    // Remove this, testing
                    Task.Run(async () =>
                    {
                        await Task.Delay(110);

                        if (!Context.Target.IsCharacterAlive())
                        {
                            FSM.ChangeState<CelebrateState<T>>();
                            return;
                        }

                        if (TryFindPlayerInRange(out player))
                        {
                            Debug.Log("Attack Player");
                            player?.HitDamage(Context, 1);
                        }
                    });
                }
            }
        }
    }
}
