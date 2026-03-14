using Editor.Drawers;
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
    internal class PropertiesGUIDrawEditor
    {
        private static object _copiedValue;

        private delegate bool DrawSimpleFieldDelegate<T>(string fieldName, ref T v, float width = 0, bool pressEnterToConfirm = false);
        private readonly static Type[] _visibilityAttributes = [typeof(SerializedFieldAttribute), typeof(ShowFieldNoSerialize)];
        public delegate void SetMemberValueSafeCallBack(object target, object value, MemberInfo prop, int index,
                                                        Func<object, object> valueConverter = null);


        private static bool DrawSimpleProperty<T>(string propertyName, object target, object value, bool isReadOnly,
                                                  MemberInfo prop, int index, float width, DrawSimpleFieldDelegate<T> drawField,
                                                  SetMemberValueSafeCallBack setValueCallback,
                                                  bool sameLine = true, Func<T, object> valueConverter = null)
        {
            if (target == null)
            {
                Debug.Error($"Can't draw property '{propertyName}', target is null.");
                return false;
            }

            if (sameLine)
            {
                ImGui.SameLine();
            }
            ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

            var v = (T)value;
            var result = false;

            if (drawField(propertyName, ref v, width, false))
            {
                if (valueConverter != null)
                {
                    setValueCallback(target, v, prop, index, x => valueConverter((T)x));
                }
                else
                {
                    setValueCallback(target, v, prop, index);
                }

                result = true;
            }

            return result;
        }

        public static bool DrawObject(string objectId, object target)
        {
            if(target == null)
            {
                Debug.Error($"Can't draw target with id: {objectId}, is null.");
                return false;
            }
            var members = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(target.GetType(), true, true);
            var anyChange = false;
            foreach (var member in members) 
            {
                var hasChanged = DrawVars(objectId, target, member, true);
                if (hasChanged)
                {
                    anyChange = hasChanged;
                }
            }
            return anyChange;
        }


        public static bool DrawVars(string objectId, object target, MemberInfo prop, bool enforceSerializedFieldAttribute = true)
        {
            if (target == null)
            {
                Debug.Error($"Can't draw property, target is null.");
                return false;
            }

            return DrawVars(objectId, target, prop, 0, 0, 0, enforceSerializedFieldAttribute);
        }

        public static bool DrawVars(string objectId, object target, MemberInfo prop, float cursorX, int index, float width,
                                     bool enforceSerializedFieldAttribute, SetMemberValueSafeCallBack setMemberCallBack = null)
        {
            if (target == null)
            {
                Debug.Error($"Can't draw property, target is null.");
                return false;
            }

            if (prop == null)
                return false;

            if (setMemberCallBack == null)
            {
                setMemberCallBack = ReflectionUtils.SetMemberValueSafe;
            }
            var value = ReflectionUtils.GetMemberValue(target, prop);
            var type = value != null ? value.GetType() : ReflectionUtils.GetMemberType(prop);

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

            bool changed = false;
            if (CustomEditorDatabase.TryGetCustomPropertyDrawer(target.GetType(), type, propertyName, out var customDrawer))
            {
                // Readonly will be automatically disabled for custom properties
                isReadOnly = false;

                changed = customDrawer.DrawProperty(type, propertyName, target, in value, out var valueOut, DefaultDrawVar);

                if (changed && valueOut != null)
                {
                    value = valueOut;
                    setMemberCallBack(target, value, prop, index);
                }
            }
            else
            {
                return DefaultDrawVar();
            }
            bool DefaultDrawVar()
            {
                return DrawVars(objectId, target, value, type, propertyName, isReadOnly, prop,
                             cursorX, index, width, setMemberCallBack);
            }
            return changed;
        }

        private static bool DrawVars(string objectId, object target, object value, Type type, string propertyName,
                                    bool isReadOnly, MemberInfo prop, float cursorX, int index, float width,
                                    SetMemberValueSafeCallBack setMemberValueCallBack)
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
            var spacing = ImGui.GetStyle().ItemSpacing;

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(spacing.X, 4));
            var show = ImGui.TreeNodeEx($"{propertyName}##{objectId}{index}", flags);
            ImGui.PopStyleVar();

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

                ImGui.BeginDisabled(isReadOnly);
                if (ImGui.Selectable("Paste") && _copiedValue?.GetType() == type)
                {
                    ReflectionUtils.SetMemberValue(target, prop, _copiedValue);
                }
                if (ImGui.Selectable("Clear"))
                {
                    var valueClear = type.IsValueType ? Activator.CreateInstance(type) : null;
                    ReflectionUtils.SetMemberValue(target, prop, valueClear);
                }
                ImGui.EndDisabled();

                ImGui.EndPopup();
            }
            ImGui.BeginDisabled(isReadOnly);

            bool resultChanged = false;
            // EObject
            if (ReflectionUtils.IsEObject(type))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

                var eObject = value as EObject;
                var eObjectType = eObject != null ? eObject.GetType() : type;
                EObject resultValue = eObject;

                /*resultChanged =*/ EditorGuiFieldsResolver.DrawEObjectSlot(eObject, eObjectType, v =>
                {
                    setMemberValueCallBack(target, v, prop, index);

                    resultChanged = eObject != v; // TODO: Remove once the refactor is DrawEObjectSlot is done.
                    return eObject != v;
                });
            }
            else if (type == typeof(bool))
            {
                resultChanged = DrawSimpleProperty<bool>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawBoolField, setMemberValueCallBack);
            }
            else if (type == typeof(int))
            {
                resultChanged = DrawSimpleProperty<int>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawIntField, setMemberValueCallBack);
            }
            else if (type == typeof(uint))
            {
                resultChanged = DrawSimpleProperty<uint>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawUIntField, setMemberValueCallBack);
            }
            else if (type == typeof(long))
            {
                resultChanged = DrawSimpleProperty<long>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawLongField, setMemberValueCallBack);
            }
            else if (type == typeof(ulong))
            {
                resultChanged = DrawSimpleProperty<ulong>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawULongField, setMemberValueCallBack);
            }
            else if (type == typeof(Color))
            {
                resultChanged = DrawSimpleProperty<Color>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawColorField, setMemberValueCallBack);
            }
            else if (type == typeof(Color32))
            {
                resultChanged = DrawSimpleProperty<Color32>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawColor32Field, setMemberValueCallBack, true);
            }
            else if (type == typeof(float))
            {
                resultChanged = DrawSimpleProperty<float>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawFloatField, setMemberValueCallBack);
            }
            else if (type == typeof(double))
            {
                resultChanged = DrawSimpleProperty<double>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawDoubleField, setMemberValueCallBack);
            }
            else if (type == typeof(string))
            {
                resultChanged = DrawSimpleProperty<string>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawStringField, setMemberValueCallBack);
            }
            else if (type == typeof(vec2))
            {
                resultChanged = DrawSimpleProperty<vec2>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawVec2Field, setMemberValueCallBack);
            }
            else if (type == typeof(ivec2))
            {
                resultChanged = DrawSimpleProperty<ivec2>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawIVec2Field, setMemberValueCallBack);
            }
            else if (type == typeof(vec3))
            {
                resultChanged = DrawSimpleProperty<vec3>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawVec3Field, setMemberValueCallBack);
            }
            else if (type == typeof(ivec3))
            {
                resultChanged = DrawSimpleProperty<ivec3>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawIVec3Field, setMemberValueCallBack);
            }
            else if (type == typeof(vec4))
            {
                resultChanged = DrawSimpleProperty<vec4>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawVec4Field, setMemberValueCallBack);
            }
            else if (type == typeof(quat))
            {
                resultChanged = DrawSimpleProperty<quat>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawQuatField, setMemberValueCallBack, true);
            }
            else if (type == typeof(mat2))
            {
                resultChanged = DrawSimpleProperty<mat2>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawMatrix, setMemberValueCallBack);
            }
            else if (type == typeof(mat3))
            {
                resultChanged = DrawSimpleProperty<mat3>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawMatrix, setMemberValueCallBack);
            }
            else if (type == typeof(mat4))
            {
                resultChanged = DrawSimpleProperty<mat4>(propertyName, target, value, isReadOnly, prop, index, width,
                    EditorGuiFieldsResolver.DrawMatrix, setMemberValueCallBack);
            }
            else if (type.IsEnum)
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

                var names = Enum.GetNames(type);
                var idx = Array.IndexOf(names, Enum.GetName(type, value));

                if (idx < 0)
                {
                    value = ReflectionUtils.GetDefaultValueInstance(type);
                    idx = 0;
                    setMemberValueCallBack(target, Enum.Parse(type, names[idx]), prop, index);
                }

                else if (resultChanged = EditorGuiFieldsResolver.DrawCombo(propertyName, ref idx, names, width))
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

                var elementType = type.GetGenericArguments().FirstOrDefault();

                resultChanged = DrawList(objectId, propertyName, value as IList, value, isReadOnly, elementType, prop,
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

                resultChanged = DrawList(objectId, propertyName, array, value, isReadOnly, elementType, prop,
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
                    (Type type, string argName, object argValue) =>
                {
                    object sendData = argValue;
                    var changed = DrawVars(propertyName, value, argValue, type, argName, isReadOnly, prop,
                        cursorX, index, width, (__target, __value, property, idx, valueConverter) =>
                        {
                            if (__target is IDictionary)
                            {
                                sendData = __value;
                            }
                            else
                            {
                                ReflectionUtils.SetMemberValueSafe(__target, __value, property, idx, valueConverter);
                            }
                        });

                    return (sendData, changed);
                });

                if (resultChanged)
                {
                    setMemberValueCallBack(target, value, prop, index);
                }
            }
            else if (type.IsClass || ReflectionUtils.IsUserDefinedStruct(type))
            {
                var members = ReflectionUtils.GetAllMembersWithAttributes(type, _visibilityAttributes, true, true);
                var propIndex = index;

                foreach (var subProp in members)
                {
                    if (value != null)
                    {
                        var changed = DrawVars(objectId, value, subProp, cursorX, index, width, true, setMemberValueCallBack);
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
            ImGui.EndDisabled();
            ImGui.TreePop();

            return resultChanged;
        }


        private static bool DrawList(string objectId, string propertyName, IList list, object value, bool isReadOnly, Type elementType,
                                     MemberInfo prop, float cursorX, Action<IList, int> onAddCallback,
                                     Action<IList, int> onRemoveCallback, Action<IList, int> removeCount,
                                     SetMemberValueSafeCallBack setMemberCallback)
        {
            if (elementType == null || !elementType.IsGenericType || (elementType.IsGenericType && !ReflectionUtils.IsCollection(elementType)))
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(Math.Max(EditorGuiFieldsResolver.XPosOffset, ImGui.GetCursorPosX()));

                return EditorGuiFieldsResolver.DrawListField(propertyName, list, false, onAddCallback, onRemoveCallback, removeCount,
                        (index, itemWidth, item) =>
                        {
                            if (item == null)
                            {
                                item = ReflectionUtils.GetDefaultValueInstance(elementType);
                            }

                            return DrawVars(objectId, list, item, item != null ? item.GetType() : elementType, $"##__{index}_item",
                                            isReadOnly, prop, cursorX, index, itemWidth, setMemberCallback);
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
