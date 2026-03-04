using Editor.Utils;
using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class SceneGraphWindow : EditorWindow
    {
        public SceneGraphWindow() : base("Window/Scene Graph")
        {
        }

        public override void OnDraw()
        {
            if (OnBeginWindow("Scene graph"))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2());

                void DrawActor(Actor actor)
                {
                    ImGui.PushID(actor.GetID().ToString());

                    bool hasChildren = actor.Transform.Children.Count > 0;

                    ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth;

                    if (!hasChildren)
                        flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;

                    if (Selector.Selected && Selector.Selected == actor)
                        flags |= ImGuiTreeNodeFlags.Selected;

                    if (!actor.IsActiveInHierarchy)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f));
                    }

                    bool open = ImGui.TreeNodeEx("##node", flags);

                    var isClicked = ImGui.IsItemClicked();
                    ImGui.SameLine();
                    var iconCursorX = ImGui.GetCursorPosX();
                    var iconOffset = 8;
                    ImGui.SetCursorPosX(iconCursorX - iconOffset);
                    EditorImGui.ImageFromIcon(EditorIcon.Actor2, new GlmNet.vec2(18, 18));

                    ImGui.SameLine();
                    iconCursorX = ImGui.GetCursorPosX();
                    ImGui.SetCursorPosX(iconCursorX + iconOffset - 4);
                    ImGui.TextUnformatted(actor.Name);

                    if (!actor.IsActiveInHierarchy)
                        ImGui.PopStyleColor();

                    if (isClicked)
                        Selector.Selected = actor;

                    if (EditorImGui.BeginPopupContextItem("ActorContext"))
                    {
                        Selector.Selected = actor;

                        if (ImGui.MenuItem("Create Actor"))
                        {
                            new Actor("Actor").Transform.Parent = actor.Transform;
                        }

                        ImGui.Separator();

                        if (ImGui.MenuItem("Rename"))
                        {
                        }

                        if (ImGui.MenuItem("Duplicate"))
                        {
                        }
                        ImGui.BeginDisabled(!actor.Transform.Parent);
                        if (ImGui.MenuItem("Unparent to previous"))
                        {
                            if (actor.Transform.Parent.Parent)
                            {
                                actor.Transform.Parent = actor.Transform.Parent.Parent;
                            }
                            else
                            {
                                actor.Transform.Parent = null;
                            }
                        }
                        ImGui.EndDisabled();
                        ImGui.Separator();
                        if (ImGui.MenuItem("Delete"))
                        {
                            ImGui.EndPopup();
                            if (open && hasChildren)
                            {
                                ImGui.TreePop();
                            }
                            ImGui.PopID();

                            Actor.Destroy(actor);
                            return;
                        }

                        ImGui.EndPopup();
                    }

                    if (open && hasChildren)
                    {
                        for (int i = 0; i < actor.Transform.Children.Count; i++)
                        {
                            var child = actor.Transform.Children[i];
                            DrawActor(child.Actor);
                        }

                        ImGui.TreePop();
                    }

                    ImGui.PopID();
                }

                for (int i = SceneManager.Scenes.Count - 1; i >= 0; i--)
                {
                    ImGui.PushID(SceneManager.Scenes[i].GetID().ToString());
                    ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.DefaultOpen;

                    bool open = ImGui.TreeNodeEx("##scene_node", flags);
                    bool isClicked = ImGui.IsItemClicked();

                    ImGui.SameLine();
                    var iconCursorX = ImGui.GetCursorPosX();
                    var iconOffset = 8;
                    ImGui.SetCursorPosX(iconCursorX - iconOffset);
                    EditorImGui.ImageFromIcon(EditorIcon.Scene, new GlmNet.vec2(18, 18));

                    ImGui.SameLine();
                    iconCursorX = ImGui.GetCursorPosX();
                    ImGui.SetCursorPosX(iconCursorX + iconOffset - 4);
                    ImGui.TextUnformatted(SceneManager.Scenes[i].Name);
                    if (EditorImGui.BeginPopupContextItem("SceneContext"))
                    {
                        if (i > 0 && ImGui.MenuItem("Unload Scene"))
                        {
                            SceneManager.UnloadScene(SceneManager.Scenes[i]);
                            ImGui.EndPopup();

                            if (open)
                            {
                                ImGui.TreePop();
                            }
                            ImGui.PopID();

                            break;
                        }
                        else if (ImGui.MenuItem("Create Actor"))
                        {
                            new Actor("Actor");
                        }

                        ImGui.EndPopup();
                    }
                    if (open)
                    {
                        if (SceneManager.Scenes[i].RootActors.Count > 0)
                        {
                            ImGui.Dummy(0, 4);

                            for (int j = 0; j < SceneManager.Scenes[i].RootActors.Count; j++)
                            {
                                DrawActor(SceneManager.Scenes[i].RootActors[j]);
                            }
                        }

                        ImGui.TreePop();
                    }
                    ImGui.PopID();
                }
                ImGui.PopStyleVar();
            }

            if (Selector.Selected is Actor actor && ImGui.IsKeyDown(ImGuiKey.Delete))
            {
                Actor.Destroy(actor);
            }
            OnEndWindow();
        }
    }
}