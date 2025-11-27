using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2D.NET;
using Engine.Graphics;
using GlmNet;

namespace Engine
{
    internal static class PhysicWorld
    {
        public static B2WorldId WorldID { get; }
        public static B2World World { get; }
        private static B2DebugDraw _debugDraw;
        internal static B2DebugDraw DebugDraw => _debugDraw;
        static PhysicWorld()
        {
            B2WorldDef worldDef = B2Types.b2DefaultWorldDef();
            worldDef.gravity = new B2Vec2(0, -9.8f);
            worldDef.enableContinuous = true;

            WorldID = B2Worlds.b2CreateWorld(ref worldDef);
            World = B2Worlds.b2GetWorld(0);

            _debugDraw = new B2DebugDraw()
            {
                context = null,
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

        }

        internal static void SetGravity(vec2 gravity)
        {
            B2Worlds.b2World_SetGravity(WorldID, new B2Vec2(gravity.x, gravity.y));
        }
    }
}