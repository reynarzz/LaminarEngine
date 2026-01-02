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
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - width);
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
         
        public static bool DrawDoubleField(string name, ref double value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            return DrawScalarField(name, ref value, ImGuiDataType.Double, 0, false);
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
        public static bool DrawIntField(string name, ref int value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            return DrawScalarField(name, ref value, ImGuiDataType.S32, itemWidth, pressEnterToConfirm);
        }
        public static bool DrawUIntField(string name, ref uint value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            return DrawScalarField(name, ref value, ImGuiDataType.U32, itemWidth, pressEnterToConfirm);
        }
        public static bool DrawLongField(string name, ref long value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            return DrawScalarField(name, ref value, ImGuiDataType.S64, itemWidth, pressEnterToConfirm);
        }
        public static bool DrawULongField(string name, ref ulong value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            return DrawScalarField(name, ref value, ImGuiDataType.U64, itemWidth, pressEnterToConfirm);
        }
        public static bool DrawScalarField<T>(string name, ref T value, ImGuiDataType type, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);

            unsafe
            {
                T v = value;
                nint ptr = (nint)(&v);
                var a = ImGuiKey.GetValues(typeof(ImGuiKey));

                var result = ImGui.DragScalar($"##{name}", type, ptr) && (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter));
                value = *(T*)ptr;
                return result;
            }
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
        public static bool DrawEnum<T>(string name, ref T value) where T : unmanaged, Enum 
        {
            var values = Enum.GetValues<T>();
            int idx = Array.IndexOf(values, value);
            string[] names = Enum.GetNames<T>();
            var changed = DrawCombo(name, ref idx, names);

            if (changed)
            {
                value = values[idx];
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

        public static bool DrawMatrix(string name, ref mat2 value, float itemWidth = 0, bool pressEnterToConfirm = false)
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

        public static bool DrawMatrix(string name, ref mat3 value, float itemWidth = 0, bool pressEnterToConfirm = false)
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

        public static bool DrawMatrix(string name, ref mat4 value, float itemWidth = 0, bool pressEnterToConfirm = false)
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

        public static bool DrawListField(string name, IList list, bool itemAsTree, Action<IList, int> onAddCallback, Action<IList, int> onRemoveCallback,
                                         Action<IList, int> removeCount, Func<int, float, object, bool> drawCallback)
        {

            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetWindowWidth() - 120);

            bool changed = false;
            var size = list.Count;

            ImGui.BeginDisabled(size - 1 < 0);
            if (ImGui.Button("-", new Vector2(22, 22)))
            {
                changed = true;

                if (size - 1 >= 0)
                {
                    onRemoveCallback(list, size - 1);
                }
            }
            ImGui.EndDisabled();
            ImGui.SameLine();
            if (ImGui.Button("+", new Vector2(22, 22)))
            {
                changed = true;
                onAddCallback(list, size + 1);
            }

            ImGui.SameLine();

            string lenText = size.ToString();
            ImGui.SetNextItemWidth(53);

            if (DrawStringField($"##_size_{name}", ref lenText, 0, true))
            {
                if (int.TryParse(lenText, out var val) && val >= 0)
                {
                    if (size < val)
                    {
                        onAddCallback(list, val);
                    }
                    else if (size > val)
                    {
                        removeCount(list, val);
                    }

                    size = val;
                    changed = true;
                }
                changed = false;
            }

            for (int i = 0; i < list.Count; i++)
            {
                float prevCursorY = ImGui.GetCursorPosY();
                bool show;
                if (ImGui.Button($"X##_DELETE_BUTTON_{i}_{name}", new Vector2(22, 22)))
                {
                    onRemoveCallback(list, i);
                    changed = true;
                    break;
                }
                ImGui.SameLine();
                if (itemAsTree)
                {
                    show = ImGui.TreeNode($"item {i}_{name}");
                }
                else
                {
                    ImGui.Text($"item {i}");
                    show = true;
                }

                ImGui.SameLine();

                if (show)
                {
                    if (drawCallback(i, 0, list[i]))
                        changed = true;

                    if (itemAsTree)
                        ImGui.TreePop();
                }
            }

            return changed;
        }
    }
}
