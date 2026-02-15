using System;
using System.Collections.Generic;
using Engine;
using Engine.Serialization;
using GlmNet;
using Engine.Utils;

namespace Generated
{
    internal static class CollectionDeserializer
    {
        internal static object Deserialize1D(object collection, CollectionData data, SerializedType itemType, CollectionType kind)
        {
            ulong identity = itemType.GetIdentity();
            if (identity == SerializedType.Char.GetIdentity())
            {
                var charCollection = data as CollectionDataChar;
                if (kind == CollectionType.Array)
                    return ReadToArray_Char(collection, charCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_Char(collection, charCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Char(collection, charCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Char(collection, charCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Char(collection, charCollection);
            }

            if (identity == SerializedType.String.GetIdentity())
            {
                var stringCollection = data as CollectionDataString;
                if (kind == CollectionType.Array)
                    return ReadToArray_String(collection, stringCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_String(collection, stringCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_String(collection, stringCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_String(collection, stringCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_String(collection, stringCollection);
            }

            if (identity == SerializedType.Bool.GetIdentity())
            {
                var boolCollection = data as CollectionDataBool;
                if (kind == CollectionType.Array)
                    return ReadToArray_Bool(collection, boolCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_Bool(collection, boolCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Bool(collection, boolCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Bool(collection, boolCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Bool(collection, boolCollection);
            }

            if (identity == SerializedType.Byte.GetIdentity())
            {
                var byteCollection = data as CollectionDataByte;
                if (kind == CollectionType.Array)
                    return ReadToArray_Byte(collection, byteCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_Byte(collection, byteCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Byte(collection, byteCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Byte(collection, byteCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Byte(collection, byteCollection);
            }

            if (identity == SerializedType.Short.GetIdentity())
            {
                var shortCollection = data as CollectionDataShort;
                if (kind == CollectionType.Array)
                    return ReadToArray_Short(collection, shortCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_Short(collection, shortCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Short(collection, shortCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Short(collection, shortCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Short(collection, shortCollection);
            }

            if (identity == SerializedType.UShort.GetIdentity())
            {
                var ushortCollection = data as CollectionDataUShort;
                if (kind == CollectionType.Array)
                    return ReadToArray_UShort(collection, ushortCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_UShort(collection, ushortCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_UShort(collection, ushortCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_UShort(collection, ushortCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_UShort(collection, ushortCollection);
            }

            if (identity == SerializedType.Int.GetIdentity())
            {
                var intCollection = data as CollectionDataInt;
                if (kind == CollectionType.Array)
                    return ReadToArray_Int(collection, intCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_Int(collection, intCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Int(collection, intCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Int(collection, intCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Int(collection, intCollection);
            }

            if (identity == SerializedType.UInt.GetIdentity())
            {
                var uintCollection = data as CollectionDataUInt;
                if (kind == CollectionType.Array)
                    return ReadToArray_UInt(collection, uintCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_UInt(collection, uintCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_UInt(collection, uintCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_UInt(collection, uintCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_UInt(collection, uintCollection);
            }

            if (identity == SerializedType.Long.GetIdentity())
            {
                var longCollection = data as CollectionDataLong;
                if (kind == CollectionType.Array)
                    return ReadToArray_Long(collection, longCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_Long(collection, longCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Long(collection, longCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Long(collection, longCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Long(collection, longCollection);
            }

            if (identity == SerializedType.ULong.GetIdentity())
            {
                var ulongCollection = data as CollectionDataULong;
                if (kind == CollectionType.Array)
                    return ReadToArray_ULong(collection, ulongCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_ULong(collection, ulongCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_ULong(collection, ulongCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_ULong(collection, ulongCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_ULong(collection, ulongCollection);
            }

            if (identity == SerializedType.Float.GetIdentity())
            {
                var floatCollection = data as CollectionDataFloat;
                if (kind == CollectionType.Array)
                    return ReadToArray_Float(collection, floatCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_Float(collection, floatCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Float(collection, floatCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Float(collection, floatCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Float(collection, floatCollection);
            }

            if (identity == SerializedType.Double.GetIdentity())
            {
                var doubleCollection = data as CollectionDataDouble;
                if (kind == CollectionType.Array)
                    return ReadToArray_Double(collection, doubleCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_Double(collection, doubleCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Double(collection, doubleCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Double(collection, doubleCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Double(collection, doubleCollection);
            }

            if (identity == SerializedType.Vec2.GetIdentity())
            {
                var vec2Collection = data as CollectionDataVec2;
                if (kind == CollectionType.Array)
                    return ReadToArray_Vec2(collection, vec2Collection);
                else if (kind == CollectionType.List)
                    return ReadToList_Vec2(collection, vec2Collection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Vec2(collection, vec2Collection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Vec2(collection, vec2Collection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Vec2(collection, vec2Collection);
            }

            if (identity == SerializedType.Vec3.GetIdentity())
            {
                var vec3Collection = data as CollectionDataVec3;
                if (kind == CollectionType.Array)
                    return ReadToArray_Vec3(collection, vec3Collection);
                else if (kind == CollectionType.List)
                    return ReadToList_Vec3(collection, vec3Collection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Vec3(collection, vec3Collection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Vec3(collection, vec3Collection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Vec3(collection, vec3Collection);
            }

            if (identity == SerializedType.Vec4.GetIdentity())
            {
                var vec4Collection = data as CollectionDataVec4;
                if (kind == CollectionType.Array)
                    return ReadToArray_Vec4(collection, vec4Collection);
                else if (kind == CollectionType.List)
                    return ReadToList_Vec4(collection, vec4Collection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Vec4(collection, vec4Collection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Vec4(collection, vec4Collection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Vec4(collection, vec4Collection);
            }

            if (identity == SerializedType.IVec2.GetIdentity())
            {
                var ivec2Collection = data as CollectionDataIvec2;
                if (kind == CollectionType.Array)
                    return ReadToArray_IVec2(collection, ivec2Collection);
                else if (kind == CollectionType.List)
                    return ReadToList_IVec2(collection, ivec2Collection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_IVec2(collection, ivec2Collection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_IVec2(collection, ivec2Collection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_IVec2(collection, ivec2Collection);
            }

            if (identity == SerializedType.IVec3.GetIdentity())
            {
                var ivec3Collection = data as CollectionDataIvec3;
                if (kind == CollectionType.Array)
                    return ReadToArray_IVec3(collection, ivec3Collection);
                else if (kind == CollectionType.List)
                    return ReadToList_IVec3(collection, ivec3Collection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_IVec3(collection, ivec3Collection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_IVec3(collection, ivec3Collection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_IVec3(collection, ivec3Collection);
            }

            if (identity == SerializedType.IVec4.GetIdentity())
            {
                var ivec4Collection = data as CollectionDataIvec4;
                if (kind == CollectionType.Array)
                    return ReadToArray_IVec4(collection, ivec4Collection);
                else if (kind == CollectionType.List)
                    return ReadToList_IVec4(collection, ivec4Collection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_IVec4(collection, ivec4Collection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_IVec4(collection, ivec4Collection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_IVec4(collection, ivec4Collection);
            }

            if (identity == SerializedType.Quat.GetIdentity())
            {
                var quatCollection = data as CollectionDataQuat;
                if (kind == CollectionType.Array)
                    return ReadToArray_Quat(collection, quatCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_Quat(collection, quatCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Quat(collection, quatCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Quat(collection, quatCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Quat(collection, quatCollection);
            }

            if (identity == SerializedType.Mat2.GetIdentity())
            {
                var mat2Collection = data as CollectionDataMat2;
                if (kind == CollectionType.Array)
                    return ReadToArray_Mat2(collection, mat2Collection);
                else if (kind == CollectionType.List)
                    return ReadToList_Mat2(collection, mat2Collection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Mat2(collection, mat2Collection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Mat2(collection, mat2Collection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Mat2(collection, mat2Collection);
            }

            if (identity == SerializedType.Mat3.GetIdentity())
            {
                var mat3Collection = data as CollectionDataMat3;
                if (kind == CollectionType.Array)
                    return ReadToArray_Mat3(collection, mat3Collection);
                else if (kind == CollectionType.List)
                    return ReadToList_Mat3(collection, mat3Collection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Mat3(collection, mat3Collection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Mat3(collection, mat3Collection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Mat3(collection, mat3Collection);
            }

            if (identity == SerializedType.Mat4.GetIdentity())
            {
                var mat4Collection = data as CollectionDataMat4;
                if (kind == CollectionType.Array)
                    return ReadToArray_Mat4(collection, mat4Collection);
                else if (kind == CollectionType.List)
                    return ReadToList_Mat4(collection, mat4Collection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Mat4(collection, mat4Collection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Mat4(collection, mat4Collection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Mat4(collection, mat4Collection);
            }

            if (identity == SerializedType.Color.GetIdentity())
            {
                var ColorCollection = data as CollectionDataColor;
                if (kind == CollectionType.Array)
                    return ReadToArray_Color(collection, ColorCollection);
                else if (kind == CollectionType.List)
                    return ReadToList_Color(collection, ColorCollection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Color(collection, ColorCollection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Color(collection, ColorCollection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Color(collection, ColorCollection);
            }

            if (identity == SerializedType.Color32.GetIdentity())
            {
                var Color32Collection = data as CollectionDataColor32;
                if (kind == CollectionType.Array)
                    return ReadToArray_Color32(collection, Color32Collection);
                else if (kind == CollectionType.List)
                    return ReadToList_Color32(collection, Color32Collection);
                else if (kind == CollectionType.Queue)
                    return ReadToQueue_Color32(collection, Color32Collection);
                else if (kind == CollectionType.Stack)
                    return ReadToStack_Color32(collection, Color32Collection);
                else if (kind == CollectionType.HashSet)
                    return ReadToHashSet_Color32(collection, Color32Collection);
            }

            throw new NotSupportedException();
        }

        internal static object Deserialize(object collection, Variant[] keys, Variant[] values, SerializedType keyType, SerializedType valueType)
        {
            ulong keyId = keyType.GetIdentity();
            ulong valueId = valueType.GetIdentity();
            if (keyId == SerializedType.Char.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Char_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Char_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Char_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Char_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Char_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Char_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Char_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Char_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Char_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Char_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Char_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Char_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Char_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Char_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Char_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Char_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Char_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Char_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Char_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Char_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Char_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Char_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Char_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Char_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.String.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_String_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_String_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_String_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_String_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_String_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_String_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_String_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_String_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_String_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_String_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_String_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_String_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_String_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_String_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_String_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_String_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_String_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_String_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_String_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_String_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_String_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_String_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_String_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_String_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Bool.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Bool_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Bool_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Bool_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Bool_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Bool_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Bool_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Bool_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Bool_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Bool_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Bool_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Bool_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Bool_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Bool_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Bool_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Bool_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Bool_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Bool_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Bool_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Bool_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Bool_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Bool_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Bool_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Bool_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Bool_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Byte.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Byte_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Byte_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Byte_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Byte_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Byte_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Byte_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Byte_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Byte_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Byte_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Byte_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Byte_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Byte_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Byte_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Byte_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Byte_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Byte_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Byte_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Byte_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Byte_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Byte_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Byte_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Byte_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Byte_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Byte_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Short.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Short_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Short_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Short_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Short_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Short_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Short_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Short_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Short_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Short_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Short_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Short_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Short_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Short_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Short_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Short_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Short_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Short_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Short_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Short_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Short_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Short_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Short_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Short_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Short_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.UShort.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_UShort_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_UShort_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_UShort_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_UShort_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_UShort_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_UShort_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_UShort_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_UShort_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_UShort_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_UShort_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_UShort_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_UShort_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_UShort_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_UShort_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_UShort_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_UShort_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_UShort_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_UShort_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_UShort_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_UShort_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_UShort_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_UShort_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_UShort_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_UShort_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Int.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Int_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Int_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Int_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Int_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Int_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Int_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Int_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Int_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Int_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Int_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Int_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Int_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Int_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Int_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Int_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Int_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Int_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Int_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Int_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Int_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Int_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Int_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Int_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Int_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.UInt.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_UInt_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_UInt_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_UInt_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_UInt_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_UInt_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_UInt_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_UInt_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_UInt_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_UInt_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_UInt_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_UInt_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_UInt_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_UInt_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_UInt_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_UInt_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_UInt_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_UInt_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_UInt_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_UInt_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_UInt_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_UInt_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_UInt_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_UInt_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_UInt_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Long.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Long_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Long_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Long_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Long_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Long_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Long_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Long_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Long_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Long_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Long_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Long_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Long_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Long_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Long_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Long_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Long_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Long_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Long_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Long_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Long_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Long_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Long_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Long_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Long_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.ULong.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_ULong_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_ULong_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_ULong_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_ULong_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_ULong_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_ULong_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_ULong_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_ULong_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_ULong_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_ULong_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_ULong_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_ULong_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_ULong_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_ULong_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_ULong_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_ULong_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_ULong_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_ULong_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_ULong_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_ULong_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_ULong_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_ULong_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_ULong_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_ULong_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Float.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Float_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Float_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Float_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Float_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Float_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Float_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Float_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Float_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Float_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Float_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Float_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Float_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Float_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Float_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Float_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Float_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Float_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Float_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Float_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Float_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Float_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Float_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Float_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Float_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Double.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Double_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Double_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Double_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Double_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Double_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Double_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Double_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Double_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Double_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Double_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Double_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Double_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Double_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Double_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Double_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Double_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Double_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Double_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Double_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Double_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Double_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Double_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Double_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Double_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Vec2.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Vec2_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Vec2_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Vec2_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Vec2_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Vec2_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Vec2_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Vec2_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Vec2_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Vec2_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Vec2_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Vec2_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Vec2_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Vec2_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Vec2_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Vec2_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Vec2_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Vec2_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Vec2_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Vec2_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Vec2_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Vec2_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Vec2_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Vec2_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Vec2_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Vec3.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Vec3_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Vec3_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Vec3_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Vec3_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Vec3_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Vec3_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Vec3_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Vec3_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Vec3_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Vec3_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Vec3_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Vec3_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Vec3_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Vec3_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Vec3_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Vec3_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Vec3_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Vec3_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Vec3_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Vec3_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Vec3_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Vec3_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Vec3_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Vec3_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Vec4.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Vec4_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Vec4_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Vec4_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Vec4_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Vec4_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Vec4_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Vec4_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Vec4_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Vec4_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Vec4_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Vec4_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Vec4_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Vec4_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Vec4_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Vec4_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Vec4_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Vec4_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Vec4_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Vec4_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Vec4_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Vec4_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Vec4_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Vec4_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Vec4_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.IVec2.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_IVec2_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_IVec2_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_IVec2_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_IVec2_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_IVec2_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_IVec2_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_IVec2_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_IVec2_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_IVec2_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_IVec2_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_IVec2_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_IVec2_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_IVec2_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_IVec2_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_IVec2_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_IVec2_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_IVec2_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_IVec2_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_IVec2_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_IVec2_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_IVec2_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_IVec2_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_IVec2_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_IVec2_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.IVec3.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_IVec3_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_IVec3_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_IVec3_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_IVec3_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_IVec3_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_IVec3_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_IVec3_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_IVec3_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_IVec3_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_IVec3_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_IVec3_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_IVec3_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_IVec3_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_IVec3_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_IVec3_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_IVec3_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_IVec3_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_IVec3_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_IVec3_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_IVec3_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_IVec3_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_IVec3_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_IVec3_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_IVec3_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.IVec4.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_IVec4_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_IVec4_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_IVec4_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_IVec4_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_IVec4_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_IVec4_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_IVec4_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_IVec4_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_IVec4_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_IVec4_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_IVec4_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_IVec4_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_IVec4_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_IVec4_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_IVec4_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_IVec4_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_IVec4_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_IVec4_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_IVec4_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_IVec4_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_IVec4_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_IVec4_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_IVec4_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_IVec4_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Quat.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Quat_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Quat_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Quat_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Quat_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Quat_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Quat_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Quat_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Quat_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Quat_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Quat_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Quat_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Quat_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Quat_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Quat_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Quat_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Quat_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Quat_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Quat_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Quat_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Quat_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Quat_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Quat_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Quat_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Quat_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Mat2.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Mat2_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Mat2_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Mat2_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Mat2_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Mat2_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Mat2_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Mat2_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Mat2_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Mat2_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Mat2_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Mat2_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Mat2_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Mat2_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Mat2_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Mat2_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Mat2_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Mat2_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Mat2_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Mat2_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Mat2_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Mat2_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Mat2_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Mat2_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Mat2_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Mat3.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Mat3_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Mat3_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Mat3_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Mat3_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Mat3_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Mat3_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Mat3_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Mat3_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Mat3_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Mat3_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Mat3_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Mat3_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Mat3_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Mat3_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Mat3_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Mat3_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Mat3_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Mat3_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Mat3_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Mat3_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Mat3_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Mat3_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Mat3_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Mat3_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Mat4.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Mat4_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Mat4_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Mat4_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Mat4_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Mat4_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Mat4_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Mat4_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Mat4_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Mat4_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Mat4_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Mat4_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Mat4_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Mat4_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Mat4_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Mat4_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Mat4_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Mat4_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Mat4_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Mat4_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Mat4_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Mat4_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Mat4_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Mat4_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Mat4_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Color.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Color_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Color_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Color_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Color_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Color_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Color_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Color_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Color_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Color_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Color_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Color_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Color_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Color_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Color_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Color_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Color_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Color_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Color_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Color_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Color_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Color_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Color_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Color_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Color_Color32(collection, keys, values);
            }
            else if (keyId == SerializedType.Color32.GetIdentity())
            {
                if (valueId == SerializedType.Char.GetIdentity())
                    return ReadToDictionary_Color32_Char(collection, keys, values);
                else if (valueId == SerializedType.String.GetIdentity())
                    return ReadToDictionary_Color32_String(collection, keys, values);
                else if (valueId == SerializedType.Bool.GetIdentity())
                    return ReadToDictionary_Color32_Bool(collection, keys, values);
                else if (valueId == SerializedType.Byte.GetIdentity())
                    return ReadToDictionary_Color32_Byte(collection, keys, values);
                else if (valueId == SerializedType.Short.GetIdentity())
                    return ReadToDictionary_Color32_Short(collection, keys, values);
                else if (valueId == SerializedType.UShort.GetIdentity())
                    return ReadToDictionary_Color32_UShort(collection, keys, values);
                else if (valueId == SerializedType.Int.GetIdentity())
                    return ReadToDictionary_Color32_Int(collection, keys, values);
                else if (valueId == SerializedType.UInt.GetIdentity())
                    return ReadToDictionary_Color32_UInt(collection, keys, values);
                else if (valueId == SerializedType.Long.GetIdentity())
                    return ReadToDictionary_Color32_Long(collection, keys, values);
                else if (valueId == SerializedType.ULong.GetIdentity())
                    return ReadToDictionary_Color32_ULong(collection, keys, values);
                else if (valueId == SerializedType.Float.GetIdentity())
                    return ReadToDictionary_Color32_Float(collection, keys, values);
                else if (valueId == SerializedType.Double.GetIdentity())
                    return ReadToDictionary_Color32_Double(collection, keys, values);
                else if (valueId == SerializedType.Vec2.GetIdentity())
                    return ReadToDictionary_Color32_Vec2(collection, keys, values);
                else if (valueId == SerializedType.Vec3.GetIdentity())
                    return ReadToDictionary_Color32_Vec3(collection, keys, values);
                else if (valueId == SerializedType.Vec4.GetIdentity())
                    return ReadToDictionary_Color32_Vec4(collection, keys, values);
                else if (valueId == SerializedType.IVec2.GetIdentity())
                    return ReadToDictionary_Color32_IVec2(collection, keys, values);
                else if (valueId == SerializedType.IVec3.GetIdentity())
                    return ReadToDictionary_Color32_IVec3(collection, keys, values);
                else if (valueId == SerializedType.IVec4.GetIdentity())
                    return ReadToDictionary_Color32_IVec4(collection, keys, values);
                else if (valueId == SerializedType.Quat.GetIdentity())
                    return ReadToDictionary_Color32_Quat(collection, keys, values);
                else if (valueId == SerializedType.Mat2.GetIdentity())
                    return ReadToDictionary_Color32_Mat2(collection, keys, values);
                else if (valueId == SerializedType.Mat3.GetIdentity())
                    return ReadToDictionary_Color32_Mat3(collection, keys, values);
                else if (valueId == SerializedType.Mat4.GetIdentity())
                    return ReadToDictionary_Color32_Mat4(collection, keys, values);
                else if (valueId == SerializedType.Color.GetIdentity())
                    return ReadToDictionary_Color32_Color(collection, keys, values);
                else if (valueId == SerializedType.Color32.GetIdentity())
                    return ReadToDictionary_Color32_Color32(collection, keys, values);
            }

            throw new NotSupportedException();
        }

        private static object ReadToArray_Char(object collection, CollectionDataChar data)
        {
            var array = (char[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Char(object collection, CollectionDataChar data)
        {
            var list = (List<char>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Char(object collection, CollectionDataChar data)
        {
            var queue = (Queue<char>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Char(object collection, CollectionDataChar data)
        {
            var stack = (Stack<char>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Char(object collection, CollectionDataChar data)
        {
            var set = (HashSet<char>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_String(object collection, CollectionDataString data)
        {
            var array = (string[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_String(object collection, CollectionDataString data)
        {
            var list = (List<string>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_String(object collection, CollectionDataString data)
        {
            var queue = (Queue<string>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_String(object collection, CollectionDataString data)
        {
            var stack = (Stack<string>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_String(object collection, CollectionDataString data)
        {
            var set = (HashSet<string>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Bool(object collection, CollectionDataBool data)
        {
            var array = (bool[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Bool(object collection, CollectionDataBool data)
        {
            var list = (List<bool>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Bool(object collection, CollectionDataBool data)
        {
            var queue = (Queue<bool>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Bool(object collection, CollectionDataBool data)
        {
            var stack = (Stack<bool>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Bool(object collection, CollectionDataBool data)
        {
            var set = (HashSet<bool>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Byte(object collection, CollectionDataByte data)
        {
            var array = (byte[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Byte(object collection, CollectionDataByte data)
        {
            var list = (List<byte>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Byte(object collection, CollectionDataByte data)
        {
            var queue = (Queue<byte>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Byte(object collection, CollectionDataByte data)
        {
            var stack = (Stack<byte>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Byte(object collection, CollectionDataByte data)
        {
            var set = (HashSet<byte>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Short(object collection, CollectionDataShort data)
        {
            var array = (short[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Short(object collection, CollectionDataShort data)
        {
            var list = (List<short>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Short(object collection, CollectionDataShort data)
        {
            var queue = (Queue<short>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Short(object collection, CollectionDataShort data)
        {
            var stack = (Stack<short>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Short(object collection, CollectionDataShort data)
        {
            var set = (HashSet<short>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_UShort(object collection, CollectionDataUShort data)
        {
            var array = (ushort[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_UShort(object collection, CollectionDataUShort data)
        {
            var list = (List<ushort>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_UShort(object collection, CollectionDataUShort data)
        {
            var queue = (Queue<ushort>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_UShort(object collection, CollectionDataUShort data)
        {
            var stack = (Stack<ushort>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_UShort(object collection, CollectionDataUShort data)
        {
            var set = (HashSet<ushort>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Int(object collection, CollectionDataInt data)
        {
            var array = (int[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Int(object collection, CollectionDataInt data)
        {
            var list = (List<int>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Int(object collection, CollectionDataInt data)
        {
            var queue = (Queue<int>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Int(object collection, CollectionDataInt data)
        {
            var stack = (Stack<int>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Int(object collection, CollectionDataInt data)
        {
            var set = (HashSet<int>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_UInt(object collection, CollectionDataUInt data)
        {
            var array = (uint[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_UInt(object collection, CollectionDataUInt data)
        {
            var list = (List<uint>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_UInt(object collection, CollectionDataUInt data)
        {
            var queue = (Queue<uint>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_UInt(object collection, CollectionDataUInt data)
        {
            var stack = (Stack<uint>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_UInt(object collection, CollectionDataUInt data)
        {
            var set = (HashSet<uint>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Long(object collection, CollectionDataLong data)
        {
            var array = (long[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Long(object collection, CollectionDataLong data)
        {
            var list = (List<long>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Long(object collection, CollectionDataLong data)
        {
            var queue = (Queue<long>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Long(object collection, CollectionDataLong data)
        {
            var stack = (Stack<long>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Long(object collection, CollectionDataLong data)
        {
            var set = (HashSet<long>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_ULong(object collection, CollectionDataULong data)
        {
            var array = (ulong[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_ULong(object collection, CollectionDataULong data)
        {
            var list = (List<ulong>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_ULong(object collection, CollectionDataULong data)
        {
            var queue = (Queue<ulong>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_ULong(object collection, CollectionDataULong data)
        {
            var stack = (Stack<ulong>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_ULong(object collection, CollectionDataULong data)
        {
            var set = (HashSet<ulong>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Float(object collection, CollectionDataFloat data)
        {
            var array = (float[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Float(object collection, CollectionDataFloat data)
        {
            var list = (List<float>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Float(object collection, CollectionDataFloat data)
        {
            var queue = (Queue<float>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Float(object collection, CollectionDataFloat data)
        {
            var stack = (Stack<float>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Float(object collection, CollectionDataFloat data)
        {
            var set = (HashSet<float>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Double(object collection, CollectionDataDouble data)
        {
            var array = (double[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Double(object collection, CollectionDataDouble data)
        {
            var list = (List<double>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Double(object collection, CollectionDataDouble data)
        {
            var queue = (Queue<double>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Double(object collection, CollectionDataDouble data)
        {
            var stack = (Stack<double>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Double(object collection, CollectionDataDouble data)
        {
            var set = (HashSet<double>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Vec2(object collection, CollectionDataVec2 data)
        {
            var array = (vec2[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Vec2(object collection, CollectionDataVec2 data)
        {
            var list = (List<vec2>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Vec2(object collection, CollectionDataVec2 data)
        {
            var queue = (Queue<vec2>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Vec2(object collection, CollectionDataVec2 data)
        {
            var stack = (Stack<vec2>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Vec2(object collection, CollectionDataVec2 data)
        {
            var set = (HashSet<vec2>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Vec3(object collection, CollectionDataVec3 data)
        {
            var array = (vec3[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Vec3(object collection, CollectionDataVec3 data)
        {
            var list = (List<vec3>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Vec3(object collection, CollectionDataVec3 data)
        {
            var queue = (Queue<vec3>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Vec3(object collection, CollectionDataVec3 data)
        {
            var stack = (Stack<vec3>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Vec3(object collection, CollectionDataVec3 data)
        {
            var set = (HashSet<vec3>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Vec4(object collection, CollectionDataVec4 data)
        {
            var array = (vec4[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Vec4(object collection, CollectionDataVec4 data)
        {
            var list = (List<vec4>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Vec4(object collection, CollectionDataVec4 data)
        {
            var queue = (Queue<vec4>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Vec4(object collection, CollectionDataVec4 data)
        {
            var stack = (Stack<vec4>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Vec4(object collection, CollectionDataVec4 data)
        {
            var set = (HashSet<vec4>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_IVec2(object collection, CollectionDataIvec2 data)
        {
            var array = (ivec2[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_IVec2(object collection, CollectionDataIvec2 data)
        {
            var list = (List<ivec2>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_IVec2(object collection, CollectionDataIvec2 data)
        {
            var queue = (Queue<ivec2>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_IVec2(object collection, CollectionDataIvec2 data)
        {
            var stack = (Stack<ivec2>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_IVec2(object collection, CollectionDataIvec2 data)
        {
            var set = (HashSet<ivec2>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_IVec3(object collection, CollectionDataIvec3 data)
        {
            var array = (ivec3[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_IVec3(object collection, CollectionDataIvec3 data)
        {
            var list = (List<ivec3>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_IVec3(object collection, CollectionDataIvec3 data)
        {
            var queue = (Queue<ivec3>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_IVec3(object collection, CollectionDataIvec3 data)
        {
            var stack = (Stack<ivec3>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_IVec3(object collection, CollectionDataIvec3 data)
        {
            var set = (HashSet<ivec3>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_IVec4(object collection, CollectionDataIvec4 data)
        {
            var array = (ivec4[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_IVec4(object collection, CollectionDataIvec4 data)
        {
            var list = (List<ivec4>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_IVec4(object collection, CollectionDataIvec4 data)
        {
            var queue = (Queue<ivec4>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_IVec4(object collection, CollectionDataIvec4 data)
        {
            var stack = (Stack<ivec4>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_IVec4(object collection, CollectionDataIvec4 data)
        {
            var set = (HashSet<ivec4>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Quat(object collection, CollectionDataQuat data)
        {
            var array = (quat[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Quat(object collection, CollectionDataQuat data)
        {
            var list = (List<quat>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Quat(object collection, CollectionDataQuat data)
        {
            var queue = (Queue<quat>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Quat(object collection, CollectionDataQuat data)
        {
            var stack = (Stack<quat>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Quat(object collection, CollectionDataQuat data)
        {
            var set = (HashSet<quat>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Mat2(object collection, CollectionDataMat2 data)
        {
            var array = (mat2[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Mat2(object collection, CollectionDataMat2 data)
        {
            var list = (List<mat2>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Mat2(object collection, CollectionDataMat2 data)
        {
            var queue = (Queue<mat2>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Mat2(object collection, CollectionDataMat2 data)
        {
            var stack = (Stack<mat2>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Mat2(object collection, CollectionDataMat2 data)
        {
            var set = (HashSet<mat2>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Mat3(object collection, CollectionDataMat3 data)
        {
            var array = (mat3[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Mat3(object collection, CollectionDataMat3 data)
        {
            var list = (List<mat3>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Mat3(object collection, CollectionDataMat3 data)
        {
            var queue = (Queue<mat3>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Mat3(object collection, CollectionDataMat3 data)
        {
            var stack = (Stack<mat3>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Mat3(object collection, CollectionDataMat3 data)
        {
            var set = (HashSet<mat3>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Mat4(object collection, CollectionDataMat4 data)
        {
            var array = (mat4[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Mat4(object collection, CollectionDataMat4 data)
        {
            var list = (List<mat4>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Mat4(object collection, CollectionDataMat4 data)
        {
            var queue = (Queue<mat4>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Mat4(object collection, CollectionDataMat4 data)
        {
            var stack = (Stack<mat4>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Mat4(object collection, CollectionDataMat4 data)
        {
            var set = (HashSet<mat4>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Color(object collection, CollectionDataColor data)
        {
            var array = (Color[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Color(object collection, CollectionDataColor data)
        {
            var list = (List<Color>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Color(object collection, CollectionDataColor data)
        {
            var queue = (Queue<Color>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Color(object collection, CollectionDataColor data)
        {
            var stack = (Stack<Color>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Color(object collection, CollectionDataColor data)
        {
            var set = (HashSet<Color>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToArray_Color32(object collection, CollectionDataColor32 data)
        {
            var array = (Color32[])ReflectionUtils.EnsureCount(collection, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                array[i] = data.Value[i];
            return array;
        }

        private static object ReadToList_Color32(object collection, CollectionDataColor32 data)
        {
            var list = (List<Color32>)collection;
            System.Runtime.InteropServices.CollectionsMarshal.SetCount(list, data.Value.Length);
            for (int i = 0; i < data.Value.Length; i++)
                list[i] = data.Value[i];
            return list;
        }

        private static object ReadToQueue_Color32(object collection, CollectionDataColor32 data)
        {
            var queue = (Queue<Color32>)collection;
            queue.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                queue.Enqueue(data.Value[i]);
            return queue;
        }

        private static object ReadToStack_Color32(object collection, CollectionDataColor32 data)
        {
            var stack = (Stack<Color32>)collection;
            stack.Clear();
            for (int i = data.Value.Length - 1; i >= 0; i--)
                stack.Push(data.Value[i]);
            return stack;
        }

        private static object ReadToHashSet_Color32(object collection, CollectionDataColor32 data)
        {
            var set = (HashSet<Color32>)collection;
            set.Clear();
            for (int i = 0; i < data.Value.Length; i++)
                set.Add(data.Value[i]);
            return set;
        }

        private static object ReadToDictionary_Char_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Char_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Char_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Char_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Char_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Char_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Char_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Char_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Char_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Char_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Char_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Char_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Char_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Char_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Char_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Char_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Char_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Char_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Char_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Char_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Char_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Char_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Char_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Char_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<char, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Char, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_String_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_String_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_String_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_String_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_String_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_String_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_String_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_String_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_String_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_String_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_String_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_String_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_String_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_String_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_String_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_String_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_String_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_String_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_String_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_String_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_String_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_String_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_String_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_String_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<string, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].String, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Bool_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Bool_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Bool_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Bool_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Bool_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Bool_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Bool_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Bool_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Bool_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Bool_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Bool_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Bool_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Bool_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Bool_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Bool_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Bool_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Bool_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Bool_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Bool_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Bool_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Bool_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Bool_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Bool_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Bool_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<bool, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Bool, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Byte_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Byte_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Byte_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Byte_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Byte_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Byte_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Byte_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Byte_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Byte_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Byte_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Byte_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Byte_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Byte_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Byte_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Byte_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Byte_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Byte_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Byte_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Byte_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Byte_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Byte_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Byte_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Byte_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Byte_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<byte, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Byte, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Short_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Short_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Short_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Short_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Short_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Short_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Short_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Short_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Short_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Short_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Short_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Short_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Short_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Short_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Short_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Short_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Short_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Short_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Short_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Short_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Short_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Short_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Short_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Short_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<short, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Short, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_UShort_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_UShort_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_UShort_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_UShort_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_UShort_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_UShort_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_UShort_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_UShort_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_UShort_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_UShort_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_UShort_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_UShort_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_UShort_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_UShort_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_UShort_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_UShort_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_UShort_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_UShort_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_UShort_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_UShort_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_UShort_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_UShort_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_UShort_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_UShort_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ushort, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.UShort, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Int_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Int_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Int_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Int_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Int_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Int_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Int_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Int_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Int_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Int_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Int_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Int_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Int_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Int_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Int_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Int_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Int_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Int_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Int_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Int_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Int_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Int_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Int_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Int_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<int, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Int, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_UInt_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_UInt_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_UInt_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_UInt_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_UInt_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_UInt_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_UInt_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_UInt_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_UInt_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_UInt_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_UInt_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_UInt_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_UInt_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_UInt_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_UInt_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_UInt_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_UInt_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_UInt_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_UInt_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_UInt_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_UInt_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_UInt_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_UInt_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_UInt_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<uint, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Uint, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Long_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Long_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Long_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Long_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Long_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Long_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Long_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Long_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Long_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Long_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Long_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Long_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Long_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Long_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Long_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Long_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Long_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Long_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Long_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Long_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Long_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Long_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Long_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Long_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<long, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Long, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_ULong_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_ULong_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_ULong_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_ULong_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_ULong_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_ULong_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_ULong_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_ULong_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_ULong_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_ULong_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_ULong_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_ULong_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_ULong_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_ULong_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_ULong_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_ULong_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_ULong_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_ULong_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_ULong_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_ULong_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_ULong_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_ULong_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_ULong_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_ULong_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ulong, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ulong, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Float_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Float_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Float_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Float_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Float_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Float_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Float_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Float_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Float_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Float_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Float_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Float_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Float_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Float_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Float_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Float_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Float_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Float_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Float_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Float_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Float_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Float_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Float_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Float_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<float, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Float, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Double_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Double_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Double_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Double_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Double_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Double_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Double_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Double_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Double_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Double_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Double_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Double_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Double_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Double_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Double_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Double_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Double_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Double_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Double_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Double_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Double_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Double_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Double_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Double_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<double, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Double, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Vec2_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Vec2_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Vec2_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Vec2_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Vec2_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Vec2_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Vec2_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Vec2_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec2, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec2, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Vec3_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Vec3_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Vec3_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Vec3_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Vec3_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Vec3_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Vec3_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Vec3_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec3, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec3, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Vec4_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Vec4_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Vec4_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Vec4_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Vec4_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Vec4_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Vec4_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Vec4_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<vec4, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Vec4, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_IVec2_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_IVec2_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_IVec2_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_IVec2_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_IVec2_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_IVec2_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_IVec2_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_IVec2_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec2, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec2, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_IVec3_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_IVec3_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_IVec3_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_IVec3_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_IVec3_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_IVec3_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_IVec3_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_IVec3_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec3, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec3, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_IVec4_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_IVec4_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_IVec4_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_IVec4_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_IVec4_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_IVec4_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_IVec4_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_IVec4_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<ivec4, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Ivec4, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Quat_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Quat_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Quat_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Quat_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Quat_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Quat_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Quat_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Quat_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Quat_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Quat_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Quat_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Quat_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Quat_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Quat_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Quat_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Quat_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Quat_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Quat_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Quat_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Quat_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Quat_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Quat_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Quat_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Quat_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<quat, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Quat, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Mat2_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Mat2_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Mat2_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Mat2_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Mat2_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Mat2_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Mat2_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Mat2_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat2, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat2, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Mat3_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Mat3_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Mat3_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Mat3_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Mat3_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Mat3_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Mat3_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Mat3_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat3, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat3, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Mat4_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Mat4_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Mat4_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Mat4_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Mat4_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Mat4_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Mat4_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Mat4_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<mat4, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Mat4, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Color_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Color_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Color_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Color_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Color_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Color_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Color_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Color_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Color_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Color_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Color_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Color_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Color_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Color_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Color_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Color_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Color_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Color_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Color_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Color_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Color_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Color_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Color_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Color_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color, values[i].value.Color32);
            return dict;
        }

        private static object ReadToDictionary_Color32_Char(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, char>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Char);
            return dict;
        }

        private static object ReadToDictionary_Color32_String(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, string>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].String);
            return dict;
        }

        private static object ReadToDictionary_Color32_Bool(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, bool>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Bool);
            return dict;
        }

        private static object ReadToDictionary_Color32_Byte(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, byte>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Byte);
            return dict;
        }

        private static object ReadToDictionary_Color32_Short(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, short>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Short);
            return dict;
        }

        private static object ReadToDictionary_Color32_UShort(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, ushort>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.UShort);
            return dict;
        }

        private static object ReadToDictionary_Color32_Int(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, int>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Int);
            return dict;
        }

        private static object ReadToDictionary_Color32_UInt(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, uint>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Uint);
            return dict;
        }

        private static object ReadToDictionary_Color32_Long(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, long>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Long);
            return dict;
        }

        private static object ReadToDictionary_Color32_ULong(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, ulong>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Ulong);
            return dict;
        }

        private static object ReadToDictionary_Color32_Float(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, float>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Float);
            return dict;
        }

        private static object ReadToDictionary_Color32_Double(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, double>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Double);
            return dict;
        }

        private static object ReadToDictionary_Color32_Vec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, vec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Vec2);
            return dict;
        }

        private static object ReadToDictionary_Color32_Vec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, vec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Vec3);
            return dict;
        }

        private static object ReadToDictionary_Color32_Vec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, vec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Vec4);
            return dict;
        }

        private static object ReadToDictionary_Color32_IVec2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, ivec2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Ivec2);
            return dict;
        }

        private static object ReadToDictionary_Color32_IVec3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, ivec3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Ivec3);
            return dict;
        }

        private static object ReadToDictionary_Color32_IVec4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, ivec4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Ivec4);
            return dict;
        }

        private static object ReadToDictionary_Color32_Quat(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, quat>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Quat);
            return dict;
        }

        private static object ReadToDictionary_Color32_Mat2(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, mat2>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Mat2);
            return dict;
        }

        private static object ReadToDictionary_Color32_Mat3(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, mat3>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Mat3);
            return dict;
        }

        private static object ReadToDictionary_Color32_Mat4(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, mat4>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Mat4);
            return dict;
        }

        private static object ReadToDictionary_Color32_Color(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, Color>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Color);
            return dict;
        }

        private static object ReadToDictionary_Color32_Color32(object collection, Variant[] keys, Variant[] values)
        {
            var dict = (Dictionary<Color32, Color32>)collection;
            dict.Clear();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i].value.Color32, values[i].value.Color32);
            return dict;
        }
    }
}