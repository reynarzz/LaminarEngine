using System;
using System.Collections.Concurrent;
using System.Threading;
using Box2D.NET;
using Engine.Graphics;
using GlmNet;

namespace Engine
{
    internal static class PhysicWorld
    {
        public static B2WorldId WorldID { get; private set; }
        public static B2DebugDraw DebugDraw => _debugDraw;
        public static B2World World { get; private set; }

        private static B2DebugDraw _debugDraw;

        private static b2EnqueueTaskCallback _enqueueTask;
        private static b2FinishTaskCallback _finishTask;

        private static Thread[] _workers;
        private static WorkItem[] _pendingItems;
        private static AutoResetEvent[] _workReady;
        private static volatile bool _shutdown;
        private static int _workerCount;

        private static volatile Exception _workerException;

        private static readonly ConcurrentQueue<TaskBatch> _batchPool = new();

        internal static void Initialize(vec3 initialGravity)
        {
            _shutdown = false;
            _workerCount = Math.Clamp(Environment.ProcessorCount - 1, 1, 8);

            InitWorkerThreads();

            _enqueueTask = EnqueueTask;
            _finishTask = FinishTask;

            B2WorldDef worldDef = B2Types.b2DefaultWorldDef();
            worldDef.gravity = new B2Vec2(initialGravity.x, initialGravity.y);
            worldDef.enableContinuous = true;
            worldDef.workerCount = _workerCount;
            worldDef.enqueueTask = _enqueueTask;
            worldDef.finishTask = _finishTask;
            worldDef.userTaskContext = null;

            WorldID = B2Worlds.b2CreateWorld(ref worldDef);
            World = B2Worlds.b2GetWorld(WorldID.index1 - 1);

            Debug.Log($"[Physics] Initialized with {_workerCount} dedicated workers.");

            InitDebugDraw();
        }

        private static void InitDebugDraw()
        {
            _debugDraw = new B2DebugDraw
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
        private static TaskBatch RentBatch(int count)
        {
            if (!_batchPool.TryDequeue(out var batch))
            {
                batch = new TaskBatch();
            }

            batch.Reset(count);
            return batch;
        }

        private static void ReturnBatch(TaskBatch batch)
        {
            _batchPool.Enqueue(batch);
        }

        private static object EnqueueTask(b2TaskCallback task, int itemCount, int minRange, object taskContext, object userContext)
        {
            if (itemCount <= minRange)
            {
                task(0, itemCount, 0, taskContext);
                return null;
            }

            var range = Math.Max(minRange, 1);
            var needed = itemCount / range;
            var workerCount = Math.Clamp(needed, 2, _workerCount);
            var chunkSize = itemCount / workerCount;

            _workerException = null;

            TaskBatch batch = RentBatch(workerCount);

            for (int i = 0; i < workerCount; i++)
            {
                int start = i * chunkSize;
                int end = (i == workerCount - 1) ? itemCount : start + chunkSize;

                _pendingItems[i] = new WorkItem(task, start, end, (uint)i, taskContext, batch);
                _workReady[i].Set();
            }

            return batch;
        }

        private static void FinishTask(object userTask, object userContext)
        {
            if (userTask is not TaskBatch batch)
                return;

            batch.Wait();

            var workerEx = _workerException;

            ReturnBatch(batch);

            if (workerEx != null)
            {
                throw new AggregateException("[Physics] One or more worker threads faulted.", workerEx);
            }
        }

        private static void InitWorkerThreads()
        {
            _workers = new Thread[_workerCount];
            _pendingItems = new WorkItem[_workerCount];
            _workReady = new AutoResetEvent[_workerCount];

            for (int i = 0; i < _workerCount; i++)
            {
                _workReady[i] = new AutoResetEvent(false);

                _workers[i] = new Thread(WorkerLoop)
                {
                    Name = $"Box2D-Worker-{i}",
                    IsBackground = true,
                    Priority = ThreadPriority.AboveNormal
                };

                _workers[i].Start(i);
            }
        }

        private static void WorkerLoop(object state)
        {
            int workerIndex = (int)state;
            var ready = _workReady[workerIndex];

            while (true)
            {
                ready.WaitOne();

                if (_shutdown)
                    return;

                var item = _pendingItems[workerIndex];

                try
                {
                    item.Task(item.Start, item.End, item.WorkerIndex, item.TaskContext);
                }
                catch (Exception ex)
                {
                    Debug.Error($"[Physics Worker {workerIndex}] Exception: {ex}");
                    Interlocked.CompareExchange(ref _workerException, ex, null);
                }
                finally
                {
                    item.Batch.Signal();
                }
            }
        }

        internal static void SetGravity(vec2 gravity)
        {
            B2Worlds.b2World_SetGravity(WorldID, new B2Vec2(gravity.x, gravity.y));
        }

        internal static void Clear()
        {
            _shutdown = true;

            // Wake all workers exactly once; AutoResetEvent guarantees no lost signals
            for (int i = 0; i < _workReady.Length; i++)
            {
                _workReady[i].Set();
            }

            for (int i = 0; i < _workers.Length; i++)
            {
                if (!_workers[i].Join(1000))
                {
                    Debug.Error($"[Physics] Worker {i} did not exit cleanly within timeout.");
                }
            }

            B2Worlds.b2DestroyWorld(WorldID);
        }

        private struct WorkItem
        {
            public b2TaskCallback Task;
            public int Start;
            public int End;
            public uint WorkerIndex;
            public object TaskContext;
            public TaskBatch Batch;

            public WorkItem(b2TaskCallback task, int start, int end, uint workerIndex, object taskContext, TaskBatch batch)
            {
                Task = task;
                Start = start;
                End = end;
                WorkerIndex = workerIndex;
                TaskContext = taskContext;
                Batch = batch;
            }
        }

        private sealed class TaskBatch
        {
            private int _remaining;
            private readonly ManualResetEventSlim _done = new ManualResetEventSlim(false);

            public void Reset(int count)
            {
                Volatile.Write(ref _remaining, count);
                _done.Reset();
            }

            public void Signal()
            {
                if (Interlocked.Decrement(ref _remaining) == 0)
                {
                    _done.Set();
                }
            }

            public void Wait()
            {
                _done.Wait();
            }
        }
    }
}