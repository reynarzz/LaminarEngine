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
using System.Runtime.CompilerServices;

namespace Engine.Utils
{
    /// <summary>
    /// Instead of modifying the box2d source code, I will copy here the functions I want to modify, so I can update box2d without losing custom changes.
    /// </summary>
    internal static class Box2dUtils
    {
        internal delegate float b2CastResultFcn<T>(B2ShapeId shapeId, B2Vec2 point, B2Vec2 normal, float fraction, ref T context) where T : struct;

        internal static B2TreeStats b2World_CastShape(B2WorldId worldId, ref B2ShapeProxy proxy, B2Vec2 translation,
                                                         B2QueryFilter filter, b2CastResultFcn<CastContext> fcn, ref CastContext context)
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

            var worldContext = new B2WorldRayCastContext_Custom<CastContext>(world, fcn, filter, 1.0f, context);

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
        internal static B2TreeStats b2World_CastRay(B2WorldId worldId, B2Vec2 origin, B2Vec2 translation,
                                                       B2QueryFilter filter, b2CastResultFcn<CastContext> fcn, ref CastContext context)
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

            var worldContext = new B2WorldRayCastContext_Custom<CastContext>(world, fcn, filter, 1.0f, context);

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

        private static float RayCastCallback(ref B2RayCastInput input, int proxyId, ulong userData,
                                                ref B2WorldRayCastContext_Custom<CastContext> context)
        {
            B2_UNUSED(proxyId);

            int shapeId = (int)userData;

            ref B2WorldRayCastContext_Custom<CastContext> worldContext = ref context;
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
        private static float ShapeCastCallback(ref B2ShapeCastInput input, int proxyId, ulong userData,
                                                  ref B2WorldRayCastContext_Custom<CastContext> context)
        {
            B2_UNUSED(proxyId);

            int shapeId = (int)userData;

            ref B2WorldRayCastContext_Custom<CastContext> worldContext = ref context;
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
        private struct B2WorldRayCastContext_Custom<T> where T : struct
        {
            internal B2World world;
            internal b2CastResultFcn<T> fcn;
            internal B2QueryFilter filter;
            internal float fraction;
            internal T userContext;

            internal B2WorldRayCastContext_Custom(B2World world, b2CastResultFcn<T> fcn, B2QueryFilter filter, float fraction, T userContext)
            {
                this.world = world;
                this.fcn = fcn;
                this.filter = filter;
                this.fraction = fraction;
                this.userContext = userContext;
            }
        }
    }


    internal struct CastContext
    {
        public CastHit2DArray Hits;
        public ulong LayerMask { get; }
        public bool TriggersPass { get; set; }
        public bool CastAll { get; set; }
        public CastContext(ulong layerMask)
        {
            LayerMask = layerMask;
        }
    }
    public struct CastHit2DArray
    {
        public const int Capacity = 50;
        public int Length { get; private set; }

        private CastHit2D _00, _01, _02, _03, _04, _05, _06, _07, _08, _09;
        private CastHit2D _10, _11, _12, _13, _14, _15, _16, _17, _18, _19;
        private CastHit2D _20, _21, _22, _23, _24, _25, _26, _27, _28, _29;
        private CastHit2D _30, _31, _32, _33, _34, _35, _36, _37, _38, _39;
        private CastHit2D _40, _41, _42, _43, _44, _45, _46, _47, _48, _49;
        public unsafe ref CastHit2D this[int index]
        {
            get
            {
                if ((uint)index > 49)
                {
                    throw new IndexOutOfRangeException();
                }

                fixed (CastHit2D* ptr = &_00)
                {
                    return ref ptr[index];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(CastHit2D item)
        {
            if (Length < Capacity)
            {
                this[Length] = item;
                Length++;
            }
        }
    }
}
