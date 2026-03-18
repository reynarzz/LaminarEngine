using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Box2D.NET;
using Engine.Graphics;
using GlmNet;

namespace Engine
{
    internal static class PhysicWorld
    {
        public static B2WorldId WorldID { get; private set; }
        public static B2World World { get; private set; }
        private static B2DebugDraw _debugDraw;
        internal static B2DebugDraw DebugDraw => _debugDraw;
        private static b2EnqueueTaskCallback _enqueueTask;
        private static b2FinishTaskCallback _finishTask;

        private static int _workerCount; // store what we told Box2D at init

        internal static void Initialize(vec3 initialGravity)
        {
            B2WorldDef worldDef = B2Types.b2DefaultWorldDef();
            worldDef.gravity = new B2Vec2(0, -9.8f);
            worldDef.enableContinuous = true;

            _workerCount = Math.Clamp(Environment.ProcessorCount - 1, 1, 8);
            _enqueueTask = EnqueueTask;
            _finishTask = FinishTask;
            //worldDef.workerCount = _workerCount;
            //worldDef.enqueueTask = _enqueueTask;
            worldDef.finishTask = _finishTask;
            worldDef.userTaskContext = null;

            WorldID = B2Worlds.b2CreateWorld(ref worldDef);
            World = B2Worlds.b2GetWorld(0);
            Console.WriteLine($"[Physics] Running with {worldDef.workerCount} workers");

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

        private static object EnqueueTask(b2TaskCallback task, int itemCount, int minRange,
                                    object taskContext, object userContext)
        {
            if (itemCount <= minRange || itemCount < 4)
            {
                task(0, itemCount, 0, taskContext);
                return null;
            }

            // Never spawn more workers than we told Box2D about
            int workerCount = Math.Clamp(itemCount / Math.Max(minRange, 1), 1, _workerCount);
            var countdown = new CountdownEvent(workerCount);
            int chunkSize = itemCount / workerCount;

            for (int w = 0; w < workerCount; w++)
            {
                int start = w * chunkSize;
                int end = (w == workerCount - 1) ? itemCount : start + chunkSize;
                uint workerIdx = (uint)w; // now safe — capped to _workerCount

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try { task(start, end, workerIdx, taskContext); }
                    finally { countdown.Signal(); }
                });
            }

            return countdown;
        }

        private static void FinishTask(object userTask, object userContext)
        {
            if (userTask == null) return; // was executed inline, nothing to wait for

            var countdown = (CountdownEvent)userTask;
            countdown.Wait();
            countdown.Dispose();
        }


        internal static void SetGravity(vec2 gravity)
        {
            B2Worlds.b2World_SetGravity(WorldID, new B2Vec2(gravity.x, gravity.y));
        }

        internal static void Clear()
        {
            B2Worlds.b2DestroyWorld(WorldID);
        }
    }
}