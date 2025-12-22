using System;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ImGuiNET;
using GlmNet;
using Engine.Utils;

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

        public static bool DrawStringField(string name, ref string value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            int bufferSize = Math.Max(257, value?.Length + 1 ?? 257);
            byte[] buffer = new byte[bufferSize];

            if (!string.IsNullOrEmpty(value))
                System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, 0);

            if (ImGui.CalcTextSize(name).X + 10 > ImGui.GetContentRegionAvail().X)
                SetNextItemWidth(itemWidth);

            bool changed;
            unsafe
            {
                ImGui.SameLine();
                changed = ImGui.InputText("##" + name, buffer, (uint)buffer.Length);
            }

            if (changed && (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter)))
            {
                value = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                return true;
            }

            return false;
        }

        public static bool DrawFloatField(string name, ref float value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, 0f);
            ImGui.SameLine();

            bool changed = ImGui.DragFloat($"##{name}", ref value, 0.1f, 0, 0, "%.4f") &&
                           (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter));

            ImGui.PopStyleVar();
            return changed;
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

            ImGui.SameLine();
            bool result = ImGui.Checkbox($"##{name}", ref value);

           // ImGui.PopStyleColor(4);
            style.FramePadding = padding;
            style.FrameBorderSize = border;

            return result;
        }

        public static bool DrawIntField(string name, ref int value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            ImGui.SameLine();

            return ImGui.InputInt($"##{name}", ref value) &&
                   (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter));
        }

        public static bool DrawVec2Field(string name, ref vec2 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            Vector2 ve = value.ToVector2();
            ImGui.SameLine();

            return ImGui.DragFloat2($"##{name}", ref ve, 0.1f, 0, 0, "%.4f") &&
                   (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter));
        }

        public static bool DrawVec3Field(string name, ref vec3 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            Vector3 ve = new Vector3(value.x, value.y, value.z);
            ImGui.SameLine();

            return ImGui.DragFloat3($"##{name}", ref ve, 0.1f, 0, 0, "%.4f") &&
                   (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter));
        }

        public static bool DrawVec4Field(string name, ref vec4 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            Vector4 ve = new Vector4(value.x, value.y, value.z, value.w);
            ImGui.SameLine();

            return ImGui.InputFloat4($"##{name}", ref ve, "%.4f") &&
                   (!pressEnterToConfirm || ImGui.IsKeyDown(ImGuiKey.Enter));
        }

        public static bool DrawCombo(string name, ref int index, string[] values, float itemWidth = 0)
        {
            SetNextItemWidth(itemWidth);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.2f, 0.2f, 0.21568628f, 1));
            ImGui.SameLine();

            bool result = ImGui.Combo(name, ref index, values, values.Length);

            ImGui.PopStyleColor();
            return result;
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
                    for (int i = size; i < val; i++)
                        onAddCallback();
                else if (size > val)
                    onRemoveCallback(val);

                size = val;
                changed = true;
            }

            for (int i = 0; i < size; i++)
            {
                float prevCursorY = ImGui.GetCursorPosY();
                bool show;

                if (itemAsTree)
                    show = ImGui.TreeNode($"item {i}");
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
    }

}
