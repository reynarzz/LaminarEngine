using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class LayerMask
    {
        private static ulong[] MaskBits;
        private static Dictionary<string, int> Names;
        private const int ARRAY_SIZE = sizeof(ulong) * 8;
        public const ulong All = ulong.MaxValue;

        static LayerMask()
        {
            MaskBits = new ulong[ARRAY_SIZE];
            Names = new Dictionary<string, int>(ARRAY_SIZE, StringComparer.OrdinalIgnoreCase);
        }

        internal static void UpdateLayers(bool[] matrix, string[] names)
        {
            if (names == null || matrix == null)
            {
                Debug.Error("Can't set layers, matrix or names are null");
                return;
            }
            if (names.Length == 0)
            {
                Debug.Error("No layer names are valid");
                return;
            }
            int count = names.Length;
            Names.Clear();
            if (names.Length > ARRAY_SIZE)
            {
                Debug.EngineError($"Layers count cannot be bigger than: {ARRAY_SIZE}");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                MaskBits[i] = 0;
                AssignName(i, names[i]);
            }

            int index = 0;

            for (int i = 0; i < count; i++)
            {
                for (int j = i; j < count; j++)
                {
                    bool enabled = matrix[index++];

                    if (enabled)
                    {
                        MaskBits[i] |= LayerToBits(j);
                        MaskBits[j] |= LayerToBits(i);
                    }
                }
            }
        }
        internal static bool[] BuildMatrixFromMasks()
        {
            int count = MaskBits.Length;

            int size = count * (count + 1) / 2;
            var matrix = new bool[size];

            int index = 0;

            for (int i = 0; i < count; i++)
            {
                for (int j = i; j < count; j++)
                {
                    matrix[index++] = (MaskBits[i] & LayerToBits(j)) != 0;
                }
            }

            return matrix;
        }
        internal static ulong GetMaskBits(int layer)
        {
            return MaskBits[layer];
        }

        internal static int LayerToIndex(ulong layer)
        {
            return BitOperations.TrailingZeroCount(layer);
        }

        public static ulong LayerToBits(int index)
        {
            return 1UL << index;
        }

        public static bool AreValid(int layer, ulong mask)
        {
            return (LayerToBits(layer) & mask) != 0;
        }

        internal static void TurnOff(int layerA, int layerB)
        {
            ModifyLayers(layerA, layerB, (mask, bit) => mask & ~bit);
        }

        public static void TurnOffAll(string name)
        {
            TurnOffAll(NameToLayer(name));
        }

        internal static void TurnOffAll(int layer)
        {
            var bit = LayerToBits(layer);

            for (int i = 0; i < MaskBits.Length; i++)
            {
                MaskBits[i] &= ~bit;
            }
        }

        internal static void TurnOn(int layerA, int layerB)
        {
            ModifyLayers(layerA, layerB, (mask, bit) => mask | bit);
        }

        internal static void TurnOnAll(int layer)
        {
            var bit = LayerToBits(layer);

            for (int i = 0; i < MaskBits.Length; i++)
            {
                MaskBits[i] |= bit;
            }
        }

        public static void TurnOff(string nameA, string nameB)
        {
            ModifyLayers(nameA, nameB, (mask, bit) => mask & ~bit);
        }

        public static void TurnOn(string nameA, string nameB)
        {
            ModifyLayers(nameA, nameB, (mask, bit) => mask | bit);
        }

        private static void ModifyLayers(string nameA, string nameB, Func<ulong, ulong, ulong> op)
        {
            ModifyLayers(NameToLayer(nameA), NameToLayer(nameB), op);
        }

        private static void ModifyLayers(int layerA, int layerB, Func<ulong, ulong, ulong> op)
        {
            MaskBits[layerA] = op(MaskBits[layerA], LayerToBits(layerB));
            MaskBits[layerB] = op(MaskBits[layerB], LayerToBits(layerA));
        }

        internal static bool AreEnabled(int layerA, int layerB)
        {
            return (MaskBits[layerA] & LayerToBits(layerB)) != 0; // Checking just one since both MaskBits are in sync.
        }

        private static void AssignName(int layer, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            if (Names.ContainsKey(name))
            {
                Names[name] = layer;
            }
            else
            {
                Names.Add(name, layer);
            }
        }

        public static List<KeyValuePair<string, int>> GetAllAssignedNames()
        {
            var validNames = new List<KeyValuePair<string, int>>();

            foreach (var kvp in Names)
            {
                if (!string.IsNullOrEmpty(kvp.Key))
                {
                    validNames.Add(new KeyValuePair<string, int>(kvp.Key, kvp.Value));
                }
            }

            return validNames;
        }

        public static string[] GetAllLayerNames()
        {
            int count = 0;
            foreach (var kvp in Names)
            {
                if (!string.IsNullOrEmpty(kvp.Key))
                {
                    count++;
                }
            }
            var validNames = new string[count];

            int index = 0;
            foreach (var kvp in Names)
            {
                if (!string.IsNullOrEmpty(kvp.Key))
                {
                    validNames[index++] = kvp.Key;
                }
            }

            return validNames;
        }

        internal static string LayerToName(int layer)
        {
            foreach (var kvp in Names)
            {
                if (kvp.Value == layer)
                    return kvp.Key;
            }
            return string.Empty;
        }

        public static int NameToLayer(string name)
        {
            if (Names.TryGetValue(name, out var layer))
            {
                return layer;
            }

            return 0;
        }

        public static ulong NameToBit(string name)
        {
            var layer = NameToLayer(name);

            if (layer >= 0)
            {
                return LayerToBits(layer);
            }

            return 0;
        }



    }
}