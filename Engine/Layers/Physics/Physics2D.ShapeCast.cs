using Box2D.NET;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Box2D.NET.B2MathFunction;
using static Box2D.NET.B2Distances;
using static Box2D.NET.B2Hulls;
using static Box2D.NET.B2Geometries;
using static Box2D.NET.B2Diagnostics;

namespace Engine
{
    public static partial class Physics2D
    {
       
        public static CastHit2D CircleCast(vec2 origin, float radius, ulong layerMask)
        {
            return CircleCast(origin, radius, layerMask, false)[0];
        }

        public static CastHit2DArray CircleCastAll(vec2 origin, float radius, ulong layerMask)
        {
            return CircleCast(origin, radius, layerMask, true);
        }

        public static CastHit2D CircleCast(vec2 origin, float radius)
        {
            return CircleCast(origin, radius, ulong.MaxValue);
        }

        public static CastHit2D BoxCast(vec2 origin, vec2 size)
        {
            return BoxCast(origin, size, ulong.MaxValue);
        }
        
        public static CastHit2D BoxCast(vec2 origin, vec2 size, ulong layerMask)
        {
            return BoxCast(origin, size, layerMask, false)[0];
        }
        public static CastHit2DArray BoxCastAll(vec2 origin, vec2 size, ulong layerMask)
        {
            return BoxCast(origin, size, layerMask, true);
        }
        public static CastHit2DArray BoxCastAll(vec2 origin, vec2 size)
        {
            return BoxCast(origin, size, ulong.MaxValue, true);
        }
        public static CastHit2D BoxCast(vec2 origin, vec2 size, float rotation, ulong layerMask)
        {
            size *= 0.5f;
            var box = b2MakeOffsetBox(size.x, size.y, origin.ToB2Vec2(), new B2Rot(MathF.Cos(glm.radians(rotation)), MathF.Sin(glm.radians(rotation))));
            return BoxCast(ref box.vertices[0], ref box.vertices[1], ref box.vertices[2], ref box.vertices[3], layerMask, false)[0];
        }

        private static CastHit2DArray BoxCast(vec2 origin, vec2 size, ulong layerMask, bool castAll)
        {
            size *= 0.5f;
            var v0 = new B2Vec2(origin.x - size.x, origin.y - size.y);
            var v1 = new B2Vec2(origin.x + size.x, origin.y - size.y);
            var v2 = new B2Vec2(origin.x + size.x, origin.y + size.y);
            var v3 = new B2Vec2(origin.x - size.x, origin.y + size.y);

            return BoxCast(ref v0, ref v1, ref v2, ref v3, layerMask, castAll);
        }

        private static CastHit2DArray BoxCast(ref B2Vec2 v0, ref B2Vec2 v1, ref B2Vec2 v2, ref B2Vec2 v3, ulong layerMask, bool castAll)
        {
            B2ShapeProxy proxy = default;
            proxy.count = 4;
            proxy.radius = 0;
            proxy.points[0] = v0;
            proxy.points[1] = v1;
            proxy.points[2] = v2;
            proxy.points[3] = v3;

            var context = new CastContext(layerMask);
            context.CastAll = castAll;
            Box2dUtils.b2World_CastShape(PhysicWorld.WorldID, ref proxy, default, _defaultQueryFilter, CastResultFunc, ref context);
            return context.Hits;
        }

        private static CastHit2DArray CircleCast(vec2 origin, float radius, ulong layerMask, bool castAll)
        {
            B2ShapeProxy proxy = default;
            proxy.count = 1;
            proxy.points[0] = origin.ToB2Vec2();
            proxy.radius = radius;
            var context = new CastContext(layerMask);
            context.CastAll = castAll;
            Box2dUtils.b2World_CastShape(PhysicWorld.WorldID, ref proxy, default, _defaultQueryFilter, CastResultFunc, ref context);
            return context.Hits;
        }

    }
}