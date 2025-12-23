using Editor.Views;
using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class ObjectEditorView : IEditorWindow
    {
        public ObjectEditorView()
        {
            // TODO: factory for all the EOjects types.
        }
        public void OnClose()
        {
        }

        public void OnOpen()
        {
        }

        public void OnUpdate()
        {
        }

        public void OnRender()
        {
            ImGui.Begin("Object Editor");

            if (Selector.Selected && Selector.Selected is Actor actor)
            {
                ImGui.Text("-" + actor.Name);
                for (int i = 0; i < actor.Components.Count; i++)
                {
                    var component = actor.Components[i];
                    var properties = component.GetType().GetProperties();
                    DrawComponentTree(component, i, x =>
                    {
                        if (component)
                        {
                            for (int j = 0; j < properties.Length; j++)
                            {
                                PropertyDrawer.DrawVars(x.GetID().ToString(), component, properties[j], 0, j, 0, true);
                            }
                        }
                    });
                }
            }

            ImGui.End();
        }

        private static object _componentClipboard;

        public static void DrawComponentTree(Component component, int componentIndex, Action<Component> drawProperties)
        {
            string componentID = component.GetID().ToString();
            string baseID = $"__COMPONENT_{componentID}"; 

            var flags = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.AllowOverlap |
                ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.NoTreePushOnOpen;

            bool componentHeader = ImGui.TreeNodeEx($"##{baseID}", flags);

            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup($"##POPUP_{baseID}"); 
            }

            ImGui.SameLine();

            // Checkbox for component enabled
            bool enabled = component.IsEnabled;
            var cursorY = ImGui.GetCursorPosY();
            var cursorX = ImGui.GetCursorPosX();

            ImGui.SetCursorPosX(cursorX + 10);
            ImGui.SetCursorPosY(cursorY);

            if (ImGui.Checkbox($"##__COMPONENT__ENABLED_{baseID}", ref enabled)) 
            {
                component.IsEnabled = enabled;
            }
            ImGui.SetCursorPosX(cursorX + 12);
            ImGui.SameLine();

            // Component icon
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 45);
            ImGui.SetCursorPosY(cursorY + 2);
            IntPtr imagePtr = default; // TODO: get component icon.

            if(imagePtr != IntPtr.Zero)
            {
          
                ImGui.Image(imagePtr, new Vector2(16, 16));

            }

            // Component name
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() );
            ImGui.SetCursorPosY(cursorY);
            ImGui.Text(component.GetType().Name);

            // Context menu
            if (ImGui.BeginPopup($"##POPUP_{baseID}")) 
            {
                if (ImGui.MenuItem("Reset"))
                {
                    // Reset component logic
                    if (component is IDisposable disposable) disposable.Dispose();
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Copy values"))
                {
                    _componentClipboard = component; 
                }

                bool canPaste = _componentClipboard != null && _componentClipboard.GetType() == component.GetType();
                ImGui.BeginDisabled(!canPaste);
                if (ImGui.MenuItem("Paste values"))
                {
                    // TODO:
                }
                ImGui.EndDisabled();

                bool shouldDisableRemove = componentIndex <= 0;
                ImGui.BeginDisabled(shouldDisableRemove);
                ImGui.Separator();
                if (ImGui.MenuItem("Remove"))
                {
                    Actor.Destroy(component);
                    ImGui.EndDisabled();
                    ImGui.EndPopup();
                    return;
                }
                ImGui.EndDisabled();

                ImGui.EndPopup();
            }

            ImGui.SameLine();
            ImGui.Dummy(new Vector2(-200, 0));

            // Draw component properties
            void DrawComponentProperties()
            {
                drawProperties?.Invoke(component);
            }

            if (componentHeader)
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetWindowSize().X - 40);

                ImGui.Spacing();

                DrawComponentProperties();
            }
        }

    }
}
