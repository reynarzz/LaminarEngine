using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2D.NET;
using Engine.Layers;
using Engine.Types;
using Engine.Utils;
using GlmNet;

namespace Engine
{
    public enum Body2DType
    {
        Static,
        Kinematic,
        Dynamic
    }

    public enum ForceMode2D
    {
        Force,
        Impulse
    }

    [UniqueComponent]
    public class RigidBody2D : Component
    {
        private B2BodyId _bodyId;
        private Body2DType _bodyType = Body2DType.Dynamic;
        private bool _isContinuos = false;
        private bool _canSleep = true;
        private bool _isAutoMass = false;
        private float _userMassValue = 1.0f;
        private bool _isZRotationLocked = false;
        private bool _shouldUpdatePreTransformation = false;
        private float _gravityScale = 1;
        private float _angularDamping = 0;
        private readonly static mat4 _identity = mat4.identity();
        internal B2BodyId BodyId => _bodyId;

        [SerializedField]
        public Body2DType BodyType
        {
            get => _bodyType;
            set
            {
                if (_bodyType == value) return;
                _bodyType = value;

                if (!IsValidBody())
                    return;

                    B2Bodies.b2Body_SetType(_bodyId, (B2BodyType)_bodyType);
            }
        }

        private bool _interpolate = false;
        [SerializedField]
        public bool Interpolate
        {
            get => _interpolate;
            set
            {
                _interpolate = value;
                Transform.NeedsInterpolation = value;
            }
        }

        public override bool IsEnabled
        {
            get => base.IsEnabled;
            set
            {
                if (IsValidBody())
                {
                    if (value)
                    {
                        B2Bodies.b2Body_Enable(_bodyId);
                    }
                    else
                    {
                        B2Bodies.b2Body_Disable(_bodyId);
                    }
                }

                base.IsEnabled = value;
            }
        }

        [SerializedField]
        public bool LockZRotation
        {
            get => _isZRotationLocked;
            set
            {
                if (_isZRotationLocked == value)
                    return;
                _isZRotationLocked = value;

                if (!IsValidBody())
                    return;

                B2Bodies.b2Body_SetMotionLocks(_bodyId, new B2MotionLocks() { angularZ = value });
            }
        }

        [SerializedField]
        public bool IsContinuos
        {
            get => _isContinuos;
            set
            {
                if (_isContinuos == value) return;
                _isContinuos = value;

                if (!IsValidBody())
                    return;

                B2Bodies.b2Body_SetBullet(_bodyId, _isContinuos);
            }
        }

        [SerializedField]
        public bool CanSleep
        {
            get => _canSleep;
            set
            {
                if (_canSleep == value)
                    return;
                _canSleep = value;

                if (!IsValidBody())
                    return;

                B2Bodies.b2Body_EnableSleep(_bodyId, _canSleep);
            }
        }
        [SerializedField]
        public bool IsAutoMass
        {
            get => _isAutoMass;
            set
            {
                _isAutoMass = value;

                if (!IsValidBody())
                    return;

                if (_isAutoMass)
                {
                    B2Bodies.b2Body_ApplyMassFromShapes(_bodyId);
                }
                else
                {
                    Mass = _userMassValue;
                }
            }
        }
        [SerializedField]
        public float Mass
        {
            get => _userMassValue;
            set
            {
                if (_isAutoMass || _bodyType != Body2DType.Dynamic)
                    return;

                _userMassValue = value;

                if (!IsValidBody())
                    return;

                var currentMassData = B2Bodies.b2Body_GetMassData(_bodyId);
                currentMassData.mass = _userMassValue;
                B2Bodies.b2Body_SetMassData(_bodyId, currentMassData);
            }
        }
        [SerializedField]
        public float GravityScale
        {
            get => _gravityScale;
            set
            {
                if (!IsValidBody())
                    return;

                _gravityScale = value;
                B2Bodies.b2Body_SetGravityScale(_bodyId, _gravityScale);
            }
        }
        [SerializedField]
        public float AngularDamping
        {
            get => _angularDamping;
            set
            {
                if (!IsValidBody())
                    return;

                _angularDamping = value;
                B2Bodies.b2Body_SetAngularDamping(_bodyId, _angularDamping);
            }
        }

        private vec2 _velocity;

        [ShowFieldNoSerialize(isReadOnly: true)]
        public vec2 Velocity
        {
            get
            {
                if (!IsValidBody())
                    return default;

                return B2Bodies.b2Body_GetLinearVelocity(_bodyId).ToVec2();
            }
            set
            {
                if (!IsValidBody())
                    return;

                B2Bodies.b2Body_SetLinearVelocity(_bodyId, value.ToB2Vec2());
            }
        }
        //public vec2 Velocity
        //{
        //    get => _velocity;
        //    set
        //    {
        //        _velocity = value;
        //        B2Bodies.b2Body_SetLinearVelocity(_bodyId, _velocity.ToB2Vec2());
        //    }
        //}

        [ShowFieldNoSerialize(isReadOnly: true)]
        public float AngularVelovity
        {
            get
            {
                if (IsValidBody())
                {
                    return B2Bodies.b2Body_GetAngularVelocity(_bodyId);
                }

                return 0;
            }
            set
            {
                if (IsValidBody())
                {
                    B2Bodies.b2Body_SetAngularVelocity(_bodyId, value);
                }
            }
        }

        internal vec3 PrevLocalPosition { get; set; }
        internal quat PrevLocalRotation { get; set; }

        internal void CalculatePhysicsInterpolation(Transform transform, vec3 prevPos, quat prevRot, float alpha)
        {
            // Parent world matrix (identity if no parent)
            mat4 parentMatrix = transform.Parent?.InterpolatedWorldMatrix ?? _identity;

            // Interpolated local transform
            vec3 localPos;
            quat localRot;

            if (Interpolate)
            {
                // Interpolate in *local space*
                localPos = Mathf.Lerp(prevPos, transform.LocalPosition, alpha);
                localRot = Mathf.Slerp(prevRot, transform.LocalRotation, alpha);
            }
            else
            {
                localPos = transform.LocalPosition;
                localRot = transform.LocalRotation;
            }

            vec3 localScale = transform.LocalScale;

            // Build local matrix from interpolated local values
            mat4 scaleMat = glm.scale(mat4.identity(), localScale);
            mat4 rotMat = Mathf.QuatToMat4(localRot);
            mat4 transMat = glm.translate(mat4.identity(), localPos);

            mat4 localMatrix = transMat * rotMat * scaleMat;

            // Compose world matrix from parent
            transform.InterpolatedWorldMatrix = parentMatrix * localMatrix;

            foreach (var child in transform.Children)
            {
                child.NeedsInterpolation = Interpolate;
                CalculatePhysicsInterpolation(child.Transform, child.LocalPosition, child.LocalRotation, alpha);
            }
        }

        protected override void OnAwake()
        {
            var worldPos = Transform.WorldPosition;
            var worldRot = Transform.WorldRotation;

            var bodyDef = B2Types.b2DefaultBodyDef();
            bodyDef.type = (B2BodyType)_bodyType;
            bodyDef.position = new B2Vec2(worldPos.x, worldPos.y);
            bodyDef.rotation = worldRot.QuatToB2Rot();
            bodyDef.name = GetID().ToString();
            bodyDef.isBullet = _isContinuos;
            bodyDef.isAwake = true;
            bodyDef.isEnabled = true;
            bodyDef.gravityScale = _gravityScale;
            bodyDef.enableSleep = _canSleep;

            _bodyId = B2Bodies.b2CreateBody(PhysicWorld.WorldID, ref bodyDef);

            var colliders = GetComponentsInChildren<Collider2D>();
            foreach (var collider in colliders)
            {
                var rigid = collider.GetComponent<RigidBody2D>();
                if (!rigid || rigid == this)
                {
                    collider.AttachedRigidbody = this;
                    collider.Create();
                }
            }

            Transform.OnChanged += OnTransformChanged;

            PhysicsLayer.RegisterRigidbody(this);
        }

        internal void PreUpdateBody()
        {
            if (_shouldUpdatePreTransformation)
            {
                if (!IsValidBody())
                    return;
                _shouldUpdatePreTransformation = false;
                B2Bodies.b2Body_SetTransform(_bodyId, Transform.WorldPosition.ToB2Vec2(), Transform.WorldRotation.QuatToB2Rot());
            }
        }

        internal void PostUpdateBody()
        {
            if (_bodyType == Body2DType.Dynamic)
            {
                if (!IsValidBody())
                    return;

                var position = B2Bodies.b2Body_GetPosition(_bodyId);
                _velocity = B2Bodies.b2Body_GetLinearVelocity(_bodyId).ToVec2();

                Transform.WorldPosition = new vec3(position.X, position.Y, Transform.WorldPosition.z);
                Transform.WorldRotation = B2Bodies.b2Body_GetRotation(_bodyId).B2RotToQuat();
            }
        }

        public void AddForce(vec2 force, ForceMode2D mode)
        {
            if (!IsValidBody())
                return;

            switch (mode)
            {
                case ForceMode2D.Force:
                    B2Bodies.b2Body_ApplyForceToCenter(_bodyId, force.ToB2Vec2(), true);
                    break;
                case ForceMode2D.Impulse:
                    B2Bodies.b2Body_ApplyLinearImpulseToCenter(_bodyId, force.ToB2Vec2(), true);
                    break;
                default:
                    Debug.Error($"ForceMode2D: '{mode}' Not implemented");
                    break;
            }
        }

        public void AddForce(vec2 force, vec2 point, ForceMode2D mode)
        {
            if (!IsValidBody())
                return;

            switch (mode)
            {
                case ForceMode2D.Force:
                    B2Bodies.b2Body_ApplyForce(_bodyId, point.ToB2Vec2(), force.ToB2Vec2(), true);
                    break;
                case ForceMode2D.Impulse:
                    B2Bodies.b2Body_ApplyLinearImpulse(_bodyId, point.ToB2Vec2(), force.ToB2Vec2(), true);
                    break;
                default:
                    Debug.Error($"ForceMode2D: '{mode}' Not implemented");
                    break;
            }
        }




        public void AddAngularForce(float force, ForceMode2D mode)
        {
            if (!IsValidBody())
                return;

            switch (mode)
            {
                case ForceMode2D.Force:
                    B2Bodies.b2Body_ApplyTorque(_bodyId, force, true);
                    break;
                case ForceMode2D.Impulse:
                    B2Bodies.b2Body_ApplyAngularImpulse(_bodyId, force, true);
                    break;
                default:
                    Debug.Error($"ForceMode2D: '{mode}' Not implemented");
                    break;
            }
        }

        internal void UpdateBody()
        {
            Mass = _userMassValue;
        }

        private void OnTransformChanged(Transform transform)
        {
            _shouldUpdatePreTransformation = true;
        }

        protected internal override void OnDestroy()
        {
            base.OnDestroy();

            if (Transform)
            {
                foreach (var child in Transform.Children)
                {
                    child.NeedsInterpolation = false;
                }

                Transform.NeedsInterpolation = false;
                Transform.OnChanged -= OnTransformChanged;
            }

            // Makes collider to use the default body. TODO: it shold link to the next available rigidbody in the hierarchy.
            if (Actor)
            {
                var colliders = GetComponentsInChildren<Collider2D>(); //TODO: This caused a crash when changin scenes, why?
                foreach (var collider in colliders)
                {
                    if (collider && collider.AttachedRigidbody == this)
                    {
                        collider.AttachedRigidbody = null;
                        collider.Create();
                    }
                }
            }

            if (IsValidBody())
            {
                B2Bodies.b2DestroyBody(_bodyId);
            }

            PhysicsLayer.UnregisterRigidbody(this);
        }

        private bool IsValidBody()
        {
            return B2Worlds.b2Body_IsValid(_bodyId);
        }
        public override void OnEnabled()
        {
            base.OnEnabled();

            if (!IsValidBody())
                return;
            if (IsEnabled)
            {
                B2Bodies.b2Body_Enable(_bodyId);
            }
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            if (!IsValidBody())
                return;

                B2Bodies.b2Body_Disable(_bodyId);
        }
    }
}