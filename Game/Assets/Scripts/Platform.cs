using Engine;
using Engine.Types;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [RequiredComponent(typeof(SpriteRenderer), typeof(RigidBody2D), typeof(BoxCollider2D))]
    internal class Platform : GameEntity
    {
        private vec2[] _positions;
        public float Speed { get; set; } = 4f;
        public float WaitTime { get; set; } = 1.5f;
        private float _currentWait = 0;

        [RequiredProperty] private SpriteRenderer _renderer;
        [RequiredProperty] private RigidBody2D _rigid;

        private int _pointIndex = 0;
        private int _direction = 1;

        private Animator _animator;
        protected override void OnAwake()
        {
            base.OnAwake();
            _renderer.Color = Color.Green;
            _rigid.BodyType = Body2DType.Kinematic;
            _rigid.Interpolate = true;
            var trigger = GetComponent<BoxCollider2D>();
            trigger.Size = new vec2(3, 1);
            trigger.Offset = new vec2(0, 0.1f);
            trigger.IsTrigger = true;
            Transform.LocalScale = new vec3(trigger.Size.x, trigger.Size.y, 1);

            AddComponent<BoxCollider2D>().Size = trigger.Size;
            
            _currentWait = WaitTime;
        }

        public void Init(vec2 position, vec2[] positions)
        {
            _positions = positions;
            _pointIndex = 0;
            Transform.WorldPosition = positions[0];
        }

        //private void SetAnimator()
        //{
        //    _animator = AddComponent<Animator>();

        //    var moveClip = new AnimationClip("MoveAnim");
        //    var pointsMoveCurve = new Vec2EasingCurve();
        //    pointsMoveCurve.EasingType = EasingType.EaseInOutSine;

        //    var fps = 0.2f;
        //    var unit = 1.0f / fps;
        //    var waitTime = 1;
        //    for (int i = 0; i < _heights.Length; i++)
        //    {
        //        var point = _startPos + vec3.Up * _heights[i];
        //        pointsMoveCurve.AddKeyFrame(unit * (float)i, point);
        //        pointsMoveCurve.AddKeyFrame(unit * (float)i + waitTime, point);
        //    }
        //    pointsMoveCurve.AddKeyFrame(unit * _heights.Length, _startPos + vec3.Up * _heights[0]);

        //    moveClip.AddCurve("Move", pointsMoveCurve);
        //    var state = new AnimationState("Move", moveClip);
        //    _animator.AddState(state);
        //}

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if(_animator != null )
            {
                Transform.WorldPosition = _animator.GetVec2("Move");
            }
            else
            {
                var target = /*_startPos +*/ _positions[_pointIndex];

                Transform.WorldPosition = Mathf.MoveTowards((vec2)Transform.WorldPosition, target, Time.DeltaTime * Speed);

                var distance = Mathf.Distance((vec2)Transform.WorldPosition, target);
                if (distance < 0.001f && (_currentWait -= Time.DeltaTime) <= 0)
                {
                    _currentWait = WaitTime;
                    if (_pointIndex + 1 >= _positions.Length)
                    {
                        _direction = -1;
                    }
                    else if (_pointIndex <= 0)
                    {
                        _direction = 1;
                    }

                    if(_pointIndex + _direction >= 0)
                    {
                        _pointIndex += _direction;
                    }
                }
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.Actor.Layer == LayerMask.NameToLayer("Player"))
            {
                collider.Actor.Transform.Parent = Transform;

                if(collider.AttachedRigidbody.Velocity.y <= 0)
                {
                    collider.AttachedRigidbody.Velocity = new vec2(collider.AttachedRigidbody.Velocity.x, 0);
                }


                // Actor.Destroy(collider.Actor);
                Debug.Log("Enter player to platform");
            }
        }

        protected override void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.Actor.Layer == LayerMask.NameToLayer("Player"))
            {
                collider.Actor.Transform.Parent = null;
            }
        }
    }
}