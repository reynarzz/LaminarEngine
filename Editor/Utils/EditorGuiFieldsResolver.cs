using System;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using ImGuiNET;
using GlmNet;
using Engine.Utils;
using System.Collections;
using Engine;

namespace Editor.Utils
{
    public class EditorGuiFieldsResolver
    {
        public EditorGuiFieldsResolver()
        {

        }

        private static void SetNextItemWidth(float width)
        {
            if (width > 0)
            {
                ImGui.SetNextItemWidth(width);
            }
            else
            {
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 5);
            }
        }
        public static bool DrawStringField(string name, ref string value)
        {
            return DrawStringField(name, ref value, 0, false);
        }
        public static bool DrawStringField(string name, ref string value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            int bufferSize = Math.Max(257, value?.Length + 1 ?? 257);
            byte[] buffer = new byte[bufferSize];

            if (!string.IsNullOrEmpty(value))
                System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, 0);

            // if (ImGui.CalcTextSize(name).X + 10 > ImGui.GetContentRegionAvail().X)
            SetNextItemWidth(itemWidth);

            bool changed;
            unsafe
            {
                changed = ImGui.InputText("##" + name, buffer, (uint)buffer.Length);
            }

            if (changed && (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter)))
            {
                value = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                return true;
            }

            return false;
        }
        public static bool DrawFloatField(string name, ref float value)
        {
            return DrawFloatField(name, ref value, 0, false);
        }
        public static bool DrawFloatField(string name, ref float value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, 0f);

            bool changed = ImGui.DragFloat($"##{name}", ref value, 0.1f, 0, 0, "%.4f") &&
                           (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter));

            ImGui.PopStyleVar();
            return changed;
        }
        public static bool DrawBoolField(string name, ref bool value)
        {
            return DrawBoolField(name, ref value, 0, false);
        }
        public static bool DrawBoolField(string name, ref bool value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);

            var style = ImGui.GetStyle();
            var padding = style.FramePadding;
            float border = style.FrameBorderSize;

            //ImGui.PushStyleColor(ImGuiCol.CheckMark, new Vector4(0.1f, 0.1f, 0.1f, 1));
            //ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.1f, 0.1f, 0.1f, 1));
            //ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new Vector4(0.1f, 0.1f, 0.1f, 1));
            //ImGui.PushStyleColor(ImGuiCol.FrameBgActive, new Vector4(0.12f, 0.12f, 0.12f, 1));

            style.FrameBorderSize = 0;
            style.FramePadding = new Vector2(0.6f, 0.6f);

            bool result = ImGui.Checkbox($"##{name}", ref value);

            // ImGui.PopStyleColor(4);
            style.FramePadding = padding;
            style.FrameBorderSize = border;

            return result;
        }
        public static bool DrawIntField(string name, ref int value)
        {
            return DrawIntField(name, ref value, 0, false);
        }
        public static bool DrawIntField(string name, ref int value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);

            return ImGui.InputInt($"##{name}", ref value) &&
                   (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter));
        }

        private static bool _openColorPicker = false;
        public static bool DrawColorField(string name, ref Color value)
        {
            return DrawColorField(name, ref value, 0, false);
        }
        public static bool DrawColorField(string name, ref Color value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);

            var col = new Vector4(value.R, value.G, value.B, value.A);

            // Determine button size
            Vector2 buttonSize = new Vector2(ImGui.GetContentRegionAvail().X - 5, 20f);

            bool changed = false;

            // Draw the color button
            if (ImGui.ColorButton(name, col, ImGuiColorEditFlags.NoTooltip | ImGuiColorEditFlags.NoDragDrop, buttonSize))
            {
                _openColorPicker = true; // open popup on click
            }

            // Open popup if requested
            if (_openColorPicker)
            {
                ImGui.OpenPopup("Color Picker");
                _openColorPicker = false; // reset flag
            }

            if (ImGui.BeginPopup("Color Picker"))
            {
                if (ImGui.ColorPicker4("##picker", ref col, ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.NoSmallPreview))
                {
                    value = new Color(col.X, col.Y, col.Z, col.W);
                    changed = true;
                }
                ImGui.EndPopup();
            }

            return changed;
        }

        public static bool DrawVec2Field(string name, ref vec2 value)
        {
            return DrawVec2Field(name, ref value, 0, false);
        }
        public static bool DrawVec2Field(string name, ref vec2 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            Vector2 ve = value.ToVector2();

            var changed = ImGui.DragFloat2($"##{name}", ref ve, 0.1f, 0, 0, "%.4f") && (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter));

            if (changed)
            {
                value.x = ve.X;
                value.y = ve.Y;
            }

            return changed;
        }
        public static bool DrawVec3Field(string name, ref vec3 value)
        {
            return DrawVec3Field(name, ref value, 0, false);
        }
        public static bool DrawVec3Field(string name, ref vec3 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            Vector3 ve = new Vector3(value.x, value.y, value.z);

            var changed = ImGui.DragFloat3($"##{name}", ref ve, 0.1f, 0, 0, "%.4f") && (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter));

            if (changed)
            {
                value.x = ve.X;
                value.y = ve.Y;
                value.z = ve.Z;
            }

            return changed;
        }
        public static bool DrawVec4Field(string name, ref vec4 value)
        {
            return DrawVec4Field(name, ref value, 0, false);
        }
        public static bool DrawVec4Field(string name, ref vec4 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            Vector4 ve = new Vector4(value.x, value.y, value.z, value.w);

            var changed = ImGui.DragFloat4($"##{name}", ref ve, 0.1f, 0, 0, "%.4f") && (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter));

            if (changed)
            {
                value.x = ve.X;
                value.y = ve.Y;
                value.z = ve.Z;
                value.w = ve.W;
            }

            return changed;
        }

        public static bool DrawCombo(string name, ref int index, string[] values, float itemWidth = 0)
        {
            SetNextItemWidth(itemWidth);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.2f, 0.2f, 0.21568628f, 1));
            bool result = ImGui.Combo($"##{name}", ref index, values, values.Length);

            ImGui.PopStyleColor();
            return result;
        }
        public static bool DrawMatrix(string name, ref mat2 value)
        {
            return DrawMatrix(name, ref value, 0);
        }
        public static bool DrawMatrix(string name, ref mat3 value)
        {
            return DrawMatrix(name, ref value, 0);
        }
        public static bool DrawMatrix(string name, ref mat4 value)
        {
            return DrawMatrix(name, ref value, 0);
        }
        public static bool DrawMatrix(string name, ref mat2 value, int itemWidth)
        {
            float xpos = ImGui.GetCursorPosX();
            bool changed = false;
            var cursorPosX = ImGui.GetCursorPosX();

            for (int i = 0; i < 2; i++)
            {
                ImGui.SetCursorPosX(cursorPosX);

                SetNextItemWidth(itemWidth);
                var row = value[i];
                string id = $"##_mat_{name}{typeof(mat2).Name}_{i}";

                Vector2 v = row.ToVector2(); 
                if (ImGui.DragFloat2($"##{id}", ref v, 0.1f, 0, 0, "%.4f"))
                {
                    row.x = v.X;
                    row.y = v.Y;

                    value[i] = row;
                    changed = true;
                }
            }

            return changed;
        }
       
        public static bool DrawMatrix(string name, ref mat3 value, int itemWidth)
        {
            bool changed = false;
            float cursorX = ImGui.GetCursorPosX();

            for (int i = 0; i < 3; i++)
            {
                ImGui.SetCursorPosX(cursorX);
                SetNextItemWidth(itemWidth);

                var row = value[i];
                Vector3 v = row.ToVector3();
                string id = $"##_mat_{name}{typeof(mat3).Name}_{i}";

                if (ImGui.DragFloat3($"##{id}", ref v, 0.1f, 0, 0, "%.4f"))
                {
                    row.x = v.X;
                    row.y = v.Y;
                    row.z = v.Z;
                    value[i] = row;
                    changed = true;
                }
            }

            return changed;
        }

        public static bool DrawMatrix(string name, ref mat4 value, int itemWidth)
        {
            bool changed = false;
            float cursorX = ImGui.GetCursorPosX();

            for (int i = 0; i < 4; i++)
            {
                ImGui.SetCursorPosX(cursorX);
                SetNextItemWidth(itemWidth);

                var row = value[i];
                Vector4 v = row.ToVector4();
                string id = $"##_mat_{name}{typeof(mat4).Name}_{i}";

                if (ImGui.DragFloat4($"##{id}", ref v, 0.1f, 0, 0, "%.4f"))
                {
                    row.x = v.X;
                    row.y = v.Y;
                    row.z = v.Z;
                    row.w = v.W;
                    value[i] = row;
                    changed = true;
                }
            }

            return changed;
        }
        public static bool DrawListField(string name, IList value)
        {
            var listType = value.GetType();
            var itemType = default(Type);

            if (listType.IsGenericType)
            {
                Type genericDef = listType.GetGenericTypeDefinition();
                if (typeof(IList<>).IsAssignableFrom(genericDef) || genericDef == typeof(List<>))
                {
                    itemType = listType.GetGenericArguments()[0];
                }
            }

            bool DrawItem(int index, float width)
            {
                object item = null;
                try
                {
                    // sometimes the list can be null.
                    item = value[index];
                }
                catch (Exception e)
                {
                    return false;
                }

                bool changed = false;

                string fieldId = $"##_{name}{index}";

                if (itemType == typeof(string))
                {
                    string v = (string)item!;
                    changed = DrawStringField(fieldId, ref v, width);
                    item = v;
                }
                if (itemType.IsEnum)
                {
                    Array values = Enum.GetValues(itemType);
                    int idx = Array.IndexOf(values, item);

                    string[] names = Enum.GetNames(itemType);

                    if (DrawCombo(fieldId, ref idx, names))
                    {
                        item = Enum.ToObject(itemType, values.GetValue(idx)!);
                    }
                }
                else if (itemType == typeof(float))
                {
                    float v = (float)item!;
                    changed = DrawFloatField(fieldId, ref v, width);
                    item = v;
                }
                else if (itemType == typeof(int))
                {
                    int v = (int)item!;
                    changed = DrawIntField(fieldId, ref v, width);
                    item = v;
                }
                else if (itemType == typeof(vec2))
                {
                    vec2 v = (vec2)item!;
                    changed = DrawVec2Field(fieldId, ref v, width);
                    item = v;
                }
                else if (itemType == typeof(vec3))
                {
                    vec3 v = (vec3)item!;
                    changed = DrawVec3Field(fieldId, ref v, width);
                    item = v;
                }
                else if (itemType == typeof(vec4))
                {
                    vec4 v = (vec4)item!;
                    changed = DrawVec4Field(fieldId, ref v, width);
                    item = v;
                }
                else if (item.GetType() == typeof(mat2))
                {
                    var v = (mat2)item;
                    changed = DrawMatrix(fieldId, ref v);
                    item = v;
                }
                //else if (item.GetType() == typeof(mat3))
                //{
                //    var v = (mat3)(object)item!;
                //    changed = DrawMatrix(fieldId, ref v);
                //    item = v;
                //}
                //else if (item.GetType() == typeof(mat4))
                //{
                //    var v = (mat4)(object)item!;
                //    changed = DrawMatrix(fieldId, ref v);
                //    item = v;
                //}

                value[index] = item;
                return changed;
            }

            void OnAdd()
            {
                value.Add(GetDefault(itemType));
            }

            void OnRemove(int index)
            {
                if (value.Count > 0)
                {
                    value.RemoveAt(index);
                }
            }

            return DrawListField(name, value.Count, OnAdd, OnRemove, DrawItem, false);
        }

        public static bool DrawListField(string name, int size, Action onAddCallback, Action<int> onRemoveCallback, Func<int, float, bool> drawCallback, bool itemAsTree)
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetWindowWidth() - 120);

            bool changed = false;

            if (ImGui.Button("-"))
            {
                changed = true;
                onRemoveCallback(size - 1);
            }

            ImGui.SameLine();

            if (ImGui.Button("+"))
            {
                changed = true;
                onAddCallback();
            }

            ImGui.SameLine();

            string lenText = size.ToString();
            ImGui.SetNextItemWidth(53);

            if (DrawStringField($"##_size_{name}", ref lenText))
            {
                int val = Math.Max(0, int.Parse(lenText));

                if (size < val)
                {
                    for (int i = size; i < val; i++)
                    {
                        onAddCallback();
                    }
                }
                else if (size > val)
                {
                    onRemoveCallback(val);
                }

                size = val;
                changed = true;
            }

            for (int i = 0; i < size; i++)
            {
                float prevCursorY = ImGui.GetCursorPosY();
                bool show;

                if (itemAsTree)
                {
                    show = ImGui.TreeNode($"item {i}");
                }
                else
                {
                    ImGui.Text($"item {i}");
                    show = true;
                }

                ImGui.SameLine();

                if (show)
                {
                    if (drawCallback(i, ImGui.GetContentRegionAvail().X - 24f))
                        changed = true;

                    if (itemAsTree)
                        ImGui.TreePop();
                }

                ImGui.SameLine();
                ImGui.SetCursorPosY(prevCursorY);
                ImGui.SetCursorPosX(ImGui.GetWindowSize().X - 30);

                if (ImGui.Button($"X##_{i}"))
                {
                    onRemoveCallback(i);
                    changed = true;
                }
            }

            return changed;
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                if (type.IsEnum)
                {
                    Array values = Enum.GetValues(type);
                    return values.Length > 0 ? values.GetValue(0)! : Activator.CreateInstance(type)!;
                }
                else
                {
                    return Activator.CreateInstance(type)!;
                }
            }
            else
            {
                if (type == typeof(string))
                {
                    return string.Empty;
                }

                return null!;
            }
        }
    }
}
