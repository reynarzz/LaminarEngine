using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Serialization;
using GlmNet;
using ImGuiNET;
using System;
using System.Numerics;

namespace Editor.Views
{
    internal class SceneGraphWindow : EditorWindow
    {
        private Guid? _pressedActorId;

        public SceneGraphWindow() : base("Window/Scene Graph")
        {
        }

        public override void OnDraw()
        {
            if (OnBeginWindow("Scene graph", ImGuiWindowFlags.None, true, new GlmNet.vec2()))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2());

                for (int i = SceneManager.Scenes.Count - 1; i >= 0; i--)
                {
                    if (i == 0 && !Application.IsInPlayMode)
                    {
                        continue;
                    }
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
                        if (i > 0)
                        {
                            if (ImGui.MenuItem("Reload Scene"))
                            {

                            }
                            else if (ImGui.MenuItem("Unload Scene"))
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
                            if (ImGui.MenuItem("Create Actor"))
                            {
                                new Actor("Actor");
                            }
                        }


                        ImGui.EndPopup();
                    }

                    if (open)
                    {
                        if (SceneManager.Scenes[i].RootActors.Count > 0)
                        {
                            ImGui.Dummy(0, 4);
                            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, 15.5f);

                            for (int j = 0; j < SceneManager.Scenes[i].RootActors.Count; j++)
                            {
                                DrawActor(SceneManager.Scenes[i].RootActors[j]);
                            }
                            ImGui.PopStyleVar();
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

        private void DrawActor(Actor actor)
        {
            ImGui.PushID(actor.GetID().ToString());

            bool hasChildren = actor.Transform.Children.Count > 0;

            ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth;

            if (!hasChildren)
                flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;

            if (Selector.Selected && Selector.Selected == actor)
                flags |= ImGuiTreeNodeFlags.Selected;

            var tintColor = new vec4(1, 1, 1, 1);
            if (!actor.IsActiveInHierarchy)
            {
                const float disabledAlpha = 0.6f;
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(tintColor.x, tintColor.y, tintColor.z, disabledAlpha));
                tintColor.w = disabledAlpha;
            }

            var greenSelected = EditorColors.MainColor.ToVector4();
            greenSelected.W = 0.6f;
            ImGui.PushStyleColor(ImGuiCol.HeaderActive, greenSelected);

            bool isSelected = Selector.Selected && Selector.Selected == actor;
            var headerColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Header];

            var anotherIsDrag = (_pressedActorId != null && _pressedActorId != actor.GetID());

            if (!isSelected || anotherIsDrag)
            {
                if (anotherIsDrag)
                {
                    ImGui.PushStyleColor(ImGuiCol.Header, headerColor);
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, greenSelected);
                }
                else
                {
                    ImGui.PushStyleColor(ImGuiCol.Header, headerColor);
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, headerColor);
                }

            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Header, greenSelected);
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, greenSelected);
            }

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 1f));
            bool open = ImGui.TreeNodeEx("##node", flags);
            EditorImGui.DragAndDrop.ItemDragReference(actor.Name, EditorImGui.DragAndDrop.PAYLOAD_ID_EOBJECT, actor, actor.GetType(), actor.GetID());
            var id = actor.GetID();

            if (ImGui.IsItemActivated())
            {
                _pressedActorId = id;
            }

            if (ImGui.IsItemDeactivated())
            {
                if (_pressedActorId.HasValue && _pressedActorId.Value == id && ImGui.IsItemHovered())
                {
                    Selector.Selected = actor;
                }

                _pressedActorId = null;
            }

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
                ImGui.BeginDisabled();
                if (ImGui.MenuItem("Duplicate"))
                {
                    // TODO: create and re-assign the new instance ids of actors and components.

                    var actorsTree = SceneSerializer.SerializeActorsTree(actor);
                    var actorsRootInstances = SceneDeserializer.DeserializeScene(actorsTree, actor.Scene, true);

                    if (actorsRootInstances != null)
                    {
                        foreach (var actorRoot in actorsRootInstances)
                        {
                            actorRoot.Transform.Parent = actor.Transform.Parent;
                        }
                    }
                }
                ImGui.EndDisabled();

                ImGui.BeginDisabled(!actor.Transform.Parent);

                if (ImGui.MenuItem("Clear parent"))
                {
                    actor.Transform.Parent = null;
                }

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

            ImGui.PopStyleColor(4);

            ImGui.SameLine();
            var iconCursorX = ImGui.GetCursorPosX();
            var iconOffset = 8;
            ImGui.SetCursorPosX(iconCursorX - iconOffset);
            EditorImGui.ImageFromIcon(EditorIcon.Actor2, new GlmNet.vec2(18, 18), tintColor);

            ImGui.SameLine();
            iconCursorX = ImGui.GetCursorPosX();
            ImGui.SetCursorPosX(iconCursorX + iconOffset - 4);
            ImGui.TextUnformatted(actor.Name);

            if (!actor.IsActiveInHierarchy)
                ImGui.PopStyleColor();

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
    }
}