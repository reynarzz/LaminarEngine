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
        public float Fraction { get; set; }
        public Collider2D Collider { get; internal set; }
        public bool isHit { get; internal set; }
    }



    public static partial class Physics2D
    {
        public static bool DrawColliders { get; set; }
        private static readonly B2QueryFilter _defaultQueryFilter = new B2QueryFilter(B2Constants.B2_DEFAULT_CATEGORY_BITS,
                                                                                      B2Constants.B2_DEFAULT_MASK_BITS);


        public static CastHit2D Raycast(vec2 origin, vec2 direction)
        {
            return Raycast(origin, direction, ulong.MaxValue);
        }
        public static CastHit2DArray RaycastAll(vec2 origin, vec2 direction)
        {
            return RaycastAll(origin, direction, ulong.MaxValue);
        }
        public static CastHit2D Raycast(vec2 origin, vec2 direction, ulong layerMask)
        {
            var context = new CastContext(layerMask);
            Box2dUtils.b2World_CastRay(PhysicWorld.WorldID, origin.ToB2Vec2(), direction.ToB2Vec2(), _defaultQueryFilter, CastResultFunc, ref context);
            return context.Hits[0];
        }
        public static CastHit2DArray RaycastAll(vec2 origin, vec2 direction, ulong layerMask)
        {
            var context = new CastContext(layerMask);
            context.CastAll = true;
            Box2dUtils.b2World_CastRay(PhysicWorld.WorldID, origin.ToB2Vec2(), direction.ToB2Vec2(), _defaultQueryFilter, CastResultFunc, ref context);
            return context.Hits;
        }

        private static float CastResultFunc(B2ShapeId shapeId, B2Vec2 point, B2Vec2 normal, float fraction, ref CastContext context)
        {
            var collider = B2Shapes.b2Shape_GetUserData(shapeId) as Collider2D;
            var isColliderInvalid = collider.IsTrigger && !context.TriggersPass;

            if (isColliderInvalid || !LayerMask.AreValid(collider.Actor.Layer, context.LayerMask) || context.Hits.Length == CastHit2DArray.Capacity)
            {
                return -1;
            }

            context.Hits.Add(new CastHit2D()
            {
                Collider = collider,
                Point = point.ToVec2(),
                Normal = normal.ToVec2(),
                isHit = true,
                Fraction = fraction,
            });

            if (context.CastAll)
            {
                return -1;
            }

            return fraction;
        }
    }
}