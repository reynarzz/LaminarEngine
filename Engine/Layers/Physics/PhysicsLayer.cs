using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Box2D.NET;
using Engine.Graphics;
using Engine.Utils;

namespace Engine.Layers
{
    internal class PhysicsLayer : LayerBase
    {
        // Samples: https://github.com/ikpil/Box2D.NET/tree/e68c8ff1fb9da8bd87a71159b13010c25eed76f8/src/Box2D.NET.Samples/Samples

        private B2DebugDraw _debugDraw;
        private ContactsDispatcher _contactDispatcher;
        private B2BodyId _bodyTest;
        private B2BodyId _floorTest;
        private float accumulator = 0f;
        private const float fixedTimeStep = 0.02f;
        private List<Collider2D> _colliders = new();
        private List<RigidBody2D> _rigidbody = new();

        public override void Initialize()
        {
            _contactDispatcher = new ContactsDispatcher();
            _debugDraw = new B2DebugDraw()
            {
                context = this,

                DrawPolygonFcn = Box2DDraw.DrawPolygon,
                DrawSolidPolygonFcn = Box2DDraw.DrawSolidPolygon,
                DrawCircleFcn = Box2DDraw.DrawCircle,
                DrawSolidCircleFcn = Box2DDraw.DrawSolidCircle,
                DrawSolidCapsuleFcn = Box2DDraw.DrawSolidCapsule,
                DrawSegmentFcn = Box2DDraw.DrawSegment,
                DrawTransformFcn = Box2DDraw.DrawTransform,
                DrawPointFcn = Box2DDraw.DrawPoint,
                DrawStringFcn = Box2DDraw.DrawString,

                drawShapes = true,
                drawJoints = true,
                drawJointExtras = false,
                drawBounds = false,
                drawMass = false,
                drawBodyNames = false,
                drawContacts = true,
                drawGraphColors = false,
                drawContactNormals = true,
                drawContactImpulses = false,
                drawContactFeatures = false,
                drawFrictionImpulses = false,
                drawIslands = false,
            };

            Debug.Log("World is valid: " + B2Worlds.b2World_IsValid(PhysicWorld.WorldID));
            B2Worlds.b2World_SetCustomFilterCallback(PhysicWorld.WorldID, CustomFilter, this);

            B2QueryFilter castFilter = default;
            castFilter.categoryBits = 0;
            castFilter.maskBits = 0xFF;

            //B2ShapeProxy shapeCast = default;
            //shapeCast.radius = 1;
            //B2Worlds.b2World_CastShape(PhysicWorld.WorldID, ref shapeCast, new B2Vec2(1, 0), castFilter, CastResultFunc, null);

            // B2Geometries.b2RayCastPolygon();
        }

        public bool CustomFilter(B2ShapeId shapeIdA, B2ShapeId shapeIdB, object context)
        {
            var colA = B2Shapes.b2Shape_GetUserData(shapeIdA) as Collider2D;
            var colB = B2Shapes.b2Shape_GetUserData(shapeIdB) as Collider2D;

            return LayerMask.AreEnabled(colA.Actor.Layer, colB.Actor.Layer);
        }

        internal override void UpdateLayer()
        {
            accumulator = Math.Min(accumulator + Time.DeltaTime, 0.25f);

            // TODO: refactor, this is for fast prototyping
            _rigidbody.Clear();
            SceneManager.ActiveScene.FindAll(_rigidbody, findDisabled: false);

            foreach (var rigidbody in _rigidbody)
            {
                rigidbody.PreUpdateBody();
            }

            while (accumulator >= fixedTimeStep)
            {
                _contactDispatcher.Update();

                SceneManager.ActiveScene.FixedUpdate();

                foreach (var rigidbody in _rigidbody)
                {
                    rigidbody.PreUpdateBody();
                    rigidbody.PrevLocalPosition = rigidbody.Transform.LocalPosition;
                    rigidbody.PrevLocalRotation = rigidbody.Transform.LocalRotation;
                }

                B2Worlds.b2World_Step(PhysicWorld.WorldID, fixedTimeStep, 4);
                accumulator -= fixedTimeStep;

                foreach (var rigidbody in _rigidbody)
                {
                    if (rigidbody)
                    {
                        rigidbody.PostUpdateBody();
                    }
                }
            }

            float alpha = accumulator / fixedTimeStep;

            foreach (var rigidbody in _rigidbody)
            {
                rigidbody.CalculatePhysicsInterpolation(rigidbody.Transform, rigidbody.PrevLocalPosition, rigidbody.PrevLocalRotation, alpha);

                if (Physics2D.DrawColliders)
                {
                    DrawShapes(rigidbody);
                }
            }
        }

        private void DrawShapes(RigidBody2D rigidbody)
        {
            // Example, remove from here
            var transform = new B2Transform();
            transform.p = new B2Vec2(rigidbody.Transform.WorldPosition.x, rigidbody.Transform.WorldPosition.y);
            transform.q = rigidbody.Transform.WorldRotation.QuatToB2Rot();

            _colliders.Clear();
            rigidbody.GetComponentsInChildren(ref _colliders);

            for (int i = 0; i < _colliders.Count; i++)
            {
                if (_colliders[i] && _colliders[i].IsEnabled && _colliders[i].ShapesId != null)
                {
                    for (int j = 0; j < _colliders[i].ShapesId.Length; j++)
                    {
                        var collider = _colliders[i];
                        var color = B2HexColor.b2_colorCyan;
                        if (collider.IsTrigger)
                        {
                            color = B2HexColor.b2_colorYellow;
                        }
                        B2Worlds.b2DrawShape(_debugDraw, B2Shapes.b2GetShape(B2Worlds.b2GetWorld(0), _colliders[i].ShapesId[j]), transform, color);
                    }
                }
            }
        }

        public override void Close()
        {
        }


    }
}
