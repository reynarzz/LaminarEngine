using Editor.Utils;
using Engine;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal abstract class AssetBinaryProcessorReleaseBase<T> : IAssetProcessor
    {
        public AssetProccesResult Process(BinaryReader reader, AssetMeta meta, CookingPlatform platform)
        {
            var bytes = ArrayPool<byte>.Shared.Rent((int)reader.BaseStream.Length);

            try
            {
                reader.BaseStream.ReadExactly(bytes, 0, (int)reader.BaseStream.Length);
                var str = Encoding.UTF8.GetString(bytes, 0, (int)reader.BaseStream.Length);
                var assetIR = EditorJsonUtils.Deserialize<T>(str);

                using var stream = new MemoryStream();
                using var writer = new BinaryWriter(stream);
                WriteBinary(writer, assetIR);
                writer.Flush();

                return new AssetProccesResult()
                {
                    IsSuccess = true,
                    Data = stream.ToArray()
                };
            }
            catch (Exception e)
            {
                Debug.EngineError(e);
                return default;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }
        protected abstract void WriteBinary(BinaryWriter writer, T asset);
    }
}
