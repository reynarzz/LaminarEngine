using Engine;
using Engine.Utils;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Editor.Utils;
using Editor.Serialization;
using System.Reflection;
using GlmNet;

namespace Editor.Drawers
{
    internal class ActorInspectorDrawer : EditorDrawerBase<Actor>
    {
        private static readonly List<Type> _componentTypes = new();

        private readonly static Type[] _visibilityAttributes = [typeof(SerializedFieldAttribute), typeof(ShowFieldNoSerialize)];
        protected override bool AutoDrawTitle => false;
        internal override vec2? WindowsPadding => new vec2();
        public ActorInspectorDrawer()
        {
        }

        protected override void OnDraw(Actor actor)
        {
            DrawActor(actor);

            for (int i = 0; i < actor.Components.Count; i++)
            {
                var component = actor.Components[i];
                var members = ReflectionUtils.GetAllMembersWithAttributes(component.GetType(), _visibilityAttributes, true, true);
                DrawComponentTree(component, i, x =>
                {
                    if (component)
                    {
                        int index = 0;
                        foreach (var member in members)
                        {
                            if (PropertiesGUIDrawEditor.DrawVars(x.GetID().ToString(), component, member, 0, index, 0, true))
                            {
                                EditorSystem.Save.MarkDirty(actor);
                            }
                            index++;
                        }
                    }
                });

                PropertiesGUIDrawEditor.DrawMethods(component, component.GetID().ToString());
            }
        }

        private static void DrawID(string id, EObject obj)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 1, 1, 0.35f));
            ImGui.Indent(EditorGuiFieldsResolver.GetIdentation());
            ImGui.Text(id);
            ImGui.SameLine();
            EditorGuiFieldsResolver.SetPropertyDefaultCursorPos();
            ImGui.Text(obj.GetID().ToString());
            ImGui.Unindent(EditorGuiFieldsResolver.GetIdentation());
            ImGui.PopStyleColor();
        }
        private void DrawActor(Actor actor)
        {
            var drawList = ImGui.GetWindowDrawList();

            var pos = ImGui.GetCursorScreenPos();
            float width = ImGui.GetContentRegionAvail().X;
            float height = 100.0f;

            uint color = ImGui.GetColorU32(new Vector4(0.14f, 0.14f, 0.14f, 1.0f));

            drawList.AddRectFilled(pos, new Vector2(pos.X + width + 1, pos.Y + height), color);
            // BeginGroupPanel(actor.Name, new(ImGui.GetContentRegionAvail().X, 0));

            DrawID("Actor", actor);

            var identationAmount = 6;
            ImGui.Indent(identationAmount);
            ImGui.Dummy(0, 10);
            var isEnabled = actor.IsActiveSelf;
            if (EditorGuiFieldsResolver.DrawBoolField("##__ENABLE_ACTOR__", ref isEnabled))
            {
                EditorSystem.Save.MarkDirty(actor);
                actor.IsActiveSelf = isEnabled;
            }

            ImGui.SameLine();

            var actorName = actor.Name;
            if (EditorGuiFieldsResolver.DrawStringField("##_ACTOR_NAME_", ref actorName, 0, true))
            {
                EditorSystem.Save.MarkDirty(actor);
                actor.Name = actorName;
                Debug.Log("Changed name: " + actor.Name);
            }
            EditorImGui.ImageFromIcon(EditorIcon.Layer, new vec2(16, 16));
            ImGui.SameLine();
            ImGui.Text("Layer");
            ImGui.SameLine();
            var layerNames = LayerMask.GetAllLayerNames();
            int layerIndex = Array.IndexOf(layerNames, LayerMask.LayerToName(actor.Layer));

            if (EditorGuiFieldsResolver.DrawCombo("Layers", ref layerIndex, layerNames, 46))
            {
                EditorSystem.Save.MarkDirty(actor);
                actor.Layer = LayerMask.NameToLayer(layerNames[layerIndex]);
            }
            ImGui.SameLine();
            if (EditorImGui.ImageButton("##_ACTOR_EDIT_LAYER_", EditorTextureDatabase.GetIconImGui(EditorIcon.Edit), new vec2(20, 20)))
            {

            }
            if (ImGui.Button("Add Component", new Vector2(ImGui.GetContentRegionAvail().X - 5, 23)))
            {
                PopulateAllComponentTypes();
                ImGui.OpenPopup("DropdownPopup");
            }
            ImGui.Unindent(identationAmount);
            ImGui.Dummy(0, 5);
            // Set the position and size for the popup
            Vector2 popupPos = ImGui.GetCursorScreenPos();
            popupPos.Y -= 3.5f;

            ImGui.SetNextWindowPos(new Vector2(popupPos.X, popupPos.Y));
            ImGui.SetNextWindowSize(new Vector2(ImGui.GetContentRegionAvail().X, 260.0f));

            if (EditorImGui.BeginPopup("DropdownPopup"))
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

                foreach (var componentType in _componentTypes)
                {
                    EditorImGui.Image(EditorTextureDatabase.GetIconImGui(EditorIcon.ScriptFile), new vec2(15, 15));
                    ImGui.SameLine();

                    if (ImGui.Selectable($"{componentType.Name}##{componentType.AssemblyQualifiedName}"))
                    {
                        actor.AddComponent(componentType);
                        EditorSystem.Save.MarkDirty(actor);

                        ImGui.CloseCurrentPopup();
                    }
                }

                ImGui.EndChild();
                ImGui.EndPopup();
            }

            // EndGroupPanel();
        }


        // NOTE: This will always be called after hot reloading the game dll, for now, for quick test I'm calling it every frame,
        public static void PopulateAllComponentTypes()
        {
            void AddComponentsFromAssembly(Assembly assembly)
            {
                Type[] types;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types;
                }

                if (types == null)
                    return;

                for (int j = 0; j < types.Length; j++)
                {
                    Type t = types[j];
                    if (t == null)
                        continue;

                    if (!t.IsClass)
                        continue;

                    if (t.IsAbstract)
                        continue;

                    if (!t.IsAssignableTo(typeof(Component)))
                        continue;

                    _componentTypes.Add(t);
                }
            }

            _componentTypes.Clear();

            AddComponentsFromAssembly(LaminarTypeRegistryEditor.EngineAssembly);

            for (int i = 0; i < LaminarTypeRegistryEditor.GameAppComponentTypes.Count; i++)
            {
                var componentType = LaminarTypeRegistryEditor.GameAppComponentTypes[i];

                _componentTypes.Add(componentType);
            }

            _componentTypes.Sort(static (a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
        }

        private static object _componentClipboard;

        public static void DrawComponentTree(Component component, int componentIndex, Action<Component> drawPropertiesDefault)
        {
            string componentID = component.GetID().ToString();
            string baseID = $"__COMPONENT_{componentID}";

            var flags = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.AllowOverlap |
                ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.NoTreePushOnOpen;
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
            bool componentHeader = ImGui.TreeNodeEx($"##{baseID}", flags);
            ImGui.PopStyleVar();
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
            ImGui.SetCursorPosY(cursorY + 2);
            if (EditorGuiFieldsResolver.DrawBoolField($"##__COMPONENT__ENABLED_{baseID}", ref enabled))
            {
                component.IsEnabled = enabled;
                EditorSystem.Save.MarkDirty(component);
            }
            ImGui.SetCursorPosX(cursorX + 12);
            ImGui.SameLine();

            // Component icon
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 50);
            var imageSize = new Vector2(19, 19);
            ImGui.SetCursorPosY(cursorY + 3);

            var imagePtr = EditorTextureDatabase.GetIconImGui(component.GetType());

            if (imagePtr == 0)
            {
                imagePtr = EditorTextureDatabase.GetIconImGui(EditorIcon.ScriptFile);
            }

            EditorImGui.Image(imagePtr, new vec2(imageSize.X, imageSize.Y));


            // Component name
            ImGui.SameLine();
            ImGui.SetCursorPosX(cursorX + 40);
            ImGui.SetCursorPosY(cursorY);
            ImGui.Text(component.GetType().Name);

            // Context menu
            if (EditorImGui.BeginPopup($"##POPUP_{baseID}"))
            {
                if (ImGui.MenuItem("Reset"))
                {
                    // Reset component logic
                    if (component is IDisposable disposable) disposable.Dispose();

                    EditorSystem.Save.MarkDirty(component);
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
                    EditorSystem.Save.MarkDirty(component);
                }
                ImGui.EndDisabled();
                ImGui.BeginDisabled(component is Transform);

                var index = component.Actor.Components.IndexOf(component);
                ImGui.BeginDisabled(index <= 1);
                if (ImGui.MenuItem("Move up"))
                {
                    // TODO: check if actor requires the one that is on top, if so, then do not move to the top.
                    component.Actor.Components.RemoveAt(index);
                    component.Actor.Components.Insert(index - 1, component);
                }
                ImGui.EndDisabled();
                ImGui.BeginDisabled(index >= component.Actor.Components.Count - 1);
                if (ImGui.MenuItem("Move down"))
                {
                    component.Actor.Components.RemoveAt(index);
                    component.Actor.Components.Insert(index + 1, component);
                }
                ImGui.EndDisabled();
                ImGui.EndDisabled();
                bool shouldDisableRemove = componentIndex <= 0;
                ImGui.BeginDisabled(shouldDisableRemove);
                ImGui.Separator();
                if (ImGui.MenuItem("Remove"))
                {
                    var actor = component.Actor;
                    Actor.Destroy(component);
                    EditorSystem.Save.MarkDirty(actor);

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
                DrawID("Component", component);

                if (CustomEditorDatabase.TryGetCustomComponentDrawer(component.GetType(), out var drawer))
                {
                    drawer.Draw(component, () => drawPropertiesDefault?.Invoke(component));
                }
                else
                {
                    drawPropertiesDefault?.Invoke(component);
                }
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
        internal override void OnClose()
        {
        }
    }
}
