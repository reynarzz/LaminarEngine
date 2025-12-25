using Editor.Utils;
using Editor.Views;
using Engine;
using Engine.Utils;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
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

            if (Selector.Selected && Selector.Selected.IsAlive && Selector.Selected is Actor actor)
            {
                DrawActor(actor);

                for (int i = 0; i < actor.Components.Count; i++)
                {
                    var component = actor.Components[i];
                    var members = ReflectionUtils.GetAllMembersWithAttribute<ExposeEditorFieldAttribute>(component.GetType(), true, true);
                    DrawComponentTree(component, i, x =>
                    {
                        if (component)
                        {
                            int index = 0;
                            foreach (var member in members)
                            {
                                PropertyDrawer.DrawVars(x.GetID().ToString(), component, member, 0, index, 0, true);

                                index++;
                            }

                        }
                    });
                }
            }

            ImGui.End();
        }


        private void DrawActor(Actor actor)
        {
            BeginGroupPanel(actor.Name, new(ImGui.GetContentRegionAvail().X, 0));

            var isEnabled = actor.IsActiveSelf;
            if (EditorGuiFieldsResolver.DrawBoolField("##__ENABLE_ACTOR__", ref isEnabled))
            {
                actor.IsActiveSelf = isEnabled;
            }

            ImGui.SameLine();

            var actorName = actor.Name;
            if (EditorGuiFieldsResolver.DrawStringField("##_ACTOR_NAME_", ref actorName))
            {
                actor.Name = actorName;
            }

            ImGui.Text("Layer");
            ImGui.SameLine();
            var layerNames = LayerMask.GetLayerNames();
            int layerIndex = Array.IndexOf(layerNames, LayerMask.LayerToName(actor.Layer));

            if (EditorGuiFieldsResolver.DrawCombo("Layers", ref layerIndex, layerNames))
            {
                actor.Layer = LayerMask.NameToLayer(layerNames[layerIndex]);
            }
            if (ImGui.Button("Add Component", new Vector2(ImGui.GetContentRegionAvail().X, 23)))
            {
                ImGui.OpenPopup("DropdownPopup");
            }

            // Set the position and size for the popup
            Vector2 popupPos = ImGui.GetCursorScreenPos();
            popupPos.Y -= 3.5f;

            ImGui.SetNextWindowPos(new Vector2(popupPos.X, popupPos.Y));
            ImGui.SetNextWindowSize(new Vector2(ImGui.GetContentRegionAvail().X, 260.0f));

            if (ImGui.BeginPopup("DropdownPopup"))
            {
                string search = string.Empty;

                ImGui.Text("Search");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 5);

                // Replace with your custom text input / field renderer
                EditorGuiFieldsResolver.DrawStringField("##Component_Search_bar", ref search);

                ImGui.Separator();
                ImGui.Spacing();

                ImGui.BeginChild("##ComponentListChild", Vector2.Zero, ImGuiChildFlags.None, ImGuiWindowFlags.None);

                var allComponentTypes = GetAllComponentTypes();
                foreach (var componentType in allComponentTypes)
                {
                    // ImGui.Image(EditorIcons.GetIcon(EditorIconType.ScriptIcon), new Vector2(15, 15));
                    // ImGui.SameLine();

                    if (ImGui.Selectable(componentType.Name))
                    {
                        actor.AddComponent(componentType);
                        ImGui.CloseCurrentPopup();
                    }
                }

                ImGui.EndChild();
                ImGui.EndPopup();
            }

            EndGroupPanel();
        }

        private static readonly List<Type> _componentTypes = new();

        // NOTE: This will always be called after hot reloading the game dll, for now, for quick test I'm calling it every frame,
        public static IReadOnlyList<Type> GetAllComponentTypes()
        {
            Type baseType = typeof(Component);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _componentTypes.Clear();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types;

                try
                {
                    types = assemblies[i].GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types;
                }

                if (types == null)
                    continue;

                for (int j = 0; j < types.Length; j++)
                {
                    Type t = types[j];
                    if (t == null)
                        continue;

                    if (!t.IsClass)
                        continue;

                    if (t.IsAbstract)
                        continue;

                    if (!baseType.IsAssignableFrom(t))
                        continue;

                    _componentTypes.Add(t);
                }
            }

            _componentTypes.Sort(static (a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            return _componentTypes;
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

            if (imagePtr != IntPtr.Zero)
            {

                ImGui.Image(imagePtr, new Vector2(16, 16));

            }

            // Component name
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetCursorPosX());
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

        private static List<GuiRect> s_GroupPanelLabelStack = new();

        public static void BeginGroupPanel(string name, Vector2 size)
        {
            ImGui.BeginGroup();

            Vector2 itemSpacing = ImGui.GetStyle().ItemSpacing;
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);

            float frameHeight = ImGui.GetFrameHeight();
            ImGui.BeginGroup();

            // effective size
            Vector2 effectiveSize = size;
            if (size.X < 0)
                effectiveSize.X = ImGui.GetContentRegionAvail().X;

            ImGui.Dummy(new Vector2(effectiveSize.X, 0.0f));
            ImGui.Dummy(new Vector2(frameHeight * 0.5f, 0.0f));
            ImGui.SameLine(0.0f, 0.0f);

            ImGui.BeginGroup();
            ImGui.Dummy(new Vector2(frameHeight * 0.5f, 0.0f));
            ImGui.SameLine(0.0f, 0.0f);
            ImGui.TextUnformatted(name);

            Vector2 labelMin = ImGui.GetItemRectMin();
            Vector2 labelMax = ImGui.GetItemRectMax();

            ImGui.SameLine(0.0f, 0.0f);
            ImGui.Dummy(new Vector2(0.0f, frameHeight + itemSpacing.Y));
            ImGui.BeginGroup();

            ImGui.PopStyleVar(2);

            // Adjust item width (without window internals)
            float itemWidth = ImGui.CalcItemWidth();
            ImGui.PushItemWidth(MathF.Max(0.0f, itemWidth - frameHeight));

            s_GroupPanelLabelStack.Add(new GuiRect(labelMin, labelMax));
        }
        private struct GuiRect
        {
            public float x;
            public float y;
            public Vector2 Min;
            public Vector2 Max;

            public GuiRect(Vector2 min, Vector2 max)
            {
                Min = min; Max = max;
            }
        }
        public static void EndGroupPanel(bool drawFilledRect = false, Vector4? color = null)
        {
            ImGui.PopItemWidth();
            Vector2 itemSpacing = ImGui.GetStyle().ItemSpacing;
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);

            float frameHeight = ImGui.GetFrameHeight();
            ImGui.EndGroup(); // inner group

            if (drawFilledRect && color.HasValue)
            {
                uint col = ImGui.ColorConvertFloat4ToU32(color.Value);
                ImGui.GetWindowDrawList().AddRectFilled(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), col, 4.0f);
            }

            ImGui.EndGroup();
            ImGui.SameLine(0.0f, 0.0f);
            ImGui.Dummy(new Vector2(frameHeight * 0.5f, 0.0f));
            ImGui.Dummy(new Vector2(0.0f, (frameHeight * 0.5f - itemSpacing.Y) + 9));
            ImGui.EndGroup();

            Vector2 itemMin = ImGui.GetItemRectMin();
            Vector2 itemMax = ImGui.GetItemRectMax();

            GuiRect labelRect = s_GroupPanelLabelStack[^1];
            s_GroupPanelLabelStack.RemoveAt(s_GroupPanelLabelStack.Count - 1);

            Vector2 halfFrame = new Vector2(frameHeight * 0.25f, frameHeight) * 0.5f;
            GuiRect frameRect = new(itemMin + halfFrame, itemMax - new Vector2(halfFrame.X, 0.0f));

            // Expand label rect for cutout
            labelRect.Min.X -= itemSpacing.X;
            labelRect.Max.X += itemSpacing.X;

            var drawList = ImGui.GetWindowDrawList();
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0: ImGui.PushClipRect(new Vector2(-float.MaxValue, -float.MaxValue), new Vector2(labelRect.Min.X, float.MaxValue), true); break;
                    case 1: ImGui.PushClipRect(new Vector2(labelRect.Max.X, -float.MaxValue), new Vector2(float.MaxValue, float.MaxValue), true); break;
                    case 2: ImGui.PushClipRect(new Vector2(labelRect.Min.X, -float.MaxValue), new Vector2(labelRect.Max.X, labelRect.Min.Y), true); break;
                    case 3: ImGui.PushClipRect(new Vector2(labelRect.Min.X, labelRect.Max.Y), new Vector2(labelRect.Max.X, float.MaxValue), true); break;
                }

                drawList.AddRect(frameRect.Min, frameRect.Max, ImGui.GetColorU32(ImGuiCol.Border), halfFrame.X);
                ImGui.PopClipRect();
            }

            ImGui.PopStyleVar(2);
            ImGui.Dummy(Vector2.Zero);
            ImGui.EndGroup();
        }

    }
}
