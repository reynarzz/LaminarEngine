using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata;
using Editor;
using Engine;
using Engine.Utils;
using GlmNet;
using ImGuiNET;
using imnodesNET;

namespace Editor
{
    public class AnimatorEditorView
    {
        private int _nextId = 1;

        private readonly List<Node> _nodes = new();
        private readonly List<Link> _links = new();
        private Animator _selectedAnimator;

        public unsafe AnimatorEditorView()
        {
            imnodes.GetStyle()->link_line_segments_per_length = 0;
            imnodes.GetStyle()->node_corner_rounding = 4;
            imnodes.GetStyle()->pin_offset = -50;
            imnodes.GetStyle()->grid_spacing = 15;
            imnodes.GetStyle()->flags = StyleFlags.None;// -50;
        }

        private Node CreateNode(AnimationState state, vec2 pos)
        {
            var node = new Node(NewId(), pos);
            node.State = state;
            node.InputAttrib = NewId();
            node.OutputAttrib = NewId();

            _nodes.Add(node);
            return node;
        }

        public void Dispose()
        {
        }

        private void InitAnimatorSelected(Animator animator)
        {
            _nodes.Clear();
            _links.Clear();

            if (animator == null)
                return;

            int yPos = 100;
            int xPos = 100;

            var nodes = new Dictionary<string, Node>();

            foreach (var (currentStateName, state) in animator.States)
            {
                var node = CreateNode(state, new vec2(xPos, yPos));
                nodes.Add(currentStateName, node);
                yPos += 13;
                xPos += 13;
            }

            foreach (var (name, node) in nodes)
            {
                var transitions = node.State.Transitions;

                foreach (var transition in transitions)
                {
                    var toNode = nodes[transition.ToState];

                    var link = new Link()
                    {
                        Id = NewId(),
                        FromAttribute = node.OutputAttrib,
                        ToAttribute = toNode.InputAttrib,
                        Color = Color.RandomRGB().ToARGB_U32()
                    };
                    var color = link.Color;
                    // color.R = 1;
                    link.Color = color;
                    _links.Add(link);
                }
            }
        }

        public void OnRender()
        {
            if (Selector.SelectedTransform())
            {
                var animator = Selector.SelectedTransform().GetComponent<Animator>();

                if (animator != _selectedAnimator)
                {
                    _selectedAnimator = animator;
                    InitAnimatorSelected(animator);
                }
            }

            ImGui.Begin("Animator");
            HandleLinkCreation();
            HandleLinkDeletion();
            var dark = 0.16f;
            imnodes.PushColorStyle(ColorStyle.GridBackground, new Color(dark, dark, dark, 1).ToARGB_U32());
            imnodes.PushColorStyle(ColorStyle.GridLine, new Color(0.1f, 0.1f, 0.1f, 1).ToARGB_U32());

            imnodes.BeginNodeEditor();

            imnodes.Minimap(0.2f, MinimapLocation.BottomRight);

            ImGui.Text("Some text");

            DrawNodes();
            DrawLinks();

            imnodes.EndNodeEditor();
            imnodes.PopColorStyle();
            imnodes.PopColorStyle();
            ImGui.End();
        }
        private readonly HashSet<int> _initializedNodes = new();
        private void DrawNodes()
        {
            if (!_selectedAnimator)
                return;

            foreach (var node in _nodes)
            {
                var isCurrentState = _selectedAnimator.CurrentState == node.State;
                if (isCurrentState)
                {
                    imnodes.PushColorStyle(ColorStyle.NodeBackground, Color.DarkGray.ToARGB_U32());
                }
                imnodes.PushColorStyle(ColorStyle.NodeOutline, Color.DarkGray.ToARGB_U32());

                imnodes.BeginNode(node.Id);

                if (_initializedNodes.Add(node.Id))
                {
                    var pos = node.InitialPos;
                    imnodes.SetNodeEditorSpacePos(node.Id, pos.ToVector2());
                }
                //imnodes.BeginNodeTitleBar();
                //ImGui.TextUnformatted(node.Title);
                //imnodes.EndNodeTitleBar();


                var textSize = ImGui.CalcTextSize(node.State.Name);
                ImGui.Dummy(new Vector2(100, 10));
                var rectSize = ImGui.GetItemRectSize();
                var prevPos = ImGui.GetCursorPos();

                var centerX = prevPos.X + rectSize.X / 2.0f - textSize.X / 2.0f;
                var centerY = prevPos.Y - rectSize.Y / 2.0f - textSize.Y / 2.0f;

                var yOffset = 0;
                if (isCurrentState)
                {
                    yOffset = 8;
                }
                ImGui.SetCursorPos(new Vector2(centerX, centerY - yOffset));
                ImGui.TextUnformatted(node.State.Name);

                if (isCurrentState)
                {
                    var percent = 1.0f;
                    var duration = _selectedAnimator.CurrentState.Clip.Duration;
                    if (!Mathf.IsAlmostZero(duration))
                    {
                        percent = _selectedAnimator.CurrentStateTime / duration;
                    }

                    //  ImGui.SetCursorPos(new Vector2(prevPos.X, prevPos.Y));
                    ImGui.SetNextItemWidth(rectSize.X);
                    ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2());
                    ImGui.ProgressBar(percent, new Vector2(rectSize.X, 7), string.Empty);
                    ImGui.PopStyleVar();
                }
                ImGui.SetCursorPos(prevPos);

                // Input
                imnodes.BeginInputAttribute(node.InputAttrib);
                imnodes.EndInputAttribute();



                ImGui.SameLine();
                // Output
                {
                    imnodes.BeginOutputAttribute(node.OutputAttrib);
                    imnodes.EndOutputAttribute();
                }


                imnodes.EndNode();

                if (isCurrentState)
                {
                    imnodes.PopColorStyle();
                }
                imnodes.PopColorStyle();

            }
        }

        private void DrawLinks()
        {
            foreach (var link in _links)
            {
                if (imnodes.IsLinkSelected(link.Id))
                {
                    Debug.Log($"Link select: {link.Id}");
                }

                imnodes.PushColorStyle(ColorStyle.Link, link.Color);
                imnodes.Link(link.Id, link.FromAttribute, link.ToAttribute);
                imnodes.PopColorStyle();
            }
        }

        private void HandleLinkCreation()
        {
            int startAttr = 0;
            int endAttr = 0;
            if (imnodes.IsLinkCreated(ref startAttr, ref endAttr))
            {
                _links.Add(new Link
                {
                    Id = NewId(),
                    FromAttribute = startAttr,
                    ToAttribute = endAttr
                });
            }
        }

        private void HandleLinkDeletion()
        {
            int linkId = 0;

            if (imnodes.IsLinkDestroyed(ref linkId))
            {
                _links.RemoveAll(l => l.Id == linkId);
            }
        }

        private int NewId() => _nextId++;

        // ---------------- DATA ----------------

        private sealed class Node
        {
            public int Id;
            public vec2 InitialPos;
            public int InputAttrib;
            public int OutputAttrib;
            public AnimationState State;

            public Node(int id, vec2 pos)
            {
                Id = id;
                InitialPos = pos;
            }
        }

        private sealed class Link
        {
            public int Id;
            public int FromAttribute;
            public int ToAttribute;
            public Color Color { get; set; }
        }
    }
}