using System;
using System.Collections.Generic;
using Engine;
using Engine.Serialization;
using GlmNet;
using Engine.Utils;

namespace Generated
{
    internal static class VariantCollectionWriter
    {
        internal static object Write(object collection, VariantIRValue[] values, SerializedType itemType, CollectionType kind)
        {
            switch (itemType)
            {
                case SerializedType.Char:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Char(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Char(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Char(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Char(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Char(collection, values);
                    }

                    break;
                case SerializedType.Bool:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Bool(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Bool(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Bool(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Bool(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Bool(collection, values);
                    }

                    break;
                case SerializedType.Byte:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Byte(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Byte(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Byte(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Byte(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Byte(collection, values);
                    }

                    break;
                case SerializedType.Short:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Short(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Short(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Short(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Short(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Short(collection, values);
                    }

                    break;
                case SerializedType.UShort:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_UShort(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_UShort(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_UShort(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_UShort(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_UShort(collection, values);
                    }

                    break;
                case SerializedType.Int:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Int(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Int(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Int(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Int(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Int(collection, values);
                    }

                    break;
                case SerializedType.UInt:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_UInt(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_UInt(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_UInt(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_UInt(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_UInt(collection, values);
                    }

                    break;
                case SerializedType.Long:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Long(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Long(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Long(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Long(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Long(collection, values);
                    }

                    break;
                case SerializedType.ULong:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_ULong(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_ULong(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_ULong(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_ULong(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_ULong(collection, values);
                    }

                    break;
                case SerializedType.Float:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Float(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Float(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Float(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Float(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Float(collection, values);
                    }

                    break;
                case SerializedType.Double:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Double(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Double(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Double(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Double(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Double(collection, values);
                    }

                    break;
                case SerializedType.Vec2:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Vec2(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Vec2(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Vec2(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Vec2(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Vec2(collection, values);
                    }

                    break;
                case SerializedType.Vec3:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Vec3(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Vec3(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Vec3(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Vec3(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Vec3(collection, values);
                    }

                    break;
                case SerializedType.Vec4:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Vec4(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Vec4(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Vec4(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Vec4(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Vec4(collection, values);
                    }

                    break;
                case SerializedType.IVec2:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_IVec2(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_IVec2(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_IVec2(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_IVec2(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_IVec2(collection, values);
                    }

                    break;
                case SerializedType.IVec3:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_IVec3(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_IVec3(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_IVec3(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_IVec3(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_IVec3(collection, values);
                    }

                    break;
                case SerializedType.IVec4:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_IVec4(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_IVec4(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_IVec4(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_IVec4(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_IVec4(collection, values);
                    }

                    break;
                case SerializedType.Quat:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Quat(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Quat(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Quat(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Quat(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Quat(collection, values);
                    }

                    break;
                case SerializedType.Mat2:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Mat2(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Mat2(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Mat2(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Mat2(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Mat2(collection, values);
                    }

                    break;
                case SerializedType.Mat3:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Mat3(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Mat3(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Mat3(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Mat3(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Mat3(collection, values);
                    }

                    break;
                case SerializedType.Mat4:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Mat4(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Mat4(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Mat4(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Mat4(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Mat4(collection, values);
                    }

                    break;
                case SerializedType.Color:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Color(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Color(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Color(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Color(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Color(collection, values);
                    }

                    break;
                case SerializedType.Color32:
                    switch (kind)
                    {
                        case CollectionType.Array:
                            return VariantArrayToArray_Color32(collection, values);
                        case CollectionType.List:
                            return VariantArrayToList_Color32(collection, values);
                        case CollectionType.Queue:
                            return VariantArrayToQueue_Color32(collection, values);
                        case CollectionType.Stack:
                            return VariantArrayToStack_Color32(collection, values);
                        case CollectionType.HashSet:
                            return VariantArrayToHashSet_Color32(collection, values);
                    }

                    break;
            }

            throw new NotSupportedException();
        }

        private static object VariantArrayToArray_Char(object collection, VariantIRValue[] values)
        {
            var array = (char[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Char;
            }

            return array;
        }

        private static object VariantArrayToList_Char(object collection, VariantIRValue[] values)
        {
            var list = (List<char>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<char> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Char;
            }

            return list;
        }

        private static object VariantArrayToQueue_Char(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<char>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Char);
            }

            return queue;
        }

        private static object VariantArrayToStack_Char(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<char>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Char);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Char(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<char>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Char);
            }

            return set;
        }

        private static object VariantArrayToArray_Bool(object collection, VariantIRValue[] values)
        {
            var array = (bool[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Bool;
            }

            return array;
        }

        private static object VariantArrayToList_Bool(object collection, VariantIRValue[] values)
        {
            var list = (List<bool>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<bool> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Bool;
            }

            return list;
        }

        private static object VariantArrayToQueue_Bool(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<bool>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Bool);
            }

            return queue;
        }

        private static object VariantArrayToStack_Bool(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<bool>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Bool);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Bool(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<bool>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Bool);
            }

            return set;
        }

        private static object VariantArrayToArray_Byte(object collection, VariantIRValue[] values)
        {
            var array = (byte[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Byte;
            }

            return array;
        }

        private static object VariantArrayToList_Byte(object collection, VariantIRValue[] values)
        {
            var list = (List<byte>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<byte> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Byte;
            }

            return list;
        }

        private static object VariantArrayToQueue_Byte(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<byte>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Byte);
            }

            return queue;
        }

        private static object VariantArrayToStack_Byte(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<byte>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Byte);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Byte(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<byte>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Byte);
            }

            return set;
        }

        private static object VariantArrayToArray_Short(object collection, VariantIRValue[] values)
        {
            var array = (short[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Short;
            }

            return array;
        }

        private static object VariantArrayToList_Short(object collection, VariantIRValue[] values)
        {
            var list = (List<short>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<short> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Short;
            }

            return list;
        }

        private static object VariantArrayToQueue_Short(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<short>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Short);
            }

            return queue;
        }

        private static object VariantArrayToStack_Short(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<short>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Short);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Short(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<short>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Short);
            }

            return set;
        }

        private static object VariantArrayToArray_UShort(object collection, VariantIRValue[] values)
        {
            var array = (ushort[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.UShort;
            }

            return array;
        }

        private static object VariantArrayToList_UShort(object collection, VariantIRValue[] values)
        {
            var list = (List<ushort>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<ushort> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.UShort;
            }

            return list;
        }

        private static object VariantArrayToQueue_UShort(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<ushort>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.UShort);
            }

            return queue;
        }

        private static object VariantArrayToStack_UShort(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<ushort>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.UShort);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_UShort(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<ushort>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.UShort);
            }

            return set;
        }

        private static object VariantArrayToArray_Int(object collection, VariantIRValue[] values)
        {
            var array = (int[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Int;
            }

            return array;
        }

        private static object VariantArrayToList_Int(object collection, VariantIRValue[] values)
        {
            var list = (List<int>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<int> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Int;
            }

            return list;
        }

        private static object VariantArrayToQueue_Int(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<int>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Int);
            }

            return queue;
        }

        private static object VariantArrayToStack_Int(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<int>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Int);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Int(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<int>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Int);
            }

            return set;
        }

        private static object VariantArrayToArray_UInt(object collection, VariantIRValue[] values)
        {
            var array = (uint[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Uint;
            }

            return array;
        }

        private static object VariantArrayToList_UInt(object collection, VariantIRValue[] values)
        {
            var list = (List<uint>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<uint> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Uint;
            }

            return list;
        }

        private static object VariantArrayToQueue_UInt(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<uint>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Uint);
            }

            return queue;
        }

        private static object VariantArrayToStack_UInt(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<uint>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Uint);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_UInt(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<uint>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Uint);
            }

            return set;
        }

        private static object VariantArrayToArray_Long(object collection, VariantIRValue[] values)
        {
            var array = (long[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Long;
            }

            return array;
        }

        private static object VariantArrayToList_Long(object collection, VariantIRValue[] values)
        {
            var list = (List<long>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<long> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Long;
            }

            return list;
        }

        private static object VariantArrayToQueue_Long(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<long>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Long);
            }

            return queue;
        }

        private static object VariantArrayToStack_Long(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<long>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Long);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Long(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<long>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Long);
            }

            return set;
        }

        private static object VariantArrayToArray_ULong(object collection, VariantIRValue[] values)
        {
            var array = (ulong[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Ulong;
            }

            return array;
        }

        private static object VariantArrayToList_ULong(object collection, VariantIRValue[] values)
        {
            var list = (List<ulong>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<ulong> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Ulong;
            }

            return list;
        }

        private static object VariantArrayToQueue_ULong(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<ulong>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Ulong);
            }

            return queue;
        }

        private static object VariantArrayToStack_ULong(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<ulong>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Ulong);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_ULong(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<ulong>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Ulong);
            }

            return set;
        }

        private static object VariantArrayToArray_Float(object collection, VariantIRValue[] values)
        {
            var array = (float[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Float;
            }

            return array;
        }

        private static object VariantArrayToList_Float(object collection, VariantIRValue[] values)
        {
            var list = (List<float>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<float> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Float;
            }

            return list;
        }

        private static object VariantArrayToQueue_Float(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<float>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Float);
            }

            return queue;
        }

        private static object VariantArrayToStack_Float(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<float>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Float);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Float(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<float>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Float);
            }

            return set;
        }

        private static object VariantArrayToArray_Double(object collection, VariantIRValue[] values)
        {
            var array = (double[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Double;
            }

            return array;
        }

        private static object VariantArrayToList_Double(object collection, VariantIRValue[] values)
        {
            var list = (List<double>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<double> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Double;
            }

            return list;
        }

        private static object VariantArrayToQueue_Double(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<double>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Double);
            }

            return queue;
        }

        private static object VariantArrayToStack_Double(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<double>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Double);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Double(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<double>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Double);
            }

            return set;
        }

        private static object VariantArrayToArray_Vec2(object collection, VariantIRValue[] values)
        {
            var array = (vec2[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Vec2;
            }

            return array;
        }

        private static object VariantArrayToList_Vec2(object collection, VariantIRValue[] values)
        {
            var list = (List<vec2>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<vec2> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Vec2;
            }

            return list;
        }

        private static object VariantArrayToQueue_Vec2(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<vec2>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Vec2);
            }

            return queue;
        }

        private static object VariantArrayToStack_Vec2(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<vec2>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Vec2);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Vec2(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<vec2>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Vec2);
            }

            return set;
        }

        private static object VariantArrayToArray_Vec3(object collection, VariantIRValue[] values)
        {
            var array = (vec3[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Vec3;
            }

            return array;
        }

        private static object VariantArrayToList_Vec3(object collection, VariantIRValue[] values)
        {
            var list = (List<vec3>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<vec3> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Vec3;
            }

            return list;
        }

        private static object VariantArrayToQueue_Vec3(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<vec3>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Vec3);
            }

            return queue;
        }

        private static object VariantArrayToStack_Vec3(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<vec3>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Vec3);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Vec3(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<vec3>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Vec3);
            }

            return set;
        }

        private static object VariantArrayToArray_Vec4(object collection, VariantIRValue[] values)
        {
            var array = (vec4[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Vec4;
            }

            return array;
        }

        private static object VariantArrayToList_Vec4(object collection, VariantIRValue[] values)
        {
            var list = (List<vec4>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<vec4> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Vec4;
            }

            return list;
        }

        private static object VariantArrayToQueue_Vec4(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<vec4>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Vec4);
            }

            return queue;
        }

        private static object VariantArrayToStack_Vec4(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<vec4>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Vec4);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Vec4(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<vec4>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Vec4);
            }

            return set;
        }

        private static object VariantArrayToArray_IVec2(object collection, VariantIRValue[] values)
        {
            var array = (ivec2[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Ivec2;
            }

            return array;
        }

        private static object VariantArrayToList_IVec2(object collection, VariantIRValue[] values)
        {
            var list = (List<ivec2>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<ivec2> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Ivec2;
            }

            return list;
        }

        private static object VariantArrayToQueue_IVec2(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<ivec2>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Ivec2);
            }

            return queue;
        }

        private static object VariantArrayToStack_IVec2(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<ivec2>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Ivec2);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_IVec2(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<ivec2>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Ivec2);
            }

            return set;
        }

        private static object VariantArrayToArray_IVec3(object collection, VariantIRValue[] values)
        {
            var array = (ivec3[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Ivec3;
            }

            return array;
        }

        private static object VariantArrayToList_IVec3(object collection, VariantIRValue[] values)
        {
            var list = (List<ivec3>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<ivec3> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Ivec3;
            }

            return list;
        }

        private static object VariantArrayToQueue_IVec3(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<ivec3>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Ivec3);
            }

            return queue;
        }

        private static object VariantArrayToStack_IVec3(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<ivec3>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Ivec3);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_IVec3(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<ivec3>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Ivec3);
            }

            return set;
        }

        private static object VariantArrayToArray_IVec4(object collection, VariantIRValue[] values)
        {
            var array = (ivec4[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Ivec4;
            }

            return array;
        }

        private static object VariantArrayToList_IVec4(object collection, VariantIRValue[] values)
        {
            var list = (List<ivec4>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<ivec4> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Ivec4;
            }

            return list;
        }

        private static object VariantArrayToQueue_IVec4(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<ivec4>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Ivec4);
            }

            return queue;
        }

        private static object VariantArrayToStack_IVec4(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<ivec4>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Ivec4);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_IVec4(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<ivec4>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Ivec4);
            }

            return set;
        }

        private static object VariantArrayToArray_Quat(object collection, VariantIRValue[] values)
        {
            var array = (quat[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Quat;
            }

            return array;
        }

        private static object VariantArrayToList_Quat(object collection, VariantIRValue[] values)
        {
            var list = (List<quat>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<quat> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Quat;
            }

            return list;
        }

        private static object VariantArrayToQueue_Quat(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<quat>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Quat);
            }

            return queue;
        }

        private static object VariantArrayToStack_Quat(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<quat>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Quat);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Quat(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<quat>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Quat);
            }

            return set;
        }

        private static object VariantArrayToArray_Mat2(object collection, VariantIRValue[] values)
        {
            var array = (mat2[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Mat2;
            }

            return array;
        }

        private static object VariantArrayToList_Mat2(object collection, VariantIRValue[] values)
        {
            var list = (List<mat2>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<mat2> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Mat2;
            }

            return list;
        }

        private static object VariantArrayToQueue_Mat2(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<mat2>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Mat2);
            }

            return queue;
        }

        private static object VariantArrayToStack_Mat2(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<mat2>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Mat2);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Mat2(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<mat2>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Mat2);
            }

            return set;
        }

        private static object VariantArrayToArray_Mat3(object collection, VariantIRValue[] values)
        {
            var array = (mat3[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Mat3;
            }

            return array;
        }

        private static object VariantArrayToList_Mat3(object collection, VariantIRValue[] values)
        {
            var list = (List<mat3>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<mat3> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Mat3;
            }

            return list;
        }

        private static object VariantArrayToQueue_Mat3(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<mat3>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Mat3);
            }

            return queue;
        }

        private static object VariantArrayToStack_Mat3(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<mat3>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Mat3);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Mat3(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<mat3>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Mat3);
            }

            return set;
        }

        private static object VariantArrayToArray_Mat4(object collection, VariantIRValue[] values)
        {
            var array = (mat4[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Mat4;
            }

            return array;
        }

        private static object VariantArrayToList_Mat4(object collection, VariantIRValue[] values)
        {
            var list = (List<mat4>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<mat4> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Mat4;
            }

            return list;
        }

        private static object VariantArrayToQueue_Mat4(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<mat4>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Mat4);
            }

            return queue;
        }

        private static object VariantArrayToStack_Mat4(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<mat4>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Mat4);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Mat4(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<mat4>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Mat4);
            }

            return set;
        }

        private static object VariantArrayToArray_Color(object collection, VariantIRValue[] values)
        {
            var array = (Color[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Color;
            }

            return array;
        }

        private static object VariantArrayToList_Color(object collection, VariantIRValue[] values)
        {
            var list = (List<Color>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<Color> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Color;
            }

            return list;
        }

        private static object VariantArrayToQueue_Color(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<Color>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Color);
            }

            return queue;
        }

        private static object VariantArrayToStack_Color(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<Color>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Color);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Color(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<Color>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Color);
            }

            return set;
        }

        private static object VariantArrayToArray_Color32(object collection, VariantIRValue[] values)
        {
            var array = (Color32[])ReflectionUtils.EnsureCount(collection, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = values[i].Payload.Color32;
            }

            return array;
        }

        private static object VariantArrayToList_Color32(object collection, VariantIRValue[] values)
        {
            var list = (List<Color32>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, values.Length);
            Span<Color32> span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            for (int i = 0; i < values.Length; i++)
            {
                list[i] = values[i].Payload.Color32;
            }

            return list;
        }

        private static object VariantArrayToQueue_Color32(object collection, VariantIRValue[] values)
        {
            var queue = (Queue<Color32>)collection;
            queue.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                queue.Enqueue(values[i].Payload.Color32);
            }

            return queue;
        }

        private static object VariantArrayToStack_Color32(object collection, VariantIRValue[] values)
        {
            var stack = (Stack<Color32>)collection;
            stack.Clear();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                stack.Push(values[i].Payload.Color32);
            }

            return stack;
        }

        private static object VariantArrayToHashSet_Color32(object collection, VariantIRValue[] values)
        {
            var set = (HashSet<Color32>)collection;
            set.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                set.Add(values[i].Payload.Color32);
            }

            return set;
        }
    }
}