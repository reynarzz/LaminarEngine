using Engine.Serialization;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    internal class ReferenceBinaryReaderFactory
    {
        private readonly Dictionary<SerializedType, Func<BinaryReader, SerializedType, ReferenceData>> _referenceReaders;
        public ReferenceBinaryReaderFactory()
        {
            _referenceReaders = new()
            {
                // Custom
                { SerializedType.SpriteAsset, BaseReader<SpriteReferenceData>(ReadSpriteAssetReference) },
            };
        }

        public ReferenceData ReadReference(BinaryReader reader)
        {
            var type = (SerializedType)reader.ReadUInt64();

            if (type == SerializedType.None)
                return null;

            if (_referenceReaders.TryGetValue(type, out var readerFunc))
            {
                return readerFunc(reader, type);
            }

            if (type.IsDefaultRef())
            {
                return ReadDefaultReference(reader, type);
            }

            throw new InvalidDataException($"Type {type} is not a valid referenceable type.");
        }

        private void ReadSpriteAssetReference(BinaryReader reader, SpriteReferenceData reference)
        {
            reference.AtlasIndex = reader.ReadInt32();
            reference.TexRefId = EngineFileUtils.ReadGuidNoAlloc(reader);
        }

        private Func<BinaryReader, SerializedType, ReferenceData> BaseReader<T>(Action<BinaryReader, T> readFunction)
            where T : ReferenceData, new()
        {
            return (reader, type) =>
            {
                var reference = ReaderReferenceHeader<T>(reader, type);
                readFunction(reader, reference as T);
                return reference;
            };
        }
        private ReferenceData ReadDefaultReference(BinaryReader reader, SerializedType type)
        {
            return ReaderReferenceHeader<ReferenceData>(reader, type);
        }
        private ReferenceData ReaderReferenceHeader<T>(BinaryReader reader, SerializedType type)
            where T : ReferenceData, new()
        {
            return new T()
            {
                Type = type,
                RefId = EngineFileUtils.ReadGuidNoAlloc(reader)
            };
        }
    }
}
