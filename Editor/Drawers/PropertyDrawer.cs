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

        private delegate bool DrawSimpleFieldDelegate<T>(string fieldName, ref T v, float width = 0, bool pressEnterToConfirm = false);
        private readonly static Type[] _visibilityAttributes = [typeof(SerializedFieldAttribute), typeof(ShowFieldNoSerialize)];



        private static void DrawSimpleProperty<T>(string propertyName, object target, object value, bool isReadOnly,
                                                  MemberInfo prop, int index, float width, DrawSimpleFieldDelegate<T> drawField,
                                                  bool sameLine = true, Func<T, object> valueConverter = null)
        {
            if (sameLine)
            {
                ImGui.SameLine();
            }
            ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

            ImGui.BeginDisabled(isReadOnly);
            var v = (T)value;
            if (drawField(propertyName, ref v, width, false))
            {
                ReflectionUtils.SetMemberValueSafe(target, v, prop, index, valueConverter);
            }
            ImGui.EndDisabled();
        }

        public static void DrawVars(string objectId, object target, MemberInfo prop, float cursorX, int index, float width, bool enforceSerializedFieldAttribute)
        {
            if (prop == null)
                return;

            object value = ReflectionUtils.GetMemberValue(target, prop);
            Type type = ReflectionUtils.GetMemberType(prop);
            string propertyName = prop.Name;
            // NOTE: I will enforce that any property that needs to be exposed in the editor should have the
            //       SerializedField/ShowFieldNoSerialize attribute

            bool isReadOnly = false;

            if (enforceSerializedFieldAttribute)
            {
                PropertyVisibilityAttribute attrib = prop.GetCustomAttribute<SerializedFieldAttribute>();

                if (attrib == null)
                {
                    attrib = prop.GetCustomAttribute<ShowFieldNoSerialize>();
                    if (attrib == null)
                    {
                        return;
                    }
                }

                if (prop.GetCustomAttribute<HideFromInspectorAttribute>() != null)
                {
                    return;
                }

                isReadOnly = attrib.IsReadOnly;

                if (!string.IsNullOrEmpty(attrib.CustomFieldName))
                {
                    propertyName = attrib.CustomFieldName;
                }
            }

            DrawVars(objectId, target, value, type, propertyName, isReadOnly, prop, cursorX, index, width);
        }

        public static void DrawVars(string objectId, object target, object value, Type type, string propertyName,
                                    bool isReadOnly, MemberInfo prop, float cursorX, int index, float width)
        {
            var header = prop.GetCustomAttribute<PropertyHeaderAttribute>();
            if (header != null)
            {
                ImGui.Indent();
                ImGui.Text(header.HeaderText);
                ImGui.Unindent();
            }

            if (value == null)
            {
                value = ReflectionUtils.GetDefaultValueInstance(type);
                ReflectionUtils.SetMemberValueSafe(target, value, prop, index);
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

            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0));
            ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Vector4(0));

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
                ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

                var eObject = value as IObject;
                var eObjectType = eObject != null ? eObject.GetType() : type;

                EditorGuiFieldsResolver.DrawEObjectSlot(eObject, eObjectType, v =>
                {
                    ReflectionUtils.SetMemberValueSafe(target, v, prop, index);
                    return true;
                });
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
            else if (type == typeof(quat))
            {
                var q = (quat)value; ;
                vec4 v = new vec4(q.x, q.y, q.z, q.w);
                DrawSimpleProperty<vec4>(propertyName, target, v, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawVec4Field, true, v => value = new quat(v.x, v.y, v.z, v.w));
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
                ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

                int idx = (int)value;
                string[] names = Enum.GetNames(type);
                if (EditorGuiFieldsResolver.DrawCombo(propertyName, ref idx, names, width))
                {
                    ReflectionUtils.SetMemberValueSafe(target, Enum.Parse(type, names[idx]), prop, index);
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (value == null)
                {
                    value = ReflectionUtils.GetDefaultValueInstance(type);
                }

                var list = value as IList;

                var elementType = type.GetGenericArguments().FirstOrDefault();

                DrawList(objectId, propertyName, list, value, elementType, prop, cursorX, OnAdd, OnRemove, OnRemoveCount);
                ReflectionUtils.SetMemberValueSafe(target, value, prop, index);

                void OnAdd(IList list, int totalLength)
                {
                    while (list.Count < totalLength)
                    {
                        list.Add(ReflectionUtils.GetDefaultValueInstance(elementType));
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
                    value = ReflectionUtils.GetDefaultValueInstance(type);
                }

                var array = value as Array;

                var elementType = type.GetElementType();

                DrawList(objectId, propertyName, array, value, elementType, prop, cursorX, OnAdd, OnRemove, OnRemoveCount);
                ReflectionUtils.SetMemberValueSafe(target, value, prop, index);

                void OnAdd(IList list, int totalLength)
                {
                    var array = list as Array;

                    var copy = Array.CreateInstance(elementType, totalLength);
                    Array.Copy(array, copy, array.Length);
                    value = copy;
                    ReflectionUtils.SetMemberValueSafe(target, value, prop, index);
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
                    ReflectionUtils.SetMemberValueSafe(target, value, prop, index);
                }

                void OnRemoveCount(IList list, int totalLength)
                {
                    var array = list as Array;
                    var copy = Array.CreateInstance(elementType, totalLength);
                    Array.Copy(array, copy, totalLength);
                    value = copy;
                    ReflectionUtils.SetMemberValueSafe(target, value, prop, index);
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                if (value == null)
                {
                    value = ReflectionUtils.GetDefaultValueInstance(type);
                }

                var dictionary = value as IDictionary;
                var keyDictType = type.GetGenericArguments()[0];
                var valueDictType = type.GetGenericArguments()[1];

                if (EditorGuiFieldsResolver.DrawDictionaryField(propertyName, dictionary, false))
                {

                }

                //DrawList(objectId, propertyName, list, value, elementType, prop, cursorX, OnAdd, OnRemove, OnRemoveCount);
            }
            else if (type.IsClass || ReflectionUtils.IsUserDefinedStruct(type))
            {
                var members = ReflectionUtils.GetAllMembersWithAttributes(type, _visibilityAttributes, true, true);

                var propIndex = index;
                foreach (var subProp in members)
                {
                    if (value != null)
                    {
                        DrawVars(objectId, value, subProp, cursorX, index++, width, true);
                    }
                }
                ReflectionUtils.SetMemberValueSafe(target, value, prop, propIndex);
                DrawMethods(value, objectId);

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
                ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

                var listType = value.GetType();

                EditorGuiFieldsResolver.DrawListField(propertyName, list, false, onAddCallback, onRemoveCallback, removeCount,
                  (index, itemWidth, item) =>
                  {
                      if (item == null)
                      {
                          item = ReflectionUtils.GetDefaultValueInstance(elemenType);
                      }

                      DrawVars(objectId, list, item, elemenType, $"##__{index}_item", false, prop, cursorX, index, itemWidth);

                      return false;
                  });
            }
        }

        private static readonly List<(MethodInfo method, bool nextSameLine)> _invokableMethods = new();
        public static void DrawMethods(object target, string objectId)
        {
            // TODO: This is very slow, so slow that I will comment it to implement it better once the hot reloading system is implemented.
            if (target == null)
                return;
            _invokableMethods.Clear();
            var methods = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (methods.Length > 0)
            {
                for (var i = 0; i < methods.Length; i++)
                {
                    var method = methods[i];
                    var showMethod = method.GetCustomAttribute<ShowMethodInEditorAttribute>();
                    if (showMethod != null && method.GetParameters().Length == 0 && !method.ContainsGenericParameters)
                    {

                        _invokableMethods.Add((method, showMethod.SameLineNextFunction));

                    }
                }
                bool needNewLine = false;
                var buttonSizeX = ImGui.GetContentRegionAvail().X - ImGui.GetStyle().FramePadding.X * _invokableMethods.Count;

                var buttonSize = new Vector2(buttonSizeX / _invokableMethods.Count, 23);
                for (var i = 0; i < _invokableMethods.Count; i++)
                {
                    var method = _invokableMethods[i];

                    needNewLine = false;
                    if (ImGui.Button(method.method.Name + $"##_METHOD_{objectId}_{i}", buttonSize))
                    {
                        method.method.Invoke(target, null);
                    }

                    if (method.nextSameLine)
                    {
                        needNewLine = true;
                        ImGui.SameLine();
                    }
                }
                if (needNewLine)
                {
                    ImGui.NewLine();
                }
            }
        }
    }
}
