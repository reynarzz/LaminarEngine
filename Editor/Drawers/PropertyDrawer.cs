using Editor.Utils;
using Engine;
using Engine.Utils;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class PropertyDrawer
    {
        private static object _copiedValue;
        private static float _xPosOffset = 180;

        private delegate bool DrawSimpleFieldDelegate<T>(string fieldName, ref T v);

        private static void DrawSimpleProperty<T>(string propertyName, object target, object value, bool isReadOnly, MemberInfo prop, DrawSimpleFieldDelegate<T> drawField, bool sameLine = true, Func<T, object> valueConverter = null)
        {
            if (sameLine)
            {
                ImGui.SameLine();
            }
            ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

            ImGui.BeginDisabled(isReadOnly);
            var v = (T)value;
            if (drawField(propertyName, ref v))
            {
                //if (valueConverter == null)
                //{
                //    ReflectionUtils.SetMemberValue(target, prop, v);
                //}
                //else
                //{
                //    ReflectionUtils.SetMemberValue(target, prop, valueConverter.Invoke(v));
                //}
            }
            ImGui.EndDisabled();
        }
        private static bool IsUserDefinedStruct(Type t)
        {
            return t.IsValueType &&
                   !t.IsPrimitive &&
                   !t.IsEnum;
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
                var attrib = prop.GetCustomAttribute<ExposeEditorFieldAttribute>();
                if (attrib == null)
                {
                    return;
                }

                isReadOnly = attrib.IsReadOnly;

                if (!string.IsNullOrEmpty(attrib.CustomFieldName))
                {
                    propertyName = attrib.CustomFieldName;
                }
            }

            DrawVars(entityID, target, value, type, propertyName, isReadOnly, prop, cursorX, index, width);
        }
        public static void DrawVars(string entityID, object target, object value, Type type, string propertyName, bool isReadOnly, MemberInfo prop, float cursorX, int index, float width)
        {
            ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.Leaf;

            if (type.GetProperties().Length > 1 && type.IsClass && type != typeof(string) && !ReflectionUtil.IsEObject(type))
            {
                flags = ImGuiTreeNodeFlags.OpenOnArrow;
            }

            if (!ImGui.TreeNodeEx($"{propertyName}##{entityID}{index}", flags))
                return;

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
            if (ReflectionUtil.IsEObject(type))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                DrawEObjectSlot(value as EObject, type, v =>
                {
                    ReflectionUtils.SetMemberValue(target, prop, v);
                    return true;
                }, width);
            }
            // bool
            else if (type == typeof(bool))
            {
                DrawSimpleProperty<bool>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawBoolField);
            }
            else if (type == typeof(int))
            {
                DrawSimpleProperty<int>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawIntField);
            }
            else if (type == typeof(Color))
            {
                DrawSimpleProperty<Color>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawColorField);
            }
            else if (type == typeof(Color32))
            {
                DrawSimpleProperty<Color>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawColorField, true, v => (Color32)v);
            }
            else if (type == typeof(float))
            {
                DrawSimpleProperty<float>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawFloatField);
            }
            else if (type == typeof(string))
            {
                DrawSimpleProperty<string>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawStringField);
            }
            else if (type == typeof(vec2))
            {
                DrawSimpleProperty<vec2>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawVec2Field);
            }
            else if (type == typeof(vec3))
            {
                DrawSimpleProperty<vec3>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawVec3Field);
            }
            else if (type == typeof(vec4))
            {
                DrawSimpleProperty<vec4>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawVec4Field);
            }
            else if (type == typeof(mat2))
            {
                DrawSimpleProperty<mat2>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawMatrix);
            }
            else if (type == typeof(mat3))
            {
                DrawSimpleProperty<mat3>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawMatrix);
            }
            else if (type == typeof(mat4))
            {
                DrawSimpleProperty<mat4>(propertyName, target, value, isReadOnly, prop, EditorGuiFieldsResolver.DrawMatrix);
            }
            // enum
            else if (type.IsEnum)
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                int idx = (int)value;
                string[] names = Enum.GetNames(type);
                if (EditorGuiFieldsResolver.DrawCombo(propertyName, ref idx, names))
                {
                    ReflectionUtils.SetMemberValue(target, prop, Enum.Parse(type, names[idx]));

                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (value is System.Collections.IList list)
                {
                    var arg = list.GetType().GetGenericArguments().FirstOrDefault();

                    if (arg == null || !arg.IsGenericType)
                    {
                        ImGui.SameLine();
                        ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

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

                        EditorGuiFieldsResolver.DrawListField(propertyName, list, OnAdd, OnRemove, (index, width, item) =>
                        {
                            int listIndex = 0;

                            if (item == null)
                            {
                                item = GetDefaultValue(itemType);
                            }
                            DrawVars(entityID, list, item, itemType, $"##__{index}_item", false, prop, cursorX, listIndex++, width);

                            return false;
                        }, false);

                        void OnAdd()
                        {
                            list.Add(GetDefaultValue(arg));
                        }

                        void OnRemove(int index)
                        {
                            if (list.Count > 0 && list.Count > index)
                            {
                                list.RemoveAt(index);
                            }

                            Debug.Log(index);
                        }
                    }
                }
            }
            else if (type.IsArray)
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                int size = ((Array)target).Length;
                //EditorGuiFieldsResolver.DrawListField(propertyName, size, () =>
                //{
                //    // Add
                //}, (x) =>
                //{
                //    // Remove
                //}, (x, y, item) =>
                //{
                //    // Draw callback

                //    return false;
                //}, false);

                // prop.SetValue(obj, list);

            }
            // class
            else if (type.IsClass || IsUserDefinedStruct(type))
            {
                var members = ReflectionUtils.GetAllMembersWithAttribute<ExposeEditorFieldAttribute>(type, true, true);
                foreach (var subProp in members)
                {
                     DrawVars(entityID, value, subProp, cursorX, index++, width, true);
                }
            }

            ImGui.TreePop();
        }

        private static void DrawEObjectSlot(EObject value, Type valueType, Func<object, bool> setValue, float width = -1)
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(MathF.Max(_xPosOffset, ImGui.GetCursorPosX()) + 5);

            string label = value != null ? $"{(value).Name} ({valueType.Name})" : $"null ({valueType.Name})";

            if (value != null)
            {
                if (value is AssetResourceBase res)
                {
                    ImGui.SetItemTooltip($"{res.Path}");
                }
                else
                {
                    ImGui.SetItemTooltip(value.GetID().ToString());
                }
            }

            var drawList = ImGui.GetWindowDrawList();
            var pos = ImGui.GetCursorScreenPos();
            var size = ImGui.CalcTextSize(label);

            drawList.AddRectFilled(new(pos.X - 5, pos.Y - 3), new(pos.X + ImGui.GetContentRegionAvail().X - 5, pos.Y + size.Y + 3), ImGui.ColorConvertFloat4ToU32(new(0.1f, 0.1f, 0.1f, 1f)));

            ImGui.Text(label);

            if (ImGui.IsItemClicked())
            {
                _openPopup = true;
                _selectedValue = value;
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
            else if (typeof(Component).IsAssignableFrom(targetType))
            {
                if (ImGui.Selectable($"{root.Name}##{root.GetID()}"))
                {
                    foreach (var comp in root.Actor.Components)
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
                else if (type.IsClass)
                {
                    try
                    {
                        return Activator.CreateInstance(type)!;
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                return null!;
            }
        }

    }
}
