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
    internal class SceneGraphWindow : IEditorWindow
    {
        public void OnClose()
        {
        }

        public void OnOpen()
        {
        }
        private Actor _selectedActor;
        public void OnRender()
        {
            ImGui.Begin("Scene graph");

            void DrawActor(Actor actor)
            {
                ImGui.PushID(actor.GetID().ToString());

                bool hasChildren = actor.Transform.Children.Count > 0;

                ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth;

                if (!hasChildren)
                    flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;

                if (_selectedActor == actor)
                    flags |= ImGuiTreeNodeFlags.Selected;

                if (!actor.IsActiveInHierarchy)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f));
                }

                bool open = ImGui.TreeNodeEx(actor.Name, flags);

                if (!actor.IsActiveInHierarchy)
                    ImGui.PopStyleColor();

                // Selection handling
                if (ImGui.IsItemClicked())
                    _selectedActor = actor;

                if (ImGui.BeginPopupContextItem("ActorContext"))
                {
                    _selectedActor = actor;

                    if (ImGui.MenuItem("Create Actor"))
                    {
                        new Actor("Actor").Transform.Parent = _selectedActor.Transform;
                    }

                    ImGui.Separator();

                    if (ImGui.MenuItem("Rename")) 
                    {
                        
                    }
                    
                    if (ImGui.MenuItem("Duplicate")) 
                    {
                        
                    }

                    ImGui.Separator();
                    if (ImGui.MenuItem("Delete"))
                    {

                        ImGui.EndPopup();
                        if (open && hasChildren)
                        {
                            ImGui.TreePop();
                        }
                        ImGui.PopID();

                        Actor.Destroy(_selectedActor);
                        return;
                    }

                    ImGui.EndPopup();
                }

                if (open && hasChildren)
                {
                    foreach (var child in actor.Transform.Children)
                        DrawActor(child.Actor);

                    ImGui.TreePop();
                }

                ImGui.PopID();
            }

            for (int i = 0; i < SceneManager.Scenes.Count; i++)
            {
                ImGui.PushID(SceneManager.Scenes[i].GetID().ToString());
                ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.DefaultOpen;

                bool open = ImGui.TreeNodeEx(SceneManager.Scenes[i].Name, flags);
                if (open)
                {
                    if (SceneManager.Scenes[i].RootActors.Count > 0)
                    {
                        for (int j = 0; j < SceneManager.Scenes[i].RootActors.Count; j++)
                        {
                            DrawActor(SceneManager.Scenes[i].RootActors[j]);
                        }
                    }

                    ImGui.TreePop();
                }
                ImGui.PopID();
            }

            ImGui.End();
        }
     

        public void OnUpdate()
        {
        }
    }
}
