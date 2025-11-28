using Box2D.NET;
using Engine.Layers;
using Engine.Types;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class Collider2D : Component
#if DEBUG
        , ILateUpdatableComponent
#endif
    {
        public RigidBody2D AttachedRigidbody { get; internal set; }

        private float _rotationOffset;
        public float RotationOffset
        {
            get => _rotationOffset;
            set
            {
                _rotationOffset = value;
                UpdateShapeSafe();
            }
        }

        private B2ShapeId[] _shapesID;
        private B2ShapeDef _shapeDef;
        private vec2 _offset = new vec2(0, 0);
        private bool _isTrigger = false;

        protected ref B2ShapeDef ShapeDef => ref _shapeDef;
        internal B2ShapeId[] ShapesId => _shapesID;
        private B2Filter _defaultFilter = new B2Filter(B2Constants.B2_DEFAULT_CATEGORY_BITS,
                                                       B2Constants.B2_DEFAULT_MASK_BITS, 0);
        private B2BodyId _defaultBody;

        public override bool IsEnabled
        {
            get => base.IsEnabled;
            set
            {
                var canChange = value != base.IsEnabled;
                base.IsEnabled = value;
                if (B2Worlds.b2Body_IsValid(_defaultBody))
                {
                    if (value)
                    {
                        B2Bodies.b2Body_Enable(_defaultBody);
                    }
                    else
                    {
                        B2Bodies.b2Body_Disable(_defaultBody);
                    }
                }

                if (canChange && AreShapesValid())
                {
                    if (!value)
                    {
                        PhysicsLayer.ContactsDispatcher.NotifyColliderToRemove(this);
                    }
                    ApplyToShapesSafe(shape =>
                    {
                        B2Shapes.b2Shape_SetFilter(shape, value ? _defaultFilter : default);
                    });
                }
            }
        }

        public vec2 Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                UpdateShapeSafe();
            }
        }

        public bool IsTrigger
        {
            get => _isTrigger;
            set
            {
                _isTrigger = value;

                if (/*_isTrigger == value ||*/ !AreShapesValid())
                {
                    return;
                }

                var world = B2Worlds.b2GetWorldFromId(PhysicWorld.WorldID);

                var success = ApplyToShapesSafe(shapeid =>
                {
                    B2Shapes.b2Shape_EnableSensorEvents(shapeid, value);
                    B2Shapes.b2Shape_EnableContactEvents(shapeid, !value);

                    var shape = B2Shapes.b2GetShape(world, shapeid);
                    if (_shapeDef.isSensor)
                    {
                        B2Sensors.b2DestroySensor(PhysicWorld.World, shape);
                    }

                    _shapeDef.isSensor = value;

                    if (value)
                    {
                        /* Note: Couldn't find a way to enable/disable sensors without destroying the shape
                                 So i copied the internal logic of box2d to mark a shape as 'sensor', this works correctly here.
                        */
                        shape.sensorIndex = world.sensors.count;
                        B2Sensor sensor = new B2Sensor
                        {
                            hits = B2Arrays.b2Array_Create<B2Visitor>(4),
                            overlaps1 = B2Arrays.b2Array_Create<B2Visitor>(16),
                            overlaps2 = B2Arrays.b2Array_Create<B2Visitor>(16),
                            shapeId = shape.id
                        };
                        B2Arrays.b2Array_Push(ref world.sensors, sensor);
                    }
                    else
                    {
                        shape.sensorIndex = B2Constants.B2_NULL_INDEX;
                    }

                });
            }
        }

        private float _friction;
        public float Friction
        {
            get => AreShapesValid() ? B2Shapes.b2Shape_GetFriction(_shapesID[0]) : -1;
            set
            {
                _friction = value;
                ApplyToShapesSafe(shape =>
                {
                    B2Shapes.b2Shape_SetFriction(shape, value);
                });
            }
        }
        private float _bounciness;
        public float Bounciness
        {
            get => AreShapesValid() ? B2Shapes.b2Shape_GetRestitution(_shapesID[0]) : -1;
            set
            {
                _bounciness = value;
                ApplyToShapesSafe(shape =>
                {
                    B2Shapes.b2Shape_SetRestitution(shape, value);
                });
            }
        }

        public Bounds AABB
        {
            get
            {
                var bounds = new Bounds() { Min = vec3.One * int.MaxValue, Max = vec3.One * int.MinValue };
                var wId = B2Worlds.b2GetWorldFromId(PhysicWorld.WorldID);
                var defTransform = new B2Transform() { q = Transform.WorldRotation.QuatToB2Rot() };
                ApplyToShapesSafe(x =>
                {
                    var aabb = B2Shapes.b2ComputeShapeAABB(B2Shapes.b2GetShape(wId, x), defTransform);
                    bounds.Min = Mathf.Min(bounds.Min, aabb.lowerBound.ToVec3());
                    bounds.Max = Mathf.Max(bounds.Max, aabb.upperBound.ToVec3());
                });

                return bounds;
            }
        }

        public Bounds AABBWorld
        {
            get
            {
                var bounds = new Bounds() { Min = vec3.One * int.MaxValue, Max = vec3.One * int.MinValue };
                ApplyToShapesSafe(x =>
                {
                    var aabb = B2Shapes.b2Shape_GetAABB(x);
                    bounds.Min = Mathf.Min(bounds.Min, aabb.lowerBound.ToVec3());
                    bounds.Max = Mathf.Max(bounds.Max, aabb.upperBound.ToVec3());
                });

                return bounds;
            }
        }

        protected override void OnAwake()
        {
            AttachedRigidbody = GetComponentInParents<RigidBody2D>();
            Transform.OnChanged += Transform_OnChanged;
            Create();
        }

        internal void Create()
        {
            _shapeDef = new B2ShapeDef()
            {
                enableContactEvents = true,
                enableHitEvents = true,
                enableSensorEvents = true,
                // enablePreSolveEvents = false,
                invokeContactCreation = true,
                isSensor = _isTrigger,
                density = 1,
                updateBodyMass = true,
                material = B2Types.b2DefaultSurfaceMaterial(),
                filter = _defaultFilter,
                internalValue = B2Constants.B2_SECRET_COOKIE,
                userData = this,
                enableCustomFiltering = true
            };

            if (IsEnabled)
            {
                if (AttachedRigidbody)
                {
                    if (B2Worlds.b2Body_IsValid(_defaultBody))
                    {
                        B2Bodies.b2Body_Disable(_defaultBody);
                    }
                    AddShapesToBody(AttachedRigidbody.BodyId);
                    AttachedRigidbody.UpdateBody();
                }
                else
                {
                    CreateDefaultBody();
                    AddShapesToBody(_defaultBody);
                }
            }
        }

        private void CreateDefaultBody()
        {
            var bodyDef = B2Types.b2DefaultBodyDef();
            bodyDef.type = B2BodyType.b2_staticBody;
            bodyDef.position = Transform.WorldPosition.ToB2Vec2();
            bodyDef.rotation = Transform.WorldRotation.QuatToB2Rot();
            bodyDef.name = GetID().ToString();
            bodyDef.isBullet = false;
            bodyDef.isAwake = true;
            bodyDef.isEnabled = true;
            bodyDef.gravityScale = 1;
            bodyDef.enableSleep = false;
            _defaultBody = B2Bodies.b2CreateBody(PhysicWorld.WorldID, ref bodyDef);
        }

        private void AddShapesToBody(B2BodyId body)
        {
            DestroyShape();
            _shapesID = CreateShape(body);
        }

        protected abstract B2ShapeId[] CreateShape(B2BodyId bodyId);
        protected abstract void UpdateShape();
        protected void UpdateShapeSafe()
        {
            if (ShapesId != null && (AttachedRigidbody || B2Worlds.b2Body_IsValid(_defaultBody)))
            {
                UpdateShape();
            }
        }
        protected bool AreShapesValid()
        {
            if (_shapesID == null || _shapesID.Length == 0)
                return false;

            for (int i = 0; i < _shapesID.Length; i++)
            {
                if (!B2Worlds.b2Shape_IsValid(_shapesID[i]))
                    return false;
            }

            return true;
        }

        private bool ApplyToShapesSafe(Action<B2ShapeId> shapeApply)
        {
            if (!AreShapesValid())
                return false;

            for (int i = 0; i < _shapesID.Length; i++)
            {
                shapeApply(_shapesID[i]);
            }

            return true;
        }

        protected internal override void OnDestroy()
        {
            base.OnDestroy();
            if (AttachedRigidbody != null)
            {
                PhysicsLayer.ContactsDispatcher.NotifyColliderToRemove(this);
                AttachedRigidbody = null;
            }
            DestroyShape();
            B2Bodies.b2DestroyBody(_defaultBody);

            Transform.OnChanged -= Transform_OnChanged;
            _shapeDef = default;
        }

        private void Transform_OnChanged(Transform transform)
        {
            if (B2Worlds.b2Body_IsValid(_defaultBody) && B2Bodies.b2Body_IsEnabled(_defaultBody))
            {
                B2Bodies.b2Body_SetTransform(_defaultBody, transform.WorldPosition.ToB2Vec2(), transform.WorldRotation.QuatToB2Rot());
            }
        }

        private void DestroyShape()
        {
            if (_shapesID == null)
                return;

            for (int i = 0; i < _shapesID.Length; i++)
            {
                if (B2Worlds.b2Shape_IsValid(_shapesID[i]))
                {
                    var autoMass = AttachedRigidbody ? AttachedRigidbody.IsAutoMass : false;
                    B2Shapes.b2DestroyShape(_shapesID[i], autoMass);
                    _shapesID[i] = default;
                }
            }

            _shapesID = null;
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            if (IsEnabled && AreShapesValid())
            {
                ApplyToShapesSafe(shape =>
                {
                    B2Shapes.b2Shape_SetFilter(shape, _defaultFilter);
                });
            }

            if (B2Worlds.b2Body_IsValid(_defaultBody))
            {
                B2Bodies.b2Body_Enable(_defaultBody);
            }
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            if (IsEnabled && AreShapesValid())
            {
                ApplyToShapesSafe(shape =>
                {
                    B2Shapes.b2Shape_SetFilter(shape, default);
                });
            }

            if (B2Worlds.b2Body_IsValid(_defaultBody))
            {
                B2Bodies.b2Body_Disable(_defaultBody);
            }
        }
#if DEBUG
        void ILateUpdatableComponent.OnLateUpdate()
        {
            if (Physics2D.DrawColliders && ShapesId != null)
            {
                var transform = new B2Transform();
                var pos = new vec3(Transform.GetRenderingWorldMatrix()[3]);

                transform.p = new B2Vec2(pos.x, pos.y);
                transform.q = Transform.WorldRotation.QuatToB2Rot();

                for (int i = 0; i < ShapesId.Length; i++)
                {
                    var color = B2HexColor.b2_colorCyan;
                    if (IsTrigger)
                    {
                        color = B2HexColor.b2_colorYellow;
                    }

                    B2Worlds.b2DrawShape(PhysicWorld.DebugDraw, B2Shapes.b2GetShape(PhysicWorld.World, ShapesId[i]), transform, color);
                }
            }
        }
#endif
    }
}