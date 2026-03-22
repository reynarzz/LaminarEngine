using Engine.Analysis;
using ImGuiNET;
using System.Numerics;

namespace Editor.Views
{
    internal class ProfilerWindow : EditorWindow
    {
        private int _id = 0;
        private LaminarProfiler.Node _rootSnapshot;
        public ProfilerWindow() : base("Window/Profiler")
        {
        }

        public override void OnDraw()
        {
            if (OnBeginWindow("Profiler"))
            {
                if(ImGui.Button("Take Snapshot"))
                {
                    _rootSnapshot = LaminarProfiler.GetRoot().Clone();
                }

                if (_rootSnapshot != null)
                {
                    // Flamegraph (main view)
                    DrawFlameGraph(_rootSnapshot);
                }

                // Optional: table view below
                if (ImGui.CollapsingHeader("Hierarchy"))
                {
                    if (ImGui.BeginTable("ProfilerTable", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg))
                    {
                        ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);
                        ImGui.TableSetupColumn("Time (ms)", ImGuiTableColumnFlags.WidthFixed, 100);
                        ImGui.TableSetupColumn("Calls", ImGuiTableColumnFlags.WidthFixed, 80);

                        ImGui.TableHeadersRow();

                        _id = 0;
                        if (_rootSnapshot != null)
                        {
                            DrawNode(_rootSnapshot);
                        }

                        ImGui.EndTable();
                    }
                }
            }

            OnEndWindow();
        }

        // -------------------------
        // Flamegraph
        // -------------------------

        private void DrawFlameGraph(LaminarProfiler.Node root)
        {
            var drawList = ImGui.GetWindowDrawList();

            Vector2 origin = ImGui.GetCursorScreenPos();
            float width = ImGui.GetContentRegionAvail().X;
            float barHeight = 18.0f;

            long total = root.ElapsedTicks;
            if (total <= 0)
                return;

            DrawNodeRecursive(root, 0, 0.0f, width, root.ElapsedTicks, drawList, origin);

            ImGui.Dummy(new Vector2(width, GetMaxDepth(root) * barHeight));
        }
        private void DrawNodeRecursive(
    LaminarProfiler.Node node,
    int depth,
    float xOffset,
    float totalWidth,
    long rootTicks,
    ImDrawListPtr drawList,
    Vector2 origin)
        {
            if (node.ElapsedTicks <= 0)
                return;

            float barHeight = 18.0f;

            // width relative to ROOT (not parent)
            float ratio = rootTicks > 0
                ? (float)node.ElapsedTicks / rootTicks
                : 0f;

            float w = totalWidth * ratio;

            float x0 = origin.X + xOffset;
            float y0 = origin.Y + depth * barHeight;
            float x1 = x0 + w;
            float y1 = y0 + barHeight - 2;

            uint color = GetColor(node.Id.Value);

            drawList.AddRectFilled(new Vector2(x0, y0), new Vector2(x1, y1), color);

            if (w > 40.0f)
            {
                drawList.AddText(new Vector2(x0 + 3, y0 + 2), 0xFFFFFFFF, node.Name);
            }

            if (ImGui.IsMouseHoveringRect(new Vector2(x0, y0), new Vector2(x1, y1)))
            {
                double ms = LaminarProfiler.TicksToMilliseconds(node.ElapsedTicks);

                ImGui.BeginTooltip();
                ImGui.Text(node.Name);
                ImGui.Text($"{ms:F3} ms");
                ImGui.Text($"Calls: {node.CallCount}");
                ImGui.EndTooltip();
            }

            // IMPORTANT: children are laid out sequentially using ROOT scale
            float childOffset = xOffset;

            if (node.Children.Count > 1)
            {
                node.Children.Sort((a, b) => b.ElapsedTicks.CompareTo(a.ElapsedTicks));
            }

            for (int i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];

                float childRatio = rootTicks > 0
                    ? (float)child.ElapsedTicks / rootTicks
                    : 0f;

                float childWidth = totalWidth * childRatio;

                DrawNodeRecursive(
                    child,
                    depth + 1,
                    childOffset,
                    totalWidth,
                    rootTicks,
                    drawList,
                    origin);

                childOffset += childWidth;
            }
        }

        private int GetMaxDepth(LaminarProfiler.Node node)
        {
            int max = 0;

            for (int i = 0; i < node.Children.Count; i++)
            {
                int d = GetMaxDepth(node.Children[i]);
                if (d > max)
                    max = d;
            }

            return max + 1;
        }

        private uint GetColor(int id)
        {
            uint hash = (uint)id * 2654435761u;

            float r = ((hash >> 0) & 0xFF) / 255.0f;
            float g = ((hash >> 8) & 0xFF) / 255.0f;
            float b = ((hash >> 16) & 0xFF) / 255.0f;

            return ImGui.ColorConvertFloat4ToU32(new Vector4(r, g, b, 1.0f));
        }

        // -------------------------
        // Table (optional)
        // -------------------------

        private void DrawNode(LaminarProfiler.Node node)
        {
            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);

            ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.SpanFullWidth;

            if (node.Children.Count == 0)
                flags |= ImGuiTreeNodeFlags.Leaf;

            bool open = ImGui.TreeNodeEx($"###node_{_id++}", flags, node.Name);

            ImGui.TableSetColumnIndex(1);
            double ms = LaminarProfiler.TicksToMilliseconds(node.ElapsedTicks);
            ImGui.Text($"{ms:F3}");

            ImGui.TableSetColumnIndex(2);
            ImGui.Text($"{node.CallCount}");

            if (open)
            {
                node.Children.Sort((a, b) => b.ElapsedTicks.CompareTo(a.ElapsedTicks)); // Add this
                for (int i = 0; i < node.Children.Count; i++)
                {
                    DrawNode(node.Children[i]);
                }

                ImGui.TreePop();
            }
        }
    }
}