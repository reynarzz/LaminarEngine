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
        private static readonly Vector2 VECTOR_INNER_SPACING = new Vector2(3, 2);
        private static bool _openPopup;

        private static object _selectedValue;
        private static Func<object, bool> _selectedSetter;
        public delegate (object valueOut, bool result) onDrawDictionaryArgCallback(Type argKey, string argName, object argValue);

        public EditorGuiFieldsResolver()
        {
        }

        internal static void SetPropertyDefaultCursorPos()
        {
            ImGui.SetCursorPosX(Math.Max(XPosOffset, ImGui.GetCursorPosX()));
            SetNextItemWidth(0);
        }

        private static void SetNextItemWidth(float width, bool trueWidth = false)
        {
            if (trueWidth)
            {
                ImGui.SetNextItemWidth(width);
                return;
            }
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
            SetNextItemWidth(itemWidth);

            var refStr = value;
            var changed = ImGui.InputText("##" + name, ref refStr, (uint)bufferSize);

            if (changed)
            {
                value = refStr;
            }

            return changed;
        }

        public static bool DrawDoubleField(string name, ref double value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            return DrawScalarField(name, ref value, ImGuiDataType.Double, 0, false);
        }
        // TODO: reuse the function below
        public static bool DrawFloatFieldRealWidth(string name, ref float value, float itemWidth, float min, float max, int decimalPlaces = 2)
        {
            SetNextItemWidth(itemWidth, true);
            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, 0f);

            bool changed = ImGui.DragFloat($"##{name}", ref value, 0.1f, min, max, $"%.{Mathf.Min(decimalPlaces, Mathf.GetDecimalPlaces(value))}f");

            ImGui.PopStyleVar();
            return changed;
        }
        public static bool DrawFloatField(string name, ref float value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, 0f);

            bool changed = ImGui.DragFloat($"##{name}", ref value, 0.1f, 0, 0, $"%.{Math.Max(1, Mathf.GetDecimalPlaces(value))}f") &&
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
                var result = ImGui.DragScalar($"##{name}", type, ptr, 0.2f);
                value = *(T*)ptr;
                return result;
            }
        }

        private static bool _openColorPicker = false;

        public static bool DrawColor32Field(string name, ref Color32 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            Color col = (Color)value;
            var result = DrawColorField(name, ref col, itemWidth, pressEnterToConfirm);

            if (result)
            {
                value = (Color32)col;
            }

            return result;
        }
        public static bool DrawColorField(string name, ref Color value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);

            var col = new Vector4(value.R, value.G, value.B, value.A);

            var buttonSize = new Vector2(ImGui.GetContentRegionAvail().X - 5, 24f);

            bool changed = false;

            if (ImGui.ColorButton(name, col, ImGuiColorEditFlags.NoTooltip | ImGuiColorEditFlags.NoDragDrop, buttonSize))
            {
                _openColorPicker = true;
            }
            
            var drawList = ImGui.GetWindowDrawList();

            var barSizeOffset = new Vector2(1,0);
            var barPositionOffset = new Vector2(0,-3);

            var itemMin = ImGui.GetItemRectMin() + barSizeOffset + barPositionOffset;
            var itemMax = ImGui.GetItemRectMax() - barSizeOffset + barPositionOffset;


            var barMin = new Vector2(itemMin.X, itemMax.Y);
            var barMax = new Vector2(Mathf.Lerp(itemMin.X, itemMax.X, value.A), itemMax.Y + 2.0f);

            drawList.AddRectFilled(barMin, new Vector2(itemMax.X, itemMax.Y + 2.0f), ImGui.ColorConvertFloat4ToU32(new Vector4(0.1f, 0.1f, 0.1f, 1f)));
            drawList.AddRectFilled(barMin, barMax, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f)));
            if (_openColorPicker)
            {
                
                Vector2 popupPos = new Vector2(itemMin.X - 20, itemMin.Y + 26);
                ImGui.SetNextWindowViewport(ImGui.GetWindowViewport().ID);
                ImGui.SetNextWindowPos(popupPos, ImGuiCond.Appearing);
                ImGui.OpenPopup("Color Picker");
                _openColorPicker = false;
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
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, VECTOR_INNER_SPACING);

            var changed = ImGui.DragFloat2($"##{name}", ref ve, 0.1f, 0, 0, "%.4f");
            ImGui.PopStyleVar();
            if (changed)
            {
                value.x = ve.X;
                value.y = ve.Y;
            }

            return changed;
        }
        public static bool DrawVec2Field(string name, ref ivec2 value)
        {
            return DrawIVec2Field(name, ref value, 0, false);
        }
        public static bool DrawIVec2FieldTrueWidth(string name, ref ivec2 value, float itemWidth)
        {
            SetNextItemWidth(itemWidth, true);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, VECTOR_INNER_SPACING);

            var result = ImGui.DragInt2($"##{name}", ref value.x, 0.2f);
            ImGui.PopStyleVar();
            return result;
        }
        public static bool DrawIVec2Field(string name, ref ivec2 value, float itemWidth)
        {
            SetNextItemWidth(itemWidth);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, VECTOR_INNER_SPACING);

            var result = ImGui.DragInt2($"##{name}", ref value.x, 0.2f);
            ImGui.PopStyleVar();

            return result;
        }

        public static bool DrawIVec2Field(string name, ref ivec2 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, VECTOR_INNER_SPACING);
            var result = ImGui.DragInt2($"##{name}", ref value.x, 0.2f);
            ImGui.PopStyleVar();

            return result;
        }
        public static bool DrawIVec3Field(string name, ref ivec3 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, VECTOR_INNER_SPACING);
            var result = ImGui.DragInt3($"##{name}", ref value.x, 0.2f);
            ImGui.PopStyleVar();
            return result;
        }
        public static bool DrawIVec4Field(string name, ref ivec4 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, VECTOR_INNER_SPACING);
            var result = ImGui.DragInt4($"##{name}", ref value.x, 0.2f);
            ImGui.PopStyleVar();
            return result;
        }
        public static bool DrawVec3Field(string name, ref vec3 value)
        {
            return DrawVec3Field(name, ref value, 0, false);
        }
        public static bool DrawVec3Field(string name, ref vec3 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            Vector3 ve = new Vector3(value.x, value.y, value.z);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, VECTOR_INNER_SPACING);
            var changed = ImGui.DragFloat3($"##{name}", ref ve, 0.1f, 0, 0, "%.4f");
            ImGui.PopStyleVar();
            if (changed)
            {
                value.x = ve.X;
                value.y = ve.Y;
                value.z = ve.Z;
            }

            return changed;
        }

        public static bool DrawQuatField(string name, ref quat value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            var vecValue = value.ToVec4();

            var result = DrawVec4Field(name, ref vecValue, itemWidth, pressEnterToConfirm);

            if (result)
            {
                value = new quat(vecValue.x, vecValue.y, vecValue.z, vecValue.w);
            }

            return result;
        }
        public static bool DrawVec4Field(string name, ref vec4 value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            SetNextItemWidth(itemWidth);
            Vector4 ve = new Vector4(value.x, value.y, value.z, value.w);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, VECTOR_INNER_SPACING);

            var changed = ImGui.DragFloat4($"##{name}", ref ve, 0.1f, 0, 0, "%.4f");
            ImGui.PopStyleVar();
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

        public static bool DrawCombo(string name, ref int index, IReadOnlyList<string> values, float itemWidth = 0)
        {
            SetNextItemWidth(itemWidth);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.2f, 0.2f, 0.21568628f, 1));
            bool result = ImGui.Combo($"##{name}", ref index, values, values.Count);

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
                ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, VECTOR_INNER_SPACING);

                if (ImGui.DragFloat2($"##{id}", ref v, 0.1f, 0, 0, "%.4f"))
                {
                    row.x = v.X;
                    row.y = v.Y;

                    value[i] = row;
                    changed = true;
                }
                ImGui.PopStyleVar();
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
                ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, VECTOR_INNER_SPACING);

                if (ImGui.DragFloat3($"##{id}", ref v, 0.1f, 0, 0, "%.4f"))
                {
                    row.x = v.X;
                    row.y = v.Y;
                    row.z = v.Z;
                    value[i] = row;
                    changed = true;
                }
                ImGui.PopStyleVar();
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
                ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, VECTOR_INNER_SPACING);

                if (ImGui.DragFloat4($"##{id}", ref v, 0.1f, 0, 0, "%.4f"))
                {
                    row.x = v.X;
                    row.y = v.Y;
                    row.z = v.Z;
                    row.w = v.W;
                    value[i] = row;
                    changed = true;
                }
                ImGui.PopStyleVar();
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
            else if (type == typeof(ivec2))
            {
                ivec2 refValue = CastSafe<ivec2>(value);

                bool result = false;

                if (result = DrawIVec2Field(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(ivec3))
            {
                ivec3 refValue = CastSafe<ivec3>(value);

                bool result = false;

                if (result = DrawIVec3Field(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(ivec4))
            {
                ivec4 refValue = CastSafe<ivec4>(value);

                bool result = false;

                if (result = DrawIVec4Field(name, ref refValue, itemWidth, pressEnterToConfirm))
                {
                    value = refValue;
                }
                return result;
            }
            else if (type == typeof(quat))
            {
                quat refValue = CastSafe<quat>(value);

                bool result = false;

                if (result = DrawQuatField(name, ref refValue, itemWidth, pressEnterToConfirm))
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
            bool skip = false;

            for (int i = 0; i < list.Count; i++)
            {
                bool show;
                skip = false;
                if (ImGui.Button($"X##_DELETE_BUTTON_{i}_{name}", new Vector2(22, 22)))
                {
                    onRemoveCallback(list, i);
                    changed = true;
                    skip = true;
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
                    // if (!skip)
                    {
                        if (drawCallback(i, 0, list[i]))
                            changed = true;

                        // i++;
                    }


                    if (itemAsTree)
                        ImGui.TreePop();
                }
            }

            return changed;
        }
        internal static bool DrawDictionaryField(string name, IDictionary dictionary,
                                                 onDrawDictionaryArgCallback onDrawArgCallback = null,
                                                 bool drawElementsAsTrees = false)
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

            ImGui.SameLine();

            ImGui.SetNextItemWidth(53);

            // Display count
            ImGui.Text($"{dictionary.Count}");

            bool skip = false;
            for (int i = 0; i < keysList.Count; i++)
            {
                var key = keysList[i];
                var value = dictionary[keysList[i]];
                skip = false;

                if (ImGui.Button($"X##_DELETE_BUTTON_{i}_{name}", new Vector2(22, 22)))
                {
                    dictionary.Remove(key);
                    changed = true;
                    skip = true;
                }
                ImGui.SameLine();
                var itemTitleCursorX = ImGui.GetCursorPosX();

                bool show;
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

                    var keyType = keyOut != null ? keyOut.GetType() : dictionary?.GetType().GetGenericArguments()[0];
                    var valueType = valueOut != null ? valueOut.GetType() : dictionary?.GetType().GetGenericArguments()[1];

                    if (onDrawArgCallback != null)
                    {
                        var res = onDrawArgCallback(keyType, keyArgName, key);

                        if (res.result && TryRemoveKey(res.valueOut))
                        {
                            keyOut = res.valueOut;
                        }
                    }
                    else if (DrawField(keyType, keyArgName, ref keyOut))
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
                        var res = onDrawArgCallback(valueType, valueArgName, value);

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

                    if (!skip)
                    {
                        dictionary[keyOut] = valueOut;
                    }

                    if (drawElementsAsTrees)
                    {
                        ImGui.TreePop();
                    }
                }
            }

            return changed;
        }


        public static void DrawEObjectSlot(IObject eObject, Type valueType, Func<object, bool> setValue)
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(MathF.Max(XPosOffset, ImGui.GetCursorPosX()) + 5);
            var hasObject = eObject != null;
            string label = hasObject ? $"{eObject.Name}" : $"None";

            if (hasObject)
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
            else
            {

            }

            var drawList = ImGui.GetWindowDrawList();
            var pos = ImGui.GetCursorScreenPos();
            var size = ImGui.CalcTextSize(label);

            var min = new Vector2(pos.X - 5, pos.Y);
            var max = new Vector2(pos.X + ImGui.GetContentRegionAvail().X - 5, pos.Y + size.Y + 7);

            var preRectCursor = ImGui.GetCursorPos();

            ImGui.SetCursorPos(preRectCursor);

            drawList.AddRectFilled(min, max, ImGui.ColorConvertFloat4ToU32(new(0.1f, 0.1f, 0.1f, 1f)), ImGui.GetStyle().FrameRounding);
            if (hasObject)
            {
                nint imagePtr = 0;
                ImGui.SetCursorPos(preRectCursor + new Vector2(-2, 5));
                if (eObject is RenderTexture rendTex)
                {
                    imagePtr = EditorTextureDatabase.GetIconImGui(rendTex);
                }
                else if (eObject is Texture tex)
                {
                    imagePtr = EditorTextureDatabase.GetIconImGui(tex);
                }
                else
                {
                    imagePtr = EditorTextureDatabase.GetIconImGui(eObject.GetType());
                }
               
                ImGui.Image(imagePtr, new Vector2(16, 16), new Vector2(0, 1), new Vector2(1, 0));
                ImGui.SetCursorPos(preRectCursor + new Vector2(16, 0));
            }


            string suffix = $"({ReflectionUtils.GetFriendlyTypeName(valueType)})";
            float suffixWidth = ImGui.CalcTextSize(suffix).X;

            const float offset = 6;
            var length = (max.X - min.X) - offset;
            float availableLabelWidth = length - suffixWidth;
            if (availableLabelWidth < 0)
                availableLabelWidth = 0;

            string displayLabel = label;

            float labelWidth = ImGui.CalcTextSize(label).X;
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);

            if (hasObject)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(new(1.0f, 1.0f, 1.0f, 1f)));
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(new(0.7f, 0.7f, 0.7f, 1f)));
            }

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

            ImGui.PopStyleColor();
            if (ImGui.IsItemClicked())
            {
                _openPopup = true;
                _selectedValue = eObject;
                _selectedSetter = setValue;
            }

            ImGui.Dummy(new Vector2(0, ImGui.GetStyle().ItemSpacing.Y - 2));
            PickObjectPopup(valueType, setValue);
        }
        static int count = 0;

        private static void PickObjectPopup(Type valueType, Func<object, bool> setValue, int columnCount = 4)
        {
            if (_openPopup)
            {
                _openPopup = false;
                ImGui.CloseCurrentPopup();
                ImGui.OpenPopup("ObjectPickPopup");
            }

            //var winSize = ImGui.GetWindowSize();

            //ImGui.SetNextWindowSizeConstraints(new Vector2(400, 100), new Vector2(1400, 500));
            //var viewPortPos = ImGui.GetWindowViewport().Pos;

            //ImGui.SetNextWindowPos(new Vector2(viewPortPos.X + winSize.X / 2, viewPortPos.Y + winSize.Y / 2 - 250));
            if (!ImGui.BeginPopup("ObjectPickPopup"))
                return;

            if (ImGui.Selectable("None"))
            {
                setValue(null);
                ImGui.CloseCurrentPopup();
                ImGui.EndPopup();
                return;
            }

            void RenderItemsInColumns(IEnumerable<(string label, Action action, string path)> items)
            {
                if (!items.Any())
                    return;

                int count = 0;
                ImGui.BeginTable("PopupTable", columnCount, ImGuiTableFlags.None);

                foreach (var item in items)
                {
                    if (count % columnCount == 0)
                        ImGui.TableNextRow();

                    ImGui.TableNextColumn();
                    if (ImGui.Selectable(item.label))
                    {
                        item.action();
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.SetItemTooltip(item.path);

                    count++;
                }

                ImGui.EndTable();
            }


            // TODO: refactor the asset picker list behavior.
            if (typeof(AssetResourceBase).IsAssignableFrom(valueType))
            {
                // Asset picking
                if (valueType == typeof(Material))
                {
                    //var assets = IOLayer.Database.Disk.GetAssetsInfo(AssetType.Material);
                    //foreach (var (id, info) in assets)
                    //{
                    //    if (ImGui.Selectable($"{Path.GetFileName(info.Path)}##{id}{info.Path}"))
                    //    {
                    //        setValue(Assets.GetMaterial(info.Path));
                    //    }
                    //}

                    var assets = IOLayer.Database.Disk.GetAssetsInfo(AssetType.Material);
                    var items = assets.Select(a =>
                    {
                        var (id, info) = a;
                        string label = $"{Path.GetFileName(info.Path)}##{id}{info.Path}";
                        return (label, (Action)(() => setValue(Assets.GetMaterial(info.Path))), info.Path);
                    });
                    RenderItemsInColumns(items);
                }
                else if (valueType == typeof(AudioClip))
                {
                    //var audios = IOLayer.Database.Disk.GetAssetsInfo(SharedTypes.AssetType.Audio);

                    //foreach (var asset in audios)
                    //{
                    //    if (ImGui.Selectable($"{Path.GetFileName(asset.Value.Path)}##{asset.Key}"))
                    //    {
                    //        setValue(Assets.GetAudioClip(asset.Value.Path));
                    //        ImGui.CloseCurrentPopup();
                    //    }
                    //}

                    var assets = IOLayer.Database.Disk.GetAssetsInfo(Engine.AssetType.Audio);
                    var items = assets.Select(a =>
                    {
                        var (id, info) = a;
                        string label = $"{Path.GetFileName(info.Path)}##{id}";
                        return (label, (Action)(() => setValue(Assets.GetAssetFromGuid(id))), info.Path);
                    });
                    RenderItemsInColumns(items);
                }
                else if (valueType.IsAssignableTo(typeof(Texture)))
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

                    var assets = IOLayer.Database.Disk.GetAssetsInfo(AssetType.Texture);
                    var items = assets.Select(a =>
                    {
                        var (id, info) = a;
                        string label = $"{Path.GetFileName(info.Path)}##{id}";
                        return (label, (Action)(() => setValue(Assets.GetTexture(info.Path))), info.Path);
                    });
                    RenderItemsInColumns(items);
                }
                else if (valueType == typeof(TilemapAsset))
                {
                    var assets = IOLayer.Database.Disk.GetAssetsInfo(AssetType.Tilemap);
                    var path = string.Empty;

                    var items = assets.Select(a =>
                    {
                        var (id, info) = a;
                        string label = $"{Path.GetFileName(info.Path)}##{id}";

                        return (label, (Action)(() => setValue(Assets.GetAssetFromGuid(id))), info.Path);
                    });
                    RenderItemsInColumns(items);

                }
            }

            else if (valueType == typeof(Sprite))
            {
                //var assets = IOLayer.Database.Disk.GetAssetsInfo(SharedTypes.AssetType.Texture);
                //foreach (var (id, info) in assets)
                //{
                //    // TODO: this is very very slow, I have to cache all the sprites names on load.
                //    var meta = EditorAssetUtils.GetAssetMeta(info.Path, AssetType.Texture) as TextureMetaFile;
                //    // var texturesInfo = EditorIOLayer.Database.GetAssetsInfoByType(AssetType.Texture);

                //    if (meta?.AtlasData == null || meta.AtlasData.ChunksCount == 0)
                //        continue;

                //    // TODO: use a tree node for multi sprites
                //    //if (ImGui.TreeNode())
                //    //{

                //    //}

                //    for (int i = 0; i < meta.AtlasData.ChunksCount; i++)
                //    {
                //        var name = Sprite.CreateSpriteName(Path.GetFileName(info.Path), i);
                //        if (ImGui.Selectable($"{name}##{meta.AtlasData.GetCell(i).ID}{i}{info.Path}"))
                //        {
                //            setValue(Assets.GetSpriteAtlas(info.Path).GetSprite(i));
                //        }
                //    }
                //}

                var assets = IOLayer.Database.Disk.GetAssetsInfo(Engine.AssetType.Texture);
                var spriteItems = new List<(string label, Action action, string path)>();

                foreach (var (id, info) in assets)
                {
                    var meta = EditorAssetUtils.GetAssetMeta(info.Path, AssetType.Texture) as TextureMetaFile;
                    if (meta?.AtlasData == null || meta.AtlasData.ChunksCount == 0)
                        continue;

                    var atlas = Assets.GetSpriteAtlas(info.Path);

                    for (int i = 0; i < meta.AtlasData.ChunksCount; i++)
                    {
                        var name = Sprite.CreateSpriteName(Path.GetFileName(info.Path), i);
                        var cellId = meta.AtlasData.GetCell(i).ID;
                        var label = $"{name}##{cellId}{i}{info.Path}";

                        // This copy is needed since I'm passing it to a lambda.
                        int iCopy = i;

                        spriteItems.Add((label, () =>
                        {
                            setValue(atlas.GetSprite(iCopy));
                        }, info.Path));
                    }
                }

                // Render all sprites in columns (choose number of columns here)
                RenderItemsInColumns(spriteItems);
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
