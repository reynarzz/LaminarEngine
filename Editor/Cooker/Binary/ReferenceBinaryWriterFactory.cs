using Editor.Utils;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class ReferenceBinaryWriterFactory
    {
        private readonly Dictionary<SerializedType, Action<BinaryWriter, ReferenceData>> _referenceWriters;
        public ReferenceBinaryWriterFactory()
        {
            _referenceWriters = new()
            {
                // Defaults
                { SerializedType.Actor, WriteDefaultReference },
                { SerializedType.Component, WriteDefaultReference },
                { SerializedType.TextureAsset, WriteDefaultReference },
                { SerializedType.MaterialAsset, WriteDefaultReference },
                { SerializedType.ShaderAsset, WriteDefaultReference },
                { SerializedType.AudioClipAsset, WriteDefaultReference },
                { SerializedType.AnimationAsset, WriteDefaultReference },
                { SerializedType.AnimatorControllerAsset, WriteDefaultReference },
                { SerializedType.RenderTextureAsset, WriteDefaultReference },
                { SerializedType.ScriptableObject, WriteDefaultReference },

                // Custom
                { SerializedType.SpriteAsset, BaseWriter<SpriteReferenceData>(WriteSpriteAssetReference) },
            };
        }

        public void WriteReference(BinaryWriter writer, ReferenceData reference)
        {
            if (_referenceWriters.TryGetValue(reference.Type, out var writerFunction))
            {
                writerFunction(writer, reference);
            }
            else
            {
                throw new NotImplementedException($"Writer for reference '{reference.Type}'is not implemented");
            }
        }

        private void WriteSpriteAssetReference(BinaryWriter writer, SpriteReferenceData reference)
        {
            writer.Write(reference.AtlasIndex);
            EditorUtils.WriteGuidNoAlloc(writer, reference.TextureId);
        }

        private Action<BinaryWriter, ReferenceData> BaseWriter<T>(Action<BinaryWriter, T> writerFunction)
            where T : ReferenceData
        {
            return (writer, reference) =>
            {
                if (WriteReferenceHeader(writer, reference))
                {
                    writerFunction(writer, reference as T);
                }
            };
        }

        private void WriteDefaultReference(BinaryWriter writer, ReferenceData reference)
        {
            WriteReferenceHeader(writer, reference);
        }

        private bool WriteReferenceHeader(BinaryWriter writer, ReferenceData value)
        {
            var hasData = value != null;
            if (hasData)
            {
                writer.Write((ulong)value.Type);
                EditorUtils.WriteGuidNoAlloc(writer, value.Id);
            }
            else
            {
                writer.Write((ulong)SerializedType.None);
            }

            return hasData;
        }
    }
}
