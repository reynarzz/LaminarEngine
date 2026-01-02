using Editor.Utils;
using Engine;
using Engine.Layers;
using Engine.Utils;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class PropertyDrawer
    {
        private static object _copiedValue;
        private static float _xPosOffset = 180;

        private delegate bool DrawSimpleFieldDelegate<T>(string fieldName, ref T v, float width = 0, bool pressEnterToConfirm = false);

        private static void SetMemberValueSafe<T>(object target, T value, MemberInfo prop, int index, Func<T, object> valueConverter = null)
        {
            if (target is IList list)
            {
                list[index] = value;
            }
            else
            {
                if (valueConverter == null)
                {
                    ReflectionUtils.SetMemberValue(target, prop, value);
                }
                else
                {
                    ReflectionUtils.SetMemberValue(target, prop, valueConverter.Invoke(value));
                }
            }
        }
        private static void DrawSimpleProperty<T>(string propertyName, object target, object value, bool isReadOnly,
                                                  MemberInfo prop, int index, float width, DrawSimpleFieldDelegate<T> drawField,
                                                  bool sameLine = true, Func<T, object> valueConverter = null)
        {
            if (sameLine)
            {
                ImGui.SameLine();
            }
            ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

            ImGui.BeginDisabled(isReadOnly);
            var v = (T)value;
            if (drawField(propertyName, ref v, width, false))
            {
                SetMemberValueSafe(target, v, prop, index, valueConverter);
            }
            ImGui.EndDisabled();
        }

        public static void DrawVars(string entityID, object target, MemberInfo prop, float cursorX, int index, float width, bool enforceSerializedFieldAttribute)
        {
            if (prop == null)
                return;

            object value = ReflectionUtils.GetMemberValue(target, prop);
            Type type = ReflectionUtils.GetMemberType(prop);
            string propertyName = prop.Name;
            // NOTE: I will enforce that any property that needs to be exposed in the editor should have the SerializedField attribute

            bool isReadOnly = false;

            if (enforceSerializedFieldAttribute)
            {
                var attrib = prop.GetCustomAttribute<SerializedFieldAttribute>();
                if (attrib == null || prop.GetCustomAttribute<HideFromInspectorAttribute>() != null)
                {
                    return;
                }

                isReadOnly = attrib.IsReadOnly;

                if (!string.IsNullOrEmpty(attrib.CustomFieldName))
                {
                    propertyName = attrib.CustomFieldName;
                }
            }

            var header = prop.GetCustomAttribute<PropertyHeaderAttribute>();
            if (header != null)
            {
                ImGui.Indent();
                ImGui.Text(header.HeaderText);
                ImGui.Unindent();
            }

            DrawVars(entityID, target, value, type, propertyName, isReadOnly, prop, cursorX, index, width);
        }
        public static void DrawVars(string objectId, object target, object value, Type type, string propertyName,
                                    bool isReadOnly, MemberInfo prop, float cursorX, int index, float width)
        {
            if (value == null)
            {
                value = GetDefaultValue(type);
                SetMemberValueSafe(target, value, prop, index);
            }

            ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.Leaf;

            var isValidClass = (type.IsClass) && type != typeof(string);
            if ((ReflectionUtils.GetPropertiesCount(prop) > 1 && (isValidClass) &&
                !ReflectionUtils.IsEObject(type)) ||
                (ReflectionUtils.IsCollection(prop) && isValidClass && !ReflectionUtils.IsEObject(type)) ||
                ReflectionUtils.IsUserDefinedStruct(prop))
            {
                flags = ImGuiTreeNodeFlags.OpenOnArrow;
            }

            unsafe
            {

                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0));
                ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Vector4(0));
            }

            var show = ImGui.TreeNodeEx($"{propertyName}##{objectId}{index}", flags);
            if (!show)
            {
                ImGui.PopStyleColor(2);
                return;
            }

            ImGui.PopStyleColor(2);

            // Context menu
            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.Selectable("Copy"))
                    _copiedValue = value;

                if (ImGui.Selectable("Paste") && _copiedValue?.GetType() == type)
                {
                    ReflectionUtils.SetMemberValue(target, prop, _copiedValue);
                }

                if (ImGui.Selectable("Clear"))
                {
                    var valueClear = type.IsValueType ? Activator.CreateInstance(type) : null;
                    ReflectionUtils.SetMemberValue(target, prop, valueClear);
                }

                ImGui.EndPopup();
            }

            // EObject
            if (ReflectionUtils.IsEObject(type))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                var eObject = value as IObject;
                var eObjectType = eObject != null ? eObject.GetType() : type;

                DrawEObjectSlot(eObject, eObjectType, v =>
                {
                    SetMemberValueSafe(target, v, prop, index);
                    return true;
                }, width);
            }
            else if (type == typeof(bool))
            {
                DrawSimpleProperty<bool>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawBoolField);
            }
            else if (type == typeof(int))
            {
                DrawSimpleProperty<int>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawIntField);
            }
            else if (type == typeof(uint))
            {
                DrawSimpleProperty<uint>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawUIntField);
            }
            else if (type == typeof(long))
            {
                DrawSimpleProperty<long>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawLongField);
            }
            else if (type == typeof(ulong))
            {
                DrawSimpleProperty<ulong>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawULongField);
            }
            else if (type == typeof(Color))
            {
                DrawSimpleProperty<Color>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawColorField);
            }
            else if (type == typeof(Color32))
            {
                DrawSimpleProperty<Color>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawColorField, true, v => (Color32)v);
            }
            else if (type == typeof(float))
            {
                DrawSimpleProperty<float>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawFloatField);
            }
            else if (type == typeof(double))
            {
                DrawSimpleProperty<double>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawDoubleField);
            }
            else if (type == typeof(string))
            {
                DrawSimpleProperty<string>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawStringField);
            }
            else if (type == typeof(vec2))
            {
                DrawSimpleProperty<vec2>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawVec2Field);
            }
            else if (type == typeof(vec3))
            {
                DrawSimpleProperty<vec3>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawVec3Field);
            }
            else if (type == typeof(vec4))
            {
                DrawSimpleProperty<vec4>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawVec4Field);
            }
            else if (type == typeof(mat2))
            {
                DrawSimpleProperty<mat2>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawMatrix);
            }
            else if (type == typeof(mat3))
            {
                DrawSimpleProperty<mat3>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawMatrix);
            }
            else if (type == typeof(mat4))
            {
                DrawSimpleProperty<mat4>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawMatrix);
            }
            else if (type.IsEnum)
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                int idx = (int)value;
                string[] names = Enum.GetNames(type);
                if (EditorGuiFieldsResolver.DrawCombo(propertyName, ref idx, names, width))
                {
                    SetMemberValueSafe(target, Enum.Parse(type, names[idx]), prop, index);
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (value == null)
                {
                    value = GetDefaultValue(type);
                }

                var list = value as IList;

                var elementType = type.GetGenericArguments().FirstOrDefault();

                DrawList(objectId, propertyName, list, value, elementType, prop, cursorX, OnAdd, OnRemove, OnRemoveCount);
                SetMemberValueSafe(target, value, prop, index);

                void OnAdd(IList list, int totalLength)
                {
                    while (list.Count < totalLength)
                    {
                        list.Add(GetDefaultValue(elementType));
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
            }
            else if (type.IsArray)
            {
                if (value == null)
                {
                    value = Array.CreateInstance(type.GetElementType(), 0);
                }

                var array = value as Array;

                var elementType = type.GetElementType();

                DrawList(objectId, propertyName, array, value, elementType, prop, cursorX, OnAdd, OnRemove, OnRemoveCount);
                SetMemberValueSafe(target, value, prop, index);

                void OnAdd(IList list, int totalLength)
                {
                    var array = list as Array;

                    var copy = Array.CreateInstance(elementType, totalLength);
                    Array.Copy(array, copy, array.Length);
                    value = copy;
                    SetMemberValueSafe(target, value, prop, index);
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

                    value = copy;
                    SetMemberValueSafe(target, value, prop, index);
                }

                void OnRemoveCount(IList list, int totalLength)
                {
                    var array = list as Array;
                    var copy = Array.CreateInstance(elementType, totalLength);
                    Array.Copy(array, copy, totalLength);
                    value = copy;
                    SetMemberValueSafe(target, value, prop, index);
                }
            }
            else if (type.IsClass || ReflectionUtils.IsUserDefinedStruct(type))
            {
                var members = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(type, true, true);

                var propIndex = index;
                foreach (var subProp in members)
                {
                    DrawVars(objectId, value, subProp, cursorX, index++, width, true);
                }
                SetMemberValueSafe(target, value, prop, propIndex);
            }

            ImGui.TreePop();
        }

        private static void DrawList(string objectId, string propertyName, IList list, object value, Type elemenType,
                                     MemberInfo prop, float cursorX, Action<IList, int> onAddCallback, Action<IList, int> onRemoveCallback,
                                     Action<IList, int> removeCount)
        {
            if (elemenType == null || !elemenType.IsGenericType)
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                var listType = value.GetType();

                EditorGuiFieldsResolver.DrawListField(propertyName, list, false, onAddCallback, onRemoveCallback, removeCount,
                  (index, itemWidth, item) =>
                  {
                      if (item == null)
                      {
                          item = GetDefaultValue(elemenType);
                      }

                      DrawVars(objectId, list, item, elemenType, $"##__{index}_item", false, prop, cursorX, index, itemWidth);

                      return false;
                  });
            }
        }

        private static void DrawEObjectSlot(IObject eObject, Type valueType, Func<object, bool> setValue, float width = -1)
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(MathF.Max(_xPosOffset, ImGui.GetCursorPosX()) + 5);

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

        static bool _openPopup;
        static object _selectedValue;
        static Func<object, bool> _selectedSetter;

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
                        if (ImGui.Selectable($"{System.IO.Path.GetFileName(asset.Value.Path)}##{asset.Key}"))
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

        private static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                if (type.IsEnum)
                {
                    Array values = Enum.GetValues(type);
                    return values.Length > 0 ? values.GetValue(0)! : Activator.CreateInstance(type);
                }
                else
                {
                    return Activator.CreateInstance(type);
                }
            }
            else
            {
                if (type == typeof(string))
                {
                    return string.Empty;
                }
                else if (type.IsClass && !type.IsAssignableTo(typeof(IObject)))
                {
                    try
                    {
                        return Activator.CreateInstance(type);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                return null;
            }
        }

    }
}
