using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata;
using ImGuiNET;
using imnodesNET;

public class AnimatorEditorView
{
    private int _nextId = 1;

    private readonly List<Node> _nodes = new();
    private readonly List<Link> _links = new();

    public AnimatorEditorView()
    {
        //imnodes.StyleColorsDark();
        // Create one demo node
        var entry = CreateNode("Entry", new Vector2(100, 200), null);
        CreateNode("Second node", new Vector2(200, 300), entry);
    }

    private Node CreateNode(string name, Vector2 pos, Node link)
    {
        var node = new Node(NewId(), name, pos);
        node.Inputs.Add(NewId());
        node.Outputs.Add(NewId());
        
        _nodes.Add(node);
        return node;
    }

    public void Dispose()
    {
    }

    public void OnRender()
    {
        ImGui.Begin("Animator");
        HandleLinkCreation();
        HandleLinkDeletion();
        imnodes.BeginNodeEditor();
        
        imnodes.Minimap(0.2f,  MinimapLocation.BottomRight);

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
                Vector2 pos = node.InitialPos;
                imnodes.SetNodeEditorSpacePos(node.Id, pos);
            }

            imnodes.BeginNodeTitleBar();
            ImGui.TextUnformatted(node.Title);

            imnodes.EndNodeTitleBar();

            foreach (int input in node.Inputs)
            {
                imnodes.BeginInputAttribute(input);
                ImGui.Dummy(new Vector2(10,10));
                imnodes.EndInputAttribute();
            }
            ImGui.SameLine();
            foreach (int output in node.Outputs)
            {
                imnodes.BeginOutputAttribute(output);
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
        public Vector2 InitialPos;
        public List<int> Inputs = new();
        public List<int> Outputs = new();

        public Node(int id, string title, Vector2 pos)
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
