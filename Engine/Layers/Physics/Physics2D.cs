using Box2D.NET;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct CastHit2D
    {
        public vec2 Point { get; internal set; }
        public vec2 Normal { get; internal set; }
        public Collider2D Collider { get; internal set; }
        public bool isHit { get; internal set; }
    }

    public static partial class Physics2D
    {
        public static bool DrawColliders { get; set; }
        private static readonly B2QueryFilter _defaultQueryFilter = new B2QueryFilter(B2Constants.B2_DEFAULT_CATEGORY_BITS,
                                                                                      B2Constants.B2_DEFAULT_MASK_BITS);
        private class CastContext
        {
            public CastHit2D Hit;
            public ulong LayerMask { get; private set; }
            public bool IgnoreTriggers = true;
            public void Init(ulong layerMask)
            {
                Hit = default;
                LayerMask = layerMask;
                IgnoreTriggers = true;
            }
        }

        private readonly static CastContext _castContext = new();

        static Physics2D()
        {
        }

        public static CastHit2D Raycast(vec2 origin, vec2 direction, ulong layerMask)
        {
            _castContext.Init(layerMask);
            B2Worlds.b2World_CastRay(PhysicWorld.WorldID, origin.ToB2Vec2(), direction.ToB2Vec2(), _defaultQueryFilter, CastResultFunc, _castContext);
            return _castContext.Hit;
        }

        private static float CastResultFunc(B2ShapeId shapeId, B2Vec2 point, B2Vec2 normal, float fraction, object context)
        {
            var castContext = context as CastContext;

            var collider = B2Shapes.b2Shape_GetUserData(shapeId) as Collider2D;
            var isColliderInvalid = collider.IsTrigger && castContext.IgnoreTriggers;

            if (isColliderInvalid || !LayerMask.AreValid(collider.Actor.Layer, castContext.LayerMask))
            {
                return -1;
            }

            castContext.Hit.Collider = collider;
            castContext.Hit.Point = point.ToVec2();
            castContext.Hit.Normal = normal.ToVec2();
            castContext.Hit.isHit = true;

            return fraction; // Stop at the first hit
        }
        public static CastHit2D Raycast(vec2 origin, vec2 direction)
        {
            return Raycast(origin, direction, ulong.MaxValue);
        }
    }
}