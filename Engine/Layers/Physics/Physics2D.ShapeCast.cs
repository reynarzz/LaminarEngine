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
        public static CastHit2D BoxCast(vec2 origin, vec2 size, float rotation, ulong layerMask)
        {
            size *= 0.5f;
            var box = b2MakeOffsetBox(size.x, size.y, origin.ToB2Vec2(), new B2Rot(MathF.Cos(glm.radians(rotation)), MathF.Sin(glm.radians(rotation))));
            B2ShapeProxy proxy = default;
            proxy.count = box.count;
            proxy.radius = 0;
            proxy.points[0] = box.vertices[0];
            proxy.points[1] = box.vertices[1];
            proxy.points[2] = box.vertices[2];
            proxy.points[3] = box.vertices[3];

            var context = new CastContext(layerMask);
            Box2dUtils.b2World_CastShape(PhysicWorld.WorldID, ref proxy, default, _defaultQueryFilter, CastResultFunc, ref context);
            return context.Hits[0];
        }
        public static CastHit2D BoxCast(vec2 origin, vec2 size, ulong layerMask)
        {
            size *= 0.5f;
            B2ShapeProxy proxy = default;
            proxy.count = 4;
            proxy.radius = 0;
            proxy.points[0] = new B2Vec2(-size.x, -size.y);
            proxy.points[1] = new B2Vec2(size.x, -size.y);
            proxy.points[2] = new B2Vec2(size.x, size.y);
            proxy.points[3] = new B2Vec2(-size.x, size.y);
            var context = new CastContext(layerMask);
            Box2dUtils.b2World_CastShape(PhysicWorld.WorldID, ref proxy, default, _defaultQueryFilter, CastResultFunc, ref context);
            return context.Hits[0];
        }

        public static CastHit2D CircleCast(vec2 origin, float radius, ulong layerMask)
        {
            B2ShapeProxy proxy = default;
            proxy.count = 1;
            proxy.points[0] = origin.ToB2Vec2();
            proxy.radius = radius;
            var context = new CastContext(layerMask);
            Box2dUtils.b2World_CastShape(PhysicWorld.WorldID, ref proxy, default, _defaultQueryFilter, CastResultFunc, ref context);
            return context.Hits[0];
        }


        public static CastHit2D BoxCast(vec2 origin, vec2 size)
        {
            return BoxCast(origin, size, ulong.MaxValue);
        }

        public static CastHit2D CircleCast(vec2 origin, float radius)
        {
            return CircleCast(origin, radius, ulong.MaxValue);
        }
    }
}