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

        private static ContactsDispatcher _contactDispatcher = new();
        internal static ContactsDispatcher ContactsDispatcher => _contactDispatcher;
        private static List<RigidBody2D> _rigidbodies = new();

        private static float _accumulator = 0f;
        private const float _fixedTimeStep = 0.02f;

        public override Task InitializeAsync()
        {
            PhysicWorld.Initialize();
            _contactDispatcher = new ContactsDispatcher();
            B2Worlds.b2World_SetCustomFilterCallback(PhysicWorld.WorldID, CustomFilter, this);
            _accumulator = 0;
            _rigidbodies.Clear();

            return Task.CompletedTask;
        }

        public bool CustomFilter(B2ShapeId shapeIdA, B2ShapeId shapeIdB, object context)
        {
            var colA = B2Shapes.b2Shape_GetUserData(shapeIdA) as Collider2D;
            var colB = B2Shapes.b2Shape_GetUserData(shapeIdB) as Collider2D;

            return LayerMask.AreEnabled(colA.Actor.Layer, colB.Actor.Layer);
        }

        internal override void UpdateLayer()
        {
            _accumulator = Math.Min(_accumulator + Time.DeltaTime, 0.25f);

            _rigidbodies.Clear();
            SceneManager.FindAll(_rigidbodies, findDisabled: false);

            while (_accumulator >= _fixedTimeStep)
            {
                _contactDispatcher.Update();
                SceneManager.FixedUpdate();

                foreach (var rigidbody in _rigidbodies)
                {
                    if (!rigidbody)
                        continue;

                    rigidbody.PrevLocalPosition = rigidbody.Transform.LocalPosition;
                    rigidbody.PrevLocalRotation = rigidbody.Transform.LocalRotation;

                    rigidbody.PreUpdateBody();
                }

                B2Worlds.b2World_Step(PhysicWorld.WorldID, _fixedTimeStep, 3);
                _accumulator -= _fixedTimeStep;

                foreach (var rigidbody in _rigidbodies)
                {
                    if (!rigidbody)
                        continue;

                    rigidbody.PostUpdateBody();
                }
            }

            float alpha = _accumulator / _fixedTimeStep;

            foreach (var rigidbody in _rigidbodies)
            {
                if (!rigidbody)
                    continue;

                rigidbody.CalculatePhysicsInterpolation(rigidbody.Transform, rigidbody.PrevLocalPosition, rigidbody.PrevLocalRotation, alpha);
            }
        }

        internal static void Clear()
        {
            _accumulator = 0;
            _rigidbodies.Clear();
            _contactDispatcher.ClearCollisions();
        }
        public override void Close()
        {
            Clear();
            PhysicWorld.Clear();
        }
    }
}
