using Editor.Views;
using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
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

            if(Selector.Selected && Selector.Selected is Actor actor)
            {
                ImGui.Text("-" + actor.Name);
                for (int i = 0; i < actor.Components.Count; i++)
                {
                    var component = actor.Components[i];
                    ImGui.Text(component.GetType().Name);

                    var properties = component.GetType().GetProperties();
                    for (int j = 0; j < properties.Length; j++)
                    {
                        PropertyDrawer.DrawVars(component.GetID().ToString(), component, properties[j],0, j, 0, true);
                    }
                }

            }

            ImGui.End();
        }
    }
}
