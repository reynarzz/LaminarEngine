using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Engine.Analysis
{
    public static class ProfilerRegistry
    {
        private static readonly object _lock = new object();
        private static readonly List<string> _names = new List<string>(64);

        public static ProfilerId Get(string friendlyName)
        {
            if (string.IsNullOrEmpty(friendlyName))
                throw new ArgumentException("Profiler scope name must not be null or empty.", nameof(friendlyName));

            lock (_lock)
            {
                int id = _names.Count;
                var pId = new ProfilerId(id);
                _names.Add(friendlyName);
                return pId;
            }
        }

        public static string GetName(ProfilerId id)
        {
            int v = id.Value;
            return (v >= 0 && v < _names.Count) ? _names[v] : "<unknown>";
        }

        internal static void Clear()
        {
            _names.Clear();
        }
    }

    public readonly struct ProfilerId : IEquatable<ProfilerId>
    {
        public readonly int Value;

        public ProfilerId(int value)
        {
            Value = value;
        }

        public bool IsValid => Value >= 0;

        public bool Equals(ProfilerId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ProfilerId p && Equals(p);
        public override int GetHashCode() => Value;

        public static bool operator ==(ProfilerId a, ProfilerId b) => a.Value == b.Value;
        public static bool operator !=(ProfilerId a, ProfilerId b) => a.Value != b.Value;

        public override string ToString() => ProfilerRegistry.GetName(this);
    }

    public sealed class LaminarProfiler
    {
#if DEBUG
        public static readonly ProfilerId FrameId = ProfilerRegistry.Get("Frame");

        private static readonly Dictionary<string, ProfilerId> _dynamicIds = new Dictionary<string, ProfilerId>();

        private static Node _root;

        private static readonly ThreadLocal<Context> _contexts = new ThreadLocal<Context>(() => new Context());
#endif

        static LaminarProfiler()
        {
#if DEBUG
            _root = new Node(FrameId, null);
#endif
        }

        internal static void BeginFrame()
        {
#if DEBUG
            _root.ResetFrame();

            var ctx = _contexts.Value;
            ctx.Stack.Clear();
            ctx.Stack.Add(_root);

            _root.Begin();
#endif
        }

        internal static void EndFrame()
        {
#if DEBUG
            _root.End();
#endif
        }

        public static void Begin(string name)
        {
#if DEBUG
            if (!_dynamicIds.TryGetValue(name, out var id))
            {
                id = ProfilerRegistry.Get(name);
                _dynamicIds[name] = id;
            }

            Begin(id);
#endif
        }

        public static void Begin(ProfilerId id)
        {
#if DEBUG
            var ctx = _contexts.Value;

            var parent = ctx.Stack[ctx.Stack.Count - 1];
            var node = parent.GetOrAddChild(id);

            node.Begin();
            ctx.Stack.Add(node);
#endif
        }

        public static void End()
        {
#if DEBUG
            var ctx = _contexts.Value;

            if (ctx.Stack.Count <= 1)
                return;

            int last = ctx.Stack.Count - 1;
            var node = ctx.Stack[last];

            node.End();
            ctx.Stack.RemoveAt(last);
#endif
        }

        public ProfilerScope Scope(ProfilerId id)
        {
#if DEBUG
            Begin(id);
            return new ProfilerScope();
#else
            return default;
#endif
        }

        internal static Node GetRoot() => _root;

        public static double TicksToMilliseconds(long ticks)
        {
            return ticks * 1000.0 / Stopwatch.Frequency;
        }

        public sealed class Node
        {
            public readonly ProfilerId Id;
            public readonly Node Parent;

            public readonly List<Node> Children;

            private Dictionary<int, Node> _childrenMap;

            public long ElapsedTicks;
            public int CallCount;

            private long _startTick;
            private int _activeDepth;

            public Node(ProfilerId id, Node parent)
            {
                Id = id;
                Parent = parent;
                Children = new List<Node>(4);
            }

            public string Name => ProfilerRegistry.GetName(Id);

            public void Begin()
            {
                if (_activeDepth == 0)
                {
                    _startTick = Stopwatch.GetTimestamp();
                }

                _activeDepth++;
                CallCount++;
            }

            public void End()
            {
                _activeDepth--;

                if (_activeDepth == 0)
                {
                    long end = Stopwatch.GetTimestamp();
                    ElapsedTicks += end - _startTick;
                }
            }

            public void ResetFrame()
            {
                ElapsedTicks = 0;
                CallCount = 0;
                _activeDepth = 0;

                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].ResetFrame();
                }
            }

            public Node GetOrAddChild(ProfilerId id)
            {
                if (_childrenMap == null)
                {
                    _childrenMap = new Dictionary<int, Node>(4);
                }

                if (_childrenMap.TryGetValue(id.Value, out var node))
                {
                    return node;
                }

                node = new Node(id, this);

                Children.Add(node);
                _childrenMap[id.Value] = node;

                return node;
            }

            public Node Clone(Node newParent = null)
            {
                var copy = new Node(Id, newParent)
                {
                    ElapsedTicks = ElapsedTicks,
                    CallCount = CallCount
                };

                for (int i = 0; i < Children.Count; i++)
                {
                    var childCopy = Children[i].Clone(copy);
                    copy.Children.Add(childCopy);

                    if (copy._childrenMap == null)
                        copy._childrenMap = new Dictionary<int, Node>(Children.Count);

                    copy._childrenMap[childCopy.Id.Value] = childCopy;
                }

                return copy;
            }
        }

        public readonly struct ProfilerScope : IDisposable
        {
            public void Dispose()
            {
#if DEBUG
                LaminarProfiler.End();
#endif
            }
        }

        private sealed class Context
        {
            public readonly List<Node> Stack = new List<Node>(32);
        }
    }

    internal static class EngineProfilerIds
    {
        internal static readonly ProfilerId Update = ProfilerRegistry.Get("Update");
        internal static readonly ProfilerId Render = ProfilerRegistry.Get("Render");
        internal static readonly ProfilerId Physics = ProfilerRegistry.Get("Physics");
        internal static readonly ProfilerId CleanupLayer = ProfilerRegistry.Get("CleanupLayer");
    }
}