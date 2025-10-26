using Box2D.NET;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Box2D.NET.B2Tables;
using static Box2D.NET.B2Arrays;
using static Box2D.NET.B2DynamicTrees;
using static Box2D.NET.B2Cores;
using static Box2D.NET.B2Diagnostics;
using static Box2D.NET.B2Buffers;
using static Box2D.NET.B2Profiling;
using static Box2D.NET.B2Constants;
using static Box2D.NET.B2Contacts;
using static Box2D.NET.B2MathFunction;
using static Box2D.NET.B2Shapes;
using static Box2D.NET.B2Solvers;
using static Box2D.NET.B2Bodies;
using static Box2D.NET.B2Joints;
using static Box2D.NET.B2IdPools;
using static Box2D.NET.B2ArenaAllocators;
using static Box2D.NET.B2BoardPhases;
using static Box2D.NET.B2Distances;
using static Box2D.NET.B2ConstraintGraphs;
using static Box2D.NET.B2BitSets;
using static Box2D.NET.B2SolverSets;
using static Box2D.NET.B2AABBs;
using static Box2D.NET.B2CTZs;
using static Box2D.NET.B2Islands;
using static Box2D.NET.B2Timers;
using static Box2D.NET.B2Sensors;
using static Box2D.NET.B2Worlds;

namespace Engine.Utils
{
    /// <summary>
    /// Instead of modifying the box2d source code, I will copy here the functions I want to modify, so I can update box2d without losing custom changes.
    /// </summary>
    internal static class Box2dUtils
    {
        private static B2Transform _boxTransform = B2MathFunction.b2Transform_identity;
        internal delegate float b2CastResultFcn<T>(B2ShapeId shapeId, B2Vec2 point, B2Vec2 normal, float fraction, ref T context) where T: struct, ICastContext;

        internal interface ICastContext
        {
            CastHit2D Hit { get; set; }
            ulong LayerMask { get; }
            bool TriggersPass { get; }
        }

        internal static void UpdateBox(ref B2Polygon shape, float halfWidth, float halfHeight, vec2 center)
        {
            _boxTransform.p = center.ToB2Vec2();

            shape.count = 4;
            shape.vertices[0] = B2MathFunction.b2TransformPoint(ref _boxTransform, new B2Vec2(-halfWidth, -halfHeight));
            shape.vertices[1] = B2MathFunction.b2TransformPoint(ref _boxTransform, new B2Vec2(halfWidth, -halfHeight));
            shape.vertices[2] = B2MathFunction.b2TransformPoint(ref _boxTransform, new B2Vec2(halfWidth, halfHeight));
            shape.vertices[3] = B2MathFunction.b2TransformPoint(ref _boxTransform, new B2Vec2(-halfWidth, halfHeight));

            shape.radius = 0.0f;
            shape.centroid = _boxTransform.p;
        }

        internal static void ApplyBoxNormals(ref B2Polygon shape)
        {
            shape.normals[0] = B2MathFunction.b2RotateVector(B2MathFunction.b2Rot_identity, new B2Vec2(0.0f, -1.0f));
            shape.normals[1] = B2MathFunction.b2RotateVector(B2MathFunction.b2Rot_identity, new B2Vec2(1.0f, 0.0f));
            shape.normals[2] = B2MathFunction.b2RotateVector(B2MathFunction.b2Rot_identity, new B2Vec2(0.0f, 1.0f));
            shape.normals[3] = B2MathFunction.b2RotateVector(B2MathFunction.b2Rot_identity, new B2Vec2(-1.0f, 0.0f));
        }

        internal struct B2WorldRayCastContext<T> where T : struct, ICastContext
        {
            public B2World world;
            public b2CastResultFcn<T> fcn;
            public B2QueryFilter filter;
            public float fraction;
            public T userContext;

            public B2WorldRayCastContext(B2World world, b2CastResultFcn<T> fcn, B2QueryFilter filter, float fraction, T userContext)
            {
                this.world = world;
                this.fcn = fcn;
                this.filter = filter;
                this.fraction = fraction;
                this.userContext = userContext;
            }
        }

        internal static B2TreeStats b2World_CastShape<T>(B2WorldId worldId, ref B2ShapeProxy proxy, B2Vec2 translation,
                                                         B2QueryFilter filter, b2CastResultFcn<T> fcn, ref T context) where T : struct, ICastContext
        {
            B2TreeStats treeStats = new B2TreeStats();

            B2World world = b2GetWorldFromId(worldId);
            B2_ASSERT(world.locked == false);
            if (world.locked)
            {
                return treeStats;
            }

            B2_ASSERT(b2IsValidVec2(translation));

            B2ShapeCastInput input = new B2ShapeCastInput();
            input.proxy = proxy;
            input.translation = translation;
            input.maxFraction = 1.0f;

            var worldContext = new B2WorldRayCastContext<T>(world, fcn, filter, 1.0f, context);

            for (int i = 0; i < (int)B2BodyType.b2_bodyTypeCount; ++i)
            {
                B2TreeStats treeResult =
                    b2DynamicTree_ShapeCast(world.broadPhase.trees[i], ref input, filter.maskBits, ShapeCastCallback, ref worldContext);
                treeStats.nodeVisits += treeResult.nodeVisits;
                treeStats.leafVisits += treeResult.leafVisits;

                if (worldContext.fraction == 0.0f)
                {
                    context = worldContext.userContext;
                    return treeStats;
                }

                input.maxFraction = worldContext.fraction;
            }
            context = worldContext.userContext;

            return treeStats;
        }
        internal static B2TreeStats b2World_CastRay<T>(B2WorldId worldId, B2Vec2 origin, B2Vec2 translation,
                                                       B2QueryFilter filter, b2CastResultFcn<T> fcn, ref T context) where T : struct, ICastContext
        {
            B2TreeStats treeStats = new B2TreeStats();

            B2World world = b2GetWorldFromId(worldId);
            B2_ASSERT(world.locked == false);
            if (world.locked)
            {
                return treeStats;
            }

            B2_ASSERT(b2IsValidVec2(origin));
            B2_ASSERT(b2IsValidVec2(translation));

            B2RayCastInput input = new B2RayCastInput(origin, translation, 1.0f);

            var worldContext = new B2WorldRayCastContext<T>(world, fcn, filter, 1.0f, context);

            for (int i = 0; i < (int)B2BodyType.b2_bodyTypeCount; ++i)
            {
                B2TreeStats treeResult =
                    b2DynamicTree_RayCast(world.broadPhase.trees[i], ref input, filter.maskBits, RayCastCallback, ref worldContext);
                treeStats.nodeVisits += treeResult.nodeVisits;
                treeStats.leafVisits += treeResult.leafVisits;

                if (worldContext.fraction == 0.0f)
                {
                    context = worldContext.userContext;
                    return treeStats;
                }

                input.maxFraction = worldContext.fraction;
            }

            context = worldContext.userContext;
            return treeStats;
        }

        private static float RayCastCallback<T>(ref B2RayCastInput input, int proxyId, ulong userData,
                                                ref B2WorldRayCastContext<T> context) where T : struct, ICastContext
        {
            B2_UNUSED(proxyId);

            int shapeId = (int)userData;

            ref B2WorldRayCastContext<T> worldContext = ref context;
            B2World world = worldContext.world;

            B2Shape shape = b2Array_Get(ref world.shapes, shapeId);

            if (b2ShouldQueryCollide(shape.filter, worldContext.filter) == false)
            {
                return input.maxFraction;
            }

            B2Body body = b2Array_Get(ref world.bodies, shape.bodyId);
            B2Transform transform = b2GetBodyTransformQuick(world, body);
            B2CastOutput output = b2RayCastShape(ref input, shape, transform);

            if (output.hit)
            {
                B2ShapeId id = new B2ShapeId(shapeId + 1, world.worldId, shape.generation);
                float fraction = worldContext.fcn(id, output.point, output.normal, output.fraction, ref worldContext.userContext);

                // The user may return -1 to skip this shape
                if (0.0f <= fraction && fraction <= 1.0f)
                {
                    worldContext.fraction = fraction;
                }

                return fraction;
            }

            return input.maxFraction;
        }
        private static float ShapeCastCallback<T>(ref B2ShapeCastInput input, int proxyId, ulong userData,
                                                  ref B2WorldRayCastContext<T> context) where T : struct, ICastContext
        {
            B2_UNUSED(proxyId);

            int shapeId = (int)userData;

            ref B2WorldRayCastContext<T> worldContext = ref context;
            B2World world = worldContext.world;

            B2Shape shape = b2Array_Get(ref world.shapes, shapeId);

            if (b2ShouldQueryCollide(shape.filter, worldContext.filter) == false)
            {
                return input.maxFraction;
            }

            B2Body body = b2Array_Get(ref world.bodies, shape.bodyId);
            B2Transform transform = b2GetBodyTransformQuick(world, body);

            B2CastOutput output = b2ShapeCastShape(ref input, shape, transform);

            if (output.hit)
            {
                B2ShapeId id = new B2ShapeId(shapeId + 1, world.worldId, shape.generation);
                float fraction = worldContext.fcn(id, output.point, output.normal, output.fraction, ref worldContext.userContext);

                // The user may return -1 to skip this shape
                if (0.0f <= fraction && fraction <= 1.0f)
                {
                    worldContext.fraction = fraction;
                }

                return fraction;
            }

            return input.maxFraction;
        }
    }
}
