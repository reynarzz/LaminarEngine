using Editor.Utils;
using Engine.Serialization;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class ReferenceBinaryWriterFactory
    {
        private readonly Dictionary<SerializedType, Action<BinaryWriter, ReferenceData>> _customRefWriters;
        public ReferenceBinaryWriterFactory()
        {
            _customRefWriters = new()
            {
                // Custom
                { SerializedType.SpriteAsset, BaseWriter<SpriteReferenceData>(WriteSpriteAssetReference) },
            };
        }

        public void WriteReference(BinaryWriter writer, ReferenceData reference)
        {
            var type = reference?.Type ?? SerializedType.None;
            writer.Write((ulong)type);

            if (type == SerializedType.None)
            {
                return;
            }

            EditorFileUtils.WriteGuidNoAlloc(writer, reference.RefId);

            if (_customRefWriters.TryGetValue(reference.Type, out var writerFunction))
            {
                writerFunction(writer, reference);
            }
            else if (!reference.Type.IsDefaultRef())
            {
                // NOTE: if a new asset hit this, please add the asset type to the function's switch 'IsDefaultRef()'
                throw new NotImplementedException($"Writer for reference '{reference.Type}'is not implemented");
            }
        }

       
        private void WriteSpriteAssetReference(BinaryWriter writer, SpriteReferenceData reference)
        {
            writer.Write(reference.AtlasIndex);
            EditorFileUtils.WriteGuidNoAlloc(writer, reference.TexRefId);
        }

        private Action<BinaryWriter, ReferenceData> BaseWriter<T>(Action<BinaryWriter, T> writerFunction)
            where T : ReferenceData
        {
            return (writer, reference) =>
            {
                writerFunction(writer, reference as T);
            };
        }

        
    }
}
