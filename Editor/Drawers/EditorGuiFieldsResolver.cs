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
using Engine.Layers;

namespace Editor.Utils
{
    public class EditorGuiFieldsResolver
    {
        public const float XPosOffset = 180;
        private static bool _openPopup;

        private static object _selectedValue;
        private static Func<object, bool> _selectedSetter;

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
        public static bool DrawScalarField<T>(string name, ref T value, ImGuiDataType type, float itemWidth = 0, bool pressEnterToConfirm = false) where T : unmanaged
        {
            SetNextItemWidth(itemWidth);

            unsafe
            {
                T v = value;
                nint ptr = (nint)(&v);
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
            Vector2 buttonSize = new Vector2(ImGui.GetContentRegionAvail().X - 5, 22f);

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
        public static bool DrawEnum(string name, Type type, ref object value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(Math.Max(XPosOffset, ImGui.GetCursorPosX()));

            int idx = (int)value;
            string[] names = Enum.GetNames(type);
            if (DrawCombo(name, ref idx, names, itemWidth))
            {
                value = Enum.Parse(type, names[idx]);

                return true;
            }

            return false;
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
                string id = $"##_mat_{name}mat2_{i}";

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
                string id = $"##_mat_{name}mat3_{i}";

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
                string id = $"##_mat_{name}mat4_{i}";

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

        private static T CastSafe<T>(object obj)
        {
            if (obj == null)
            {
                return default;
            }

            return (T)obj;
        }
        public static bool DrawField(Type type, string name, ref object value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            if (type.IsAssignableTo(typeof(IObject)))
            {
                var eObject = value as IObject;
                var eObjectType = eObject != null ? eObject.GetType() : type;

                object vOut = null;
                DrawEObjectSlot(eObject, eObjectType, x =>
                {
                    vOut = x;
                    return true;
                });
                value = vOut;

                return true;
            }
            else if (type == typeof(string))
            {
                string refValue = CastSafe<string>(value);
                bool result = false;

                if (result = DrawStringField(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(bool))
            {
                bool refValue = CastSafe<bool>(value);
                bool result = false;

                if (result = DrawBoolField(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type.IsEnum)
            {
                object refValue = value;
                bool result = false;

                if (result = DrawEnum(name, type, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(uint))
            {
                uint refValue = CastSafe<uint>(value);

                bool result = false;

                if (result = DrawUIntField(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(int))
            {
                int refValue = CastSafe<int>(value);

                bool result = false;

                if (result = DrawIntField(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(long))
            {
                long refValue = CastSafe<long>(value);

                bool result = false;

                if (result = DrawLongField(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(ulong))
            {
                ulong refValue = CastSafe<ulong>(value);

                bool result = false;

                if (result = DrawULongField(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(float))
            {
                float refValue = CastSafe<float>(value);

                bool result = false;

                if (result = DrawFloatField(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(double))
            {
                double refValue = CastSafe<double>(value);

                bool result = false;

                if (result = DrawDoubleField(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(Color))
            {
                Color refValue = CastSafe<Color>(value);

                bool result = false;

                if (result = DrawColorField(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(Color32))
            {
                Color refValue = CastSafe<Color32>(value);

                bool result = false;

                if (result = DrawColorField(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = (Color32)refValue;
                }
                return result;
            }
            else if (type == typeof(vec2))
            {
                vec2 refValue = CastSafe<vec2>(value);

                bool result = false;

                if (result = DrawVec2Field(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(vec3))
            {
                vec3 refValue = CastSafe<vec3>(value);

                bool result = false;

                if (result = DrawVec3Field(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(vec4))
            {
                vec4 refValue = CastSafe<vec4>(value);

                bool result = false;

                if (result = DrawVec4Field(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(quat))
            {
                vec4 refValue = (vec4)CastSafe<quat>(value);

                bool result = false;

                if (result = DrawVec4Field(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = new quat(refValue.x, refValue.y, refValue.z, refValue.w);
                }
                return result;
            }
            else if (type == typeof(mat2))
            {
                mat2 refValue = CastSafe<mat2>(value);

                bool result = false;

                if (result = DrawMatrix(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(mat3))
            {
                mat3 refValue = CastSafe<mat3>(value);

                bool result = false;

                if (result = DrawMatrix(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(mat4))
            {
                mat4 refValue = CastSafe<mat4>(value);

                bool result = false;

                if (result = DrawMatrix(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }

                return result;
            }
            return false;
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

        internal static bool DrawDictionaryField(string name, IDictionary dictionary,
            Func<Type, string, object, (object valueOut, bool result)> onDrawArgCallback = null, bool drawElementsAsTrees = false)
        {
            if (dictionary == null)
                return false;

            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetWindowWidth() - 120);

            bool changed = false;


            var keys = dictionary.Keys;
            var keysList = new List<object>(dictionary.Count);
            foreach (var key in dictionary.Keys)
            {
                keysList.Add(key);
            }

            ImGui.BeginDisabled(dictionary.Count - 1 < 0);
            if (ImGui.Button("-", new Vector2(22, 22)))
            {
                if (dictionary.Count - 1 >= 0)
                {
                    dictionary.Remove(keysList[^1]);
                    changed = true;
                    keysList.RemoveAt(keysList.Count - 1);
                }
            }
            ImGui.EndDisabled();
            ImGui.SameLine();
            if (ImGui.Button("+", new Vector2(22, 22)))
            {
                object GetDefaultArgValue(IDictionary dict, int argIndex)
                {
                    return ReflectionUtils.GetDefaultValueInstance(dict.GetType().GetGenericArguments()[argIndex]);
                }

                var newKey = GetDefaultArgValue(dictionary, 0);

                if (!dictionary.Contains(newKey))
                {
                    dictionary.Add(newKey, GetDefaultArgValue(dictionary, 1));
                    changed = true;
                    keysList.Add(newKey);
                }
            }

            for (int i = 0; i < keysList.Count; i++)
            {
                var key = keysList[i];
                var value = dictionary[keysList[i]];

                bool show;
                if (ImGui.Button($"X##_DELETE_BUTTON_{i}_{name}", new Vector2(22, 22)))
                {
                    dictionary.Remove(key);
                    Debug.Log("remove from dictionary");

                    return true;
                }
                ImGui.SameLine();
                var itemTitleCursorX = ImGui.GetCursorPosX();

                if (drawElementsAsTrees)
                {
                    show = ImGui.TreeNode($"key {i}_{name}");
                }
                else
                {
                    ImGui.Text($"key {i}");
                    show = true;
                }

                ImGui.SameLine();

                if (show)
                {
                    var xCursor = ImGui.GetCursorPosX();
                    var keyOut = key;
                    var valueOut = value;
                    ImGui.SetCursorPosX(itemTitleCursorX + 50);

                    var keyArgName = $"##{name}_{i}__DICT_KEY__";
                    var valueArgName = $"##{name}_{i}__DICT_VALUE__";
                    if (onDrawArgCallback != null)
                    {
                        var res = onDrawArgCallback(dictionary?.GetType().GetGenericArguments()[0], keyArgName, key);

                        if (res.result && TryRemoveKey(res.valueOut))
                        {
                            keyOut = res.valueOut;
                        }
                    }
                    else if (DrawField(dictionary?.GetType().GetGenericArguments()[0], keyArgName, ref keyOut))
                    {
                        changed = true;
                        if (!TryRemoveKey(key))
                        {
                            keyOut = key;
                        }
                    }

                    bool TryRemoveKey(object keyVal)
                    {
                        if (keyVal != key && !dictionary.Contains(keyVal))
                        {
                            dictionary.Remove(key);
                            return true;
                        }

                        return false;
                    }

                    ImGui.SetCursorPosX(itemTitleCursorX);
                    if (drawElementsAsTrees)
                    {
                        show = ImGui.TreeNode($"value {i}_{name}");
                    }
                    else
                    {
                        ImGui.Text($"value {i}");
                        ImGui.SameLine();
                        show = true;
                    }
                    ImGui.SetCursorPosX(itemTitleCursorX + 50);

                    if (onDrawArgCallback != null)
                    {
                        var res = onDrawArgCallback(dictionary?.GetType().GetGenericArguments()[1], valueArgName, value);

                        if (res.result)
                        {
                            valueOut = res.valueOut;
                        }
                    }
                    else if (DrawField(dictionary?.GetType().GetGenericArguments()[1], valueArgName, ref valueOut))
                    {
                        changed = true;
                        dictionary[keyOut] = valueOut;
                    }

                    dictionary[keyOut] = valueOut;

                    if (drawElementsAsTrees)
                        ImGui.TreePop();
                }

                i++;
            }

            return changed;
        }


        public static void DrawEObjectSlot(IObject eObject, Type valueType, Func<object, bool> setValue)
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(MathF.Max(XPosOffset, ImGui.GetCursorPosX()) + 5);

            string label = eObject != null ? $"{eObject.Name}" : $"Null";

            if (eObject != null)
            {
                if (eObject is AssetResourceBase res)
                {
                    ImGui.SetItemTooltip($"{res.Path}");
                }
                else
                {
                    ImGui.SetItemTooltip(eObject.GetID().ToString());
                }
            }

            var drawList = ImGui.GetWindowDrawList();
            var pos = ImGui.GetCursorScreenPos();
            var size = ImGui.CalcTextSize(label);

            var min = new Vector2(pos.X - 5, pos.Y);
            var max = new Vector2(pos.X + ImGui.GetContentRegionAvail().X - 5, pos.Y + size.Y + 2);

            drawList.AddRectFilled(min, max, ImGui.ColorConvertFloat4ToU32(new(0.1f, 0.1f, 0.1f, 1f)));

            string suffix = $"({valueType.Name})";
            float suffixWidth = ImGui.CalcTextSize(suffix).X;

            const float offset = 6;
            var length = (max.X - min.X) - offset;
            float availableLabelWidth = length - suffixWidth;
            if (availableLabelWidth < 0)
                availableLabelWidth = 0;

            string displayLabel = label;

            float labelWidth = ImGui.CalcTextSize(label).X;
            if (labelWidth > availableLabelWidth)
            {
                const string ellipsis = "...";
                float ellipsisWidth = ImGui.CalcTextSize(ellipsis).X;

                int count = 0;
                float wwidth = 0f;

                foreach (char c in label)
                {
                    float w = ImGui.CalcTextSize(c.ToString()).X;
                    if (wwidth + w + ellipsisWidth > availableLabelWidth)
                        break;

                    wwidth += w;
                    count++;
                }

                displayLabel = label.Substring(0, count) + ellipsis;

                ImGui.Text($"{displayLabel}{suffix}");
            }
            else
            {
                ImGui.Text($"{displayLabel} {suffix}");
            }


            if (ImGui.IsItemClicked())
            {
                _openPopup = true;
                _selectedValue = eObject;
                _selectedSetter = setValue;
            }

            PickObjectPopup(valueType, setValue);
        }

        private static void PickObjectPopup(Type valueType, Func<object, bool> setValue)
        {
            if (_openPopup)
            {
                _openPopup = false;
                ImGui.CloseCurrentPopup();
                ImGui.OpenPopup("ObjectPickPopup");
            }

            if (!ImGui.BeginPopup("ObjectPickPopup"))
                return;

            if (ImGui.Selectable("None"))
            {
                setValue(null);
                ImGui.CloseCurrentPopup();
                ImGui.EndPopup();
                return;
            }

            if (typeof(AssetResourceBase).IsAssignableFrom(valueType))
            {
                // Asset picking
                if (valueType == typeof(Material))
                {
                    //foreach (var guid in Assets.GetGuids(AssetType.Material))
                    //{
                    //    var path = Assets.ResolvePath(guid);
                    //    if (ImGui.Selectable($"{System.IO.Path.GetFileName(path)}##{guid}"))
                    //    {
                    //        setValue(Assets.GetMaterial(path));
                    //        ImGui.CloseCurrentPopup();
                    //    }
                    //}
                }
                else if (valueType == typeof(Texture))
                {
                    //foreach (var guid in Assets.GetGuids(AssetType.Texture))
                    //{
                    //    var path = Assets.ResolvePath(guid);
                    //    if (ImGui.Selectable($"{System.IO.Path.GetFileName(path)}##{guid}"))
                    //    {
                    //        setValue(Assets.GetTexture(path));
                    //        ImGui.CloseCurrentPopup();
                    //    }
                    //}
                }
                else if (valueType == typeof(AudioClip))
                {
                    var audios = IOLayer.Database.Disk.GetAssetsInfo(SharedTypes.AssetType.Audio);

                    foreach (var asset in audios)
                    {
                        if (ImGui.Selectable($"{Path.GetFileName(asset.Value.Path)}##{asset.Key}"))
                        {
                            setValue(Assets.GetAudioClip(asset.Value.Path));
                            ImGui.CloseCurrentPopup();
                        }
                    }
                }
            }
            else
            {
                foreach (var scene in SceneManager.Scenes)
                {
                    var root = scene.RootActors;
                    for (int i = 0; i < root.Count; i++)
                    {
                        DrawSceneObjectPropertyPicker(root[i].Transform, valueType, setValue);
                    }
                }
            }
            ImGui.EndPopup();
        }
        private static void DrawSceneObjectPropertyPicker(Transform root, Type targetType, Func<object, bool> setValue)
        {
            if (typeof(Actor).IsAssignableFrom(targetType))
            {
                if (ImGui.Selectable($"{root.Name}##{root.GetID()}"))
                {
                    setValue(root.Actor);
                    ImGui.CloseCurrentPopup();
                }
            }
            else if (typeof(IComponent).IsAssignableFrom(targetType))
            {
                // TODO: this is slow, it should be cached.
                var components = root.Actor.Components.Where(x => x.GetType().IsAssignableTo(targetType)).ToArray();

                if (components.Length > 0 && ImGui.Selectable($"{root.Name}##{root.GetID()}"))
                {
                    foreach (var comp in components)
                    {
                        if (targetType.IsAssignableFrom(comp.GetType()))
                        {
                            if (setValue(comp))
                                break;
                        }
                    }
                    ImGui.CloseCurrentPopup();
                }
            }
            //else if (typeof(EObject).IsAssignableFrom(targetType))
            //{
            //    if (ImGui.Selectable($"{root.Name}##{root.GetID()}"))
            //    {
            //        setValue(root.Actor);
            //        ImGui.CloseCurrentPopup();
            //    }
            //}
            for (int i = 0; i < root.Children.Count; i++)
            {
                DrawSceneObjectPropertyPicker(root.Children[i], targetType, setValue);
            }
        }
    }
}
