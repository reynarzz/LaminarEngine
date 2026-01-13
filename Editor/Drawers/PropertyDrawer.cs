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
        public delegate void SetMemberValueSafeCallBack(object target, object value, MemberInfo prop, object collectionData,
                                                            Func<object, object> valueConverter = null);


        private static bool DrawSimpleProperty<T>(string propertyName, object target, object value, bool isReadOnly,
                                                  MemberInfo prop, object index, object collectionData, float width, DrawSimpleFieldDelegate<T> drawField,
                                                  SetMemberValueSafeCallBack setValueCallback,
                                                  bool sameLine = true, Func<T, object> valueConverter = null)
        {
            if (sameLine)
            {
                ImGui.SameLine();
            }
            ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

            ImGui.BeginDisabled(isReadOnly);
            var v = (T)value;
            var result = false;
            if (drawField(propertyName, ref v, width, false))
            {
                var data = index;

                if (data is IDictionary)
                {
                    data = collectionData;
                }

                if (valueConverter != null)
                {
                    setValueCallback(target, v, prop, data, x => valueConverter((T)x));
                }
                else
                {
                    setValueCallback(target, v, prop, data);
                }

                result = true;
            }
            ImGui.EndDisabled();

            return result;
        }

        public static bool DrawVars(string objectId, object target, MemberInfo prop, float cursorX, int index, object collectionData, float width,
                                    SetMemberValueSafeCallBack setMemberCallBack, bool enforceSerializedFieldAttribute)
        {
            if (prop == null)
                return false;

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
                        return false;
                    }
                }

                if (prop.GetCustomAttribute<HideFromInspectorAttribute>() != null)
                {
                    return false;
                }

                isReadOnly = attrib.IsReadOnly;

                if (!string.IsNullOrEmpty(attrib.CustomFieldName))
                {
                    propertyName = attrib.CustomFieldName;
                }
            }

            return DrawVars(objectId, target, value, type, propertyName, isReadOnly, prop, cursorX, index, collectionData,
                setMemberCallBack, width);
        }

        public static bool DrawVars(string objectId, object target, object value, Type type, string propertyName,
                                    bool isReadOnly, MemberInfo prop, float cursorX, int index, object collectionData,
                                    SetMemberValueSafeCallBack setMemberValueCallBack, float width)
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
                setMemberValueCallBack(target, value, prop, index);
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
                return false;
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

            bool resultChanged = false;
            // EObject
            if (ReflectionUtils.IsEObject(type))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

                var eObject = value as IObject;
                var eObjectType = eObject != null ? eObject.GetType() : type;

                EditorGuiFieldsResolver.DrawEObjectSlot(eObject, eObjectType, v =>
                {
                    setMemberValueCallBack(target, v, prop, index);
                    return true;
                });

                resultChanged = true;
            }
            else if (type == typeof(bool))
            {
                resultChanged = DrawSimpleProperty<bool>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawBoolField, setMemberValueCallBack);
            }
            else if (type == typeof(int))
            {
                resultChanged = DrawSimpleProperty<int>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawIntField, setMemberValueCallBack);
            }
            else if (type == typeof(uint))
            {
                resultChanged = DrawSimpleProperty<uint>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawUIntField, setMemberValueCallBack);
            }
            else if (type == typeof(long))
            {
                resultChanged = DrawSimpleProperty<long>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawLongField, setMemberValueCallBack);
            }
            else if (type == typeof(ulong))
            {
                resultChanged = DrawSimpleProperty<ulong>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawULongField, setMemberValueCallBack);
            }
            else if (type == typeof(Color))
            {
                resultChanged = DrawSimpleProperty<Color>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawColorField, setMemberValueCallBack);
            }
            else if (type == typeof(Color32))
            {
                resultChanged = DrawSimpleProperty<Color>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawColorField, setMemberValueCallBack, true, v => (Color32)v);
            }
            else if (type == typeof(float))
            {
                resultChanged = DrawSimpleProperty<float>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawFloatField, setMemberValueCallBack);
            }
            else if (type == typeof(double))
            {
                resultChanged = DrawSimpleProperty<double>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawDoubleField, setMemberValueCallBack);
            }
            else if (type == typeof(string))
            {
                resultChanged = DrawSimpleProperty<string>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawStringField, setMemberValueCallBack);
            }
            else if (type == typeof(quat))
            {
                var q = (quat)value;
                vec4 v = new vec4(q.x, q.y, q.z, q.w);
                resultChanged = DrawSimpleProperty<vec4>(propertyName, target, v, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawVec4Field, setMemberValueCallBack, true, v => value = new quat(v.x, v.y, v.z, v.w));
            }
            else if (type == typeof(vec2))
            {
                resultChanged = DrawSimpleProperty<vec2>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawVec2Field, setMemberValueCallBack);
            }
            else if (type == typeof(vec3))
            {
                resultChanged = DrawSimpleProperty<vec3>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawVec3Field, setMemberValueCallBack);
            }
            else if (type == typeof(vec4))
            {
                resultChanged = DrawSimpleProperty<vec4>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawVec4Field, setMemberValueCallBack);
            }
            else if (type == typeof(mat2))
            {
                resultChanged = DrawSimpleProperty<mat2>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawMatrix, setMemberValueCallBack);
            }
            else if (type == typeof(mat3))
            {
                resultChanged = DrawSimpleProperty<mat3>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawMatrix, setMemberValueCallBack);
            }
            else if (type == typeof(mat4))
            {
                resultChanged = DrawSimpleProperty<mat4>(propertyName, target, value, isReadOnly, prop, index, collectionData, width,
                    EditorGuiFieldsResolver.DrawMatrix, setMemberValueCallBack);
            }
            else if (type.IsEnum)
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

                int idx = (int)value;
                string[] names = Enum.GetNames(type);
                if (resultChanged = EditorGuiFieldsResolver.DrawCombo(propertyName, ref idx, names, width))
                {
                    setMemberValueCallBack(target, Enum.Parse(type, names[idx]), prop, index);
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

                resultChanged = DrawList(objectId, propertyName, list, value, elementType, prop,
                                         cursorX, OnAdd, OnRemove, OnRemoveCount, setMemberValueCallBack);
                setMemberValueCallBack(target, value, prop, index);

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

                resultChanged = DrawList(objectId, propertyName, array, value, elementType, prop,
                    cursorX, OnAdd, OnRemove, OnRemoveCount, setMemberValueCallBack);

                setMemberValueCallBack(target, value, prop, index);

                void OnAdd(IList list, int totalLength)
                {
                    var array = list as Array;

                    var copy = Array.CreateInstance(elementType, totalLength);
                    Array.Copy(array, copy, array.Length);
                    value = copy;
                    setMemberValueCallBack(target, value, prop, index);
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
                    setMemberValueCallBack(target, value, prop, index);
                }

                void OnRemoveCount(IList list, int totalLength)
                {
                    var array = list as Array;
                    var copy = Array.CreateInstance(elementType, totalLength);
                    Array.Copy(array, copy, totalLength);
                    value = copy;
                    setMemberValueCallBack(target, value, prop, index);
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                if (value == null)
                {
                    value = ReflectionUtils.GetDefaultValueInstance(type);
                }
                
                resultChanged = EditorGuiFieldsResolver.DrawDictionaryField(propertyName, value as IDictionary,
                    (Type type, string argName, bool isKey, object key, object val) =>
                {
                    var send = new KeyValuePair<object, object>(key, val);
                    var changed = DrawVars(propertyName, value, isKey ? key : val, type, argName, false, prop,
                        cursorX, index, send, (__target, __value, z, k, setValCallback) =>
                        {
                            if (__target is IDictionary)
                            {
                                if (isKey)
                                {
                                    send = new KeyValuePair<object, object>(__value, val);
                                }
                                else
                                {
                                    send = new KeyValuePair<object, object>(key, __value);
                                }
                            }
                            else
                            {
                                ReflectionUtils.SetMemberValueSafe(__target, __value, z, k, setValCallback);
                            }
                        }, width);

                    return (send, changed);
                });
            }
            else if (type.IsClass || ReflectionUtils.IsUserDefinedStruct(type))
            {
                var members = ReflectionUtils.GetAllMembersWithAttributes(type, _visibilityAttributes, true, true);
                var propIndex = index;

                foreach (var subProp in members)
                {
                    if (value != null)
                    {
                        var changed = DrawVars(objectId, value, subProp, cursorX, index,
                                               collectionData, width, setMemberValueCallBack, true);

                        if (changed)
                        {
                            resultChanged = true;
                        }

                        index++;
                    }
                }
                setMemberValueCallBack(target, value, prop, propIndex);
                DrawMethods(value, objectId);

            }

            ImGui.TreePop();

            return resultChanged;
        }


        private static bool DrawList(string objectId, string propertyName, IList list, object value, Type elemenType,
                                     MemberInfo prop, float cursorX, Action<IList, int> onAddCallback, Action<IList, int> onRemoveCallback,
                                     Action<IList, int> removeCount, SetMemberValueSafeCallBack setMemberCallback)
        {
            if (elemenType == null || !elemenType.IsGenericType)
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

                var listType = value.GetType();

                return EditorGuiFieldsResolver.DrawListField(propertyName, list, false, onAddCallback, onRemoveCallback, removeCount,
                        (index, itemWidth, item) =>
                        {
                            if (item == null)
                            {
                                item = ReflectionUtils.GetDefaultValueInstance(elemenType);
                            }

                            DrawVars(objectId, list, item, elemenType, $"##__{index}_item", false, prop, cursorX, index, index, setMemberCallback, itemWidth);

                            return false;
                        });
            }

            return false;
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
