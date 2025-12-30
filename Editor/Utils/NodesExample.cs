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

public class AnimatorEditorView
{
    private int _nextId = 1;

    private readonly List<Node> _nodes = new();
    private readonly List<Link> _links = new();
    private Animator _selectedAnimator;

    public AnimatorEditorView()
    {

    }

    private Node CreateNode(string name, vec2 pos)
    {
        var node = new Node(NewId(), name, pos);
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
        if (animator == null)
            return;
        _nodes.Clear();

        int yPos = 0;
        int xPos = 0;

        var nodes = new Dictionary<string, Node>();

        foreach (var (currentStateName, state) in animator.States)
        {
            var node = CreateNode(currentStateName, new vec2(xPos, yPos));
            node.State = state;

            nodes.Add(currentStateName, node);
            yPos += 3;
            xPos += 3;
        }


        foreach (var (name, node) in nodes)
        {
            var transitions = node.State.Transitions;

            foreach (var transition in transitions)
            {
                var toNode = nodes[transition.ToState];

                _links.Add(new Link() { Id = NewId(), FromAttribute = node.OutputAttrib, ToAttribute = toNode.InputAttrib });
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
        imnodes.BeginNodeEditor();

        imnodes.Minimap(0.2f, MinimapLocation.BottomRight);

        ImGui.Text("Some text");

        DrawNodes();
        DrawLinks();

        imnodes.EndNodeEditor();
        ImGui.End();
    }
    private readonly HashSet<int> _initializedNodes = new();
    private void DrawNodes()
    {
        foreach (var node in _nodes)
        {
            imnodes.BeginNode(node.Id);

            if (_initializedNodes.Add(node.Id))
            {
                var pos = node.InitialPos;
                imnodes.SetNodeEditorSpacePos(node.Id, pos.ToVector2());
            }

            imnodes.BeginNodeTitleBar();
            ImGui.TextUnformatted(node.Title);

            imnodes.EndNodeTitleBar();

            // Input
            imnodes.BeginInputAttribute(node.InputAttrib);
            ImGui.Dummy(new Vector2(10, 10));
            imnodes.EndInputAttribute();

            ImGui.SameLine();
            // Output
            {
                imnodes.BeginOutputAttribute(node.OutputAttrib);
                ImGui.Dummy(new Vector2(10, 10));

                imnodes.EndOutputAttribute();


            }


            imnodes.EndNode();
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
            imnodes.Link(link.Id, link.FromAttribute, link.ToAttribute);
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
        public string Title;
        public vec2 InitialPos;
        public int InputAttrib;
        public int OutputAttrib;
        public AnimationState State;

        public Node(int id, string title, vec2 pos)
        {
            Id = id;
            Title = title;
            InitialPos = pos;
        }
    }


    private sealed class Link
    {
        public int Id;
        public int FromAttribute;
        public int ToAttribute;
    }
}
