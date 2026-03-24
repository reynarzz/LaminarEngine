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
using System.ComponentModel.Design;

namespace Editor.Utils
{
    public partial class EditorGuiFieldsResolver
    {
        public const float XPosOffset = 180;
        private static readonly Vector2 VECTOR_INNER_SPACING = new Vector2(3, 2);
        private static bool _openPopup;

        public delegate (object valueOut, bool result) onDrawDictionaryArgCallback(Type argKey, string argName, object argValue);
        private const string DEFAULT_COLLECTION_ITEM_TITLE = "item";

        public EditorGuiFieldsResolver()
        {
        }

        internal static void SetPropertyDefaultCursorPos()
        {
            ImGui.SetCursorPosX(Math.Max(XPosOffset, ImGui.GetCursorPosX()));
            SetNextItemWidth(0);
        }

        internal static float GetIdentation()
        {
            return ImGui.GetTreeNodeToLabelSpacing();
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
            return DrawStringField(name, ref value, 0, false, false);
        }
        public static bool DrawStringField(string name, ref string value, float itemWidth = 0, bool pressEnterToConfirm = false)
        {
            return DrawStringField(name, ref value, itemWidth, pressEnterToConfirm, false);
        }
        public static bool DrawStringField(string name, ref string value, float itemWidth = 0, bool pressEnterToConfirm = false, bool autoFocus = false)
        {
            int bufferSize = Math.Max(257, value?.Length + 1 ?? 257);
            SetNextItemWidth(itemWidth);

            var refStr = value;
            if (autoFocus)
            {
                ImGui.SetKeyboardFocusHere();
            }
            var changed = ImGui.InputText("##" + name, ref refStr, (uint)bufferSize);

            if (changed)
            {
                value = refStr;
            }

            var isEnterPressed = ImGui.IsKeyDown(ImGuiKey.Enter) || ImGui.IsKeyDown(ImGuiKey.KeypadEnter);
            return changed && ((pressEnterToConfirm && isEnterPressed) || !pressEnterToConfirm);
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

            var barSizeOffset = new Vector2(1, 0);
            var barPositionOffset = new Vector2(0, -3);

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

        public static bool DrawEnum<T>(string name, ref T value, float itemWidth = 0, bool pressEnterToConfirm = false) where T : Enum
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(Math.Max(XPosOffset, ImGui.GetCursorPosX()));

            string[] names = Enum.GetNames(typeof(T));

            // Find the index of the value
            var idx = Array.IndexOf(Enum.GetValues(typeof(T)), value);
            if (DrawCombo(name, ref idx, names, itemWidth))
            {
                value = (T)Enum.Parse(typeof(T), names[idx]);
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

        public delegate bool ListFieldDrawDelegate(int index, float width, object element);
        public static bool DrawListField<T>(string name, List<T> collection, bool itemAsTree, ListFieldDrawDelegate drawCallback)
        {
            void OnAdd(IList list, int totalLength)
            {
                while (list.Count < totalLength)
                {
                    list.Add(ReflectionUtils.GetDefaultValueInstance(typeof(T)));
                }
            }

            void OnRemove(IList list, int itemIndex)
            {
                if (list.Count > 0)
                {
                    list.RemoveAt(itemIndex);
                }
            }

            void OnRemoveCount(IList list, int totalLength)
            {
                for (int i = list.Count - 1; i >= totalLength; --i)
                {
                    list.RemoveAt(i);
                }
            }

            return DrawListField(name, collection, itemAsTree, OnAdd, OnRemove, OnRemoveCount, drawCallback);
        }
        public static bool DrawArrayField<T>(string name, ref T[] collection, bool itemAsTree, ListFieldDrawDelegate drawCallback)
        {
            return DrawArrayField(name, ref collection, itemAsTree, drawCallback, null);
        }

        public static bool DrawArrayField<T>(string name, ref T[] collection, bool itemAsTree, ListFieldDrawDelegate drawCallback, Action<T> onAdded)
        {
            return DrawArrayField(name, ref collection, itemAsTree, 0, DEFAULT_COLLECTION_ITEM_TITLE, true, drawCallback, onAdded);
        }
        public static bool DrawArrayField<T>(string name, ref T[] collection, bool itemAsTree, int maxLength, string itemsTitle, bool canChangeSize, ListFieldDrawDelegate drawCallback, Action<T> onAdded)
        {
            var elementType = collection.GetType().GetElementType();
            var colNew = collection;

            void OnAdd(IList list, int totalLength)
            {
                var array = list as Array;

                var copy = Array.CreateInstance(elementType, totalLength);
                Array.Copy(array, copy, array.Length);
                colNew = copy as T[];

                for (int i = array.Length; i < totalLength; i++)
                {
                    var defaultValue = (T)ReflectionUtils.GetDefaultValueInstance(typeof(T));
                    colNew[i] = defaultValue;
                    onAdded?.Invoke(defaultValue);
                }
            }

            void OnRemove(IList list, int itemIndex)
            {
                var array = list as Array;
                var copy = Array.CreateInstance(elementType, array.Length - 1);

                int copyIndex = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    if (i != itemIndex)
                    {
                        copy.SetValue(array.GetValue(i), copyIndex);
                        copyIndex++;
                    }
                }

                colNew = copy as T[];
            }

            void OnRemoveCount(IList list, int totalLength)
            {
                var array = list as Array;
                var copy = Array.CreateInstance(elementType, totalLength);
                Array.Copy(array, copy, totalLength);
                colNew = copy as T[];
            }

            var resultChanged = DrawListField(name, collection, false, maxLength, itemsTitle, canChangeSize, OnAdd, OnRemove, OnRemoveCount, drawCallback);

            if (resultChanged)
            {
                collection = colNew;
            }

            return resultChanged;
        }
        public static bool DrawListField(string name, IList list, bool itemAsTree, Action<IList, int> onAddCallback, Action<IList, int> onRemoveCallback,
                                        Action<IList, int> removeCount, ListFieldDrawDelegate drawCallback)
        {
            return DrawListField(name, list, itemAsTree, 0, DEFAULT_COLLECTION_ITEM_TITLE, true, onAddCallback, onRemoveCallback, removeCount, drawCallback);
        }

        public static bool DrawListField(string name, IList list, bool itemAsTree, int maxLength, string itemsTitle, bool canChangeSize, Action<IList, int> onAddCallback, Action<IList, int> onRemoveCallback,
                                         Action<IList, int> removeCount, ListFieldDrawDelegate drawCallback)
        {
            bool changed = false;
            var size = list.Count;
            var cursorPosY = ImGui.GetCursorPosY();
            if (canChangeSize)
            {
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() - 120);
                ImGui.SetCursorPosY(cursorPosY + 3);

                ImGui.BeginDisabled(size - 1 < 0);
                if (EditorImGui.ImageButtonFromIcon($"-##Remove_{name}", EditorIcon.Minus, new vec2(12, 12)))
                {
                    changed = true;

                    if (size - 1 >= 0)
                    {
                        onRemoveCallback(list, size - 1);
                    }
                }

                ImGui.EndDisabled();
                ImGui.SameLine();
            }

            var isMaxLengthReached = maxLength > 0 && size == maxLength;

            if (canChangeSize)
            {
                ImGui.BeginDisabled(isMaxLengthReached);
                ImGui.SetCursorPosY(cursorPosY + 3);
                if (EditorImGui.ImageButtonFromIcon($"+##Add_{name}", EditorIcon.Plus, new vec2(12, 12)))
                {
                    changed = true;
                    onAddCallback(list, size + 1);
                }
                ImGui.EndDisabled();
                ImGui.SameLine();
            }

            string lenText = size.ToString();
            if (canChangeSize)
            {
                ImGui.SetNextItemWidth(53);
                if (DrawStringField($"##_size_{name}", ref lenText, 0, true))
                {
                    if (int.TryParse(lenText, out var val) && val >= 0)
                    {
                        if (size < val)
                        {
                            onAddCallback(list, val);
                            changed = true;
                        }
                        else if (size > val)
                        {
                            removeCount(list, val);
                            changed = true;
                        }

                        size = val;
                    }
                }
            }

            bool skip = false;

            for (int i = 0; i < list.Count; i++)
            {
                bool show;
                skip = false;
                var cursorPosX = ImGui.GetCursorPosX();

                if (canChangeSize)
                {
                    ImGui.SetCursorPosX(cursorPosX + 12);
                    if (EditorImGui.ImageButtonFromIcon($"_DELETE_BUTTON_{i}_{name}", EditorIcon.Close, new vec2(10, 10)))
                    {
                        onRemoveCallback(list, i);
                        changed = true;
                        skip = true;
                        break;
                    }
                    ImGui.SameLine();
                }
                cursorPosY = ImGui.GetCursorPosY();
                ImGui.SetCursorPosY(cursorPosY - 2);
                cursorPosX = ImGui.GetCursorPosX();
                ImGui.SetCursorPosX(cursorPosX - 2);
                if (itemAsTree)
                {
                    show = ImGui.TreeNode($"item {i}_{name}");
                }
                else
                {
                    ImGui.Text($"{itemsTitle} {i}");
                    show = true;
                }

                ImGui.SameLine();

                if (show)
                {
                    cursorPosY = ImGui.GetCursorPosY();
                    ImGui.SetCursorPosY(cursorPosY - 2);
                    // if (!skip)
                    {
                        try
                        {
                            if (drawCallback(i, 0, list[i]))
                                changed = true;
                        }
                        catch (Exception e)
                        {
                            Debug.Error(e);
                        }
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
            if (EditorImGui.ImageButtonFromIcon($"-##Remove_Dict{name}", EditorIcon.Minus, new vec2(12, 12)))
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
            if (EditorImGui.ImageButtonFromIcon($"+##Add_{name}", EditorIcon.Plus, new vec2(12, 12)))
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

                var cursorPosX = ImGui.GetCursorPosX();
                ImGui.SetCursorPosX(cursorPosX + 12);
                var cursorPosY = ImGui.GetCursorPosY();
                ImGui.SetCursorPosY(cursorPosY + 5);
                if (EditorImGui.ImageButtonFromIcon($"_DELETE_BUTTON_{i}_{name}", EditorIcon.Close, new vec2(10, 10)))
                {
                    dictionary.Remove(key);
                    changed = true;
                    skip = true;
                }
                ImGui.SameLine();
                var itemTitleCursorX = ImGui.GetCursorPosX();

                bool show;

                cursorPosY = ImGui.GetCursorPosY();
                ImGui.SetCursorPosY(cursorPosY - 2);
                cursorPosX = ImGui.GetCursorPosX();
                ImGui.SetCursorPosX(cursorPosX - 2);
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
                            changed = true;
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

    }
}
