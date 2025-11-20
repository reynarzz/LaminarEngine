using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class Platform : ScriptBehavior
    {
        public vec3[] Points = [ new vec3(-22.5f, -9, 0), new vec3(-22.5f, 6, 0), new vec3(-22.5f, 15, 0)];
        public float Speed { get; set; } = 4f;
        public float WaitTime { get; set; } = 1.5f;
        private float _currentWait = 0;

        private SpriteRenderer _renderer;
        private int _pointIndex = 0;
        private int _direction = 1;

        private vec3 _startPos;
        private Animator _animator;
        public override void OnStart()
        {
            base.OnStart();
            _renderer = AddComponent<SpriteRenderer>();
            _renderer.Color = Color.Green;
            var rigid = AddComponent<RigidBody2D>();
            rigid.BodyType = Body2DType.Kinematic;
            rigid.Interpolate = true;
            var trigger = AddComponent<BoxCollider2D>();
            trigger.Size = new vec2(3, 1);
            trigger.Offset = new vec2(0, 0.1f);
            trigger.IsTrigger = true;
            Transform.LocalScale = new vec3(trigger.Size.x, trigger.Size.y, 1);
            _startPos = Transform.LocalPosition = new vec3(-8, 0, 0);

            AddComponent<BoxCollider2D>().Size = trigger.Size;
          
            Transform.WorldPosition = _startPos + Points[_pointIndex];
            _currentWait = WaitTime;
            Debug.Log("Platform start");
            //SetAnimator();
        }

        private void SetAnimator()
        {
            _animator = AddComponent<Animator>();

            var moveClip = new AnimationClip("MoveAnim");
            var pointsMoveCurve = new Vec2EasingCurve();
            pointsMoveCurve.EasingType = EasingType.EaseInOutSine;

            var fps = 0.2f;
            var unit = 1.0f / fps;
            var waitTime = 1;
            for (int i = 0; i < Points.Length; i++)
            {
                var point = _startPos + Points[i];
                pointsMoveCurve.AddKeyFrame(unit * (float)i, point);
                pointsMoveCurve.AddKeyFrame(unit * (float)i + waitTime, point);
            }
            pointsMoveCurve.AddKeyFrame(unit * Points.Length, _startPos + Points[0]);

            moveClip.AddCurve("Move", pointsMoveCurve);
            var state = new AnimationState("Move", moveClip);
            _animator.AddState(state);

        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if(_animator != null )
            {
                Transform.WorldPosition = _animator.GetVec2("Move");
            }
            else
            {
                var target = _startPos + Points[_pointIndex];

                Transform.WorldPosition = Mathf.MoveTowards(Transform.WorldPosition, target, Time.DeltaTime * Speed);

                var distance = Mathf.Distance(Transform.WorldPosition, target);
                if (distance < 0.001f && (_currentWait -= Time.DeltaTime) <= 0)
                {
                    _currentWait = WaitTime;
                    if (_pointIndex + 1 >= Points.Length)
                    {
                        _direction = -1;
                    }
                    else if (_pointIndex <= 0)
                    {
                        _direction = 1;
                    }

                    _pointIndex += _direction;
                }
            }
        }

        public override void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.Actor.Layer == LayerMask.NameToLayer("Player"))
            {
                collider.Actor.Transform.Parent = Transform;

                if(collider.RigidBody.Velocity.y <= 0)
                {
                    collider.RigidBody.Velocity = new vec2(collider.RigidBody.Velocity.x, 0);
                }


                // Actor.Destroy(collider.Actor);
                Debug.Log("Enter player to platform");
            }
        }

        public override void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.Actor.Layer == LayerMask.NameToLayer("Player"))
            {
                collider.Actor.Transform.Parent = null;
            }
        }
    }
}
