using Editor.Utils;
using Engine;
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
        private static float _xPosOffset = 140;
        public static void DrawVars(string entityID, object obj, PropertyInfo prop, float cursorX, int index, float width, bool enforceSerializedFieldAttribute)
        {
            if (prop == null || !prop.CanRead)
                return;

            object value = prop.GetValue(obj);
            Type type = prop.PropertyType;
            string propertyName = prop.Name;
            if (enforceSerializedFieldAttribute)
            {
                var attrib = prop.GetCustomAttribute<SerializedFieldAttribute>();
                if (attrib == null)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(attrib.CustomFieldName))
                {
                    propertyName = attrib.CustomFieldName;
                }
            }

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
                    prop.SetValue(obj, _copiedValue);

                if (ImGui.Selectable("Clear"))
                    prop.SetValue(obj, type.IsValueType ? Activator.CreateInstance(type) : null);

                ImGui.EndPopup();
            }

            // EObject
            if (ReflectionUtil.IsEObject(type))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                DrawEObjectSlot(value as EObject, type, v =>
                {
                    prop.SetValue(obj, v);
                    return true;
                }, width);
            }
            // bool
            else if (type == typeof(bool))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                bool v = (bool)value;
                if (EditorGuiFieldsResolver.DrawBoolField(propertyName, ref v))
                    prop.SetValue(obj, v);
            }
            else if (type == typeof(int))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                var v = (int)value;
                if (EditorGuiFieldsResolver.DrawIntField(propertyName, ref v))
                    prop.SetValue(obj, v);
            }
            else if (type == typeof(Color))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                var v = (Color)value;
                if (EditorGuiFieldsResolver.DrawColorField(propertyName, ref v))
                    prop.SetValue(obj, v);
            }
            else if (type == typeof(Color32))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                var v = (Color)value;
                if (EditorGuiFieldsResolver.DrawColorField(propertyName, ref v))
                    prop.SetValue(obj, (Color32)v);
            }
            // float
            else if (type == typeof(float))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                float v = (float)value;
                if (EditorGuiFieldsResolver.DrawFloatField(propertyName, ref v))
                    prop.SetValue(obj, v);
            }
            // string
            else if (type == typeof(string))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                string v = (string)value;
                if (EditorGuiFieldsResolver.DrawStringField(propertyName, ref v))
                    prop.SetValue(obj, v);
            }
            else if (type == typeof(vec2))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                vec2 v = (vec2)value;
                if (EditorGuiFieldsResolver.DrawVec2Field(propertyName, ref v))
                    prop.SetValue(obj, v);
            }
            else if (type == typeof(vec3))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                vec3 v = (vec3)value;
                if (EditorGuiFieldsResolver.DrawVec3Field(propertyName, ref v))
                    prop.SetValue(obj, v);
            }
            else if (type == typeof(vec4))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                vec4 v = (vec4)value;
                if (EditorGuiFieldsResolver.DrawVec4Field(propertyName, ref v))
                    prop.SetValue(obj, v);
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
                    prop.SetValue(obj, Enum.Parse(type, names[idx]));
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                if (value is System.Collections.IList list)
                {
                    if (EditorGuiFieldsResolver.DrawListField(propertyName, list))
                    {
                        prop.SetValue(obj, list);
                    }
                }
            }
            else if (type.IsArray)
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(_xPosOffset, ImGui.GetCursorPosX()));

                int size = ((Array)obj).Length;
                EditorGuiFieldsResolver.DrawListField(propertyName, size, () =>
                {
                    // Add
                }, (x) =>
                {
                    // Remove
                }, (x, y) =>
                {
                    // Draw callback

                    return false;
                }, false);

                // prop.SetValue(obj, list);

            }
            // class
            else if (type.IsClass)
            {
                foreach (var subProp in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    DrawVars(entityID, value, subProp, cursorX, index++, width, enforceSerializedFieldAttribute);
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
            else if (typeof(EObject).IsAssignableFrom(targetType))
            {
                if (ImGui.Selectable($"{root.Name}##{root.GetID()}"))
                {
                    setValue(root.Actor);
                    ImGui.CloseCurrentPopup();
                }
            }
            for (int i = 0; i < root.Children.Count; i++)
            {
                DrawSceneObjectPropertyPicker(root.Children[i], targetType, setValue);
            }
        }
    }
}
