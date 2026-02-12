using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.IO
{
    internal class AudioClipAssetBuilder : IAssetBuilder<AudioClip, AssetMeta>
    {
        public AudioClip BuildAsset(ref readonly AssetInfo info, AssetMeta meta, BinaryReader reader)
        {
            var sampleRate = reader.ReadInt32();
            var channels = reader.ReadInt32();
            var sampleFormat = reader.ReadInt32();
            var samplesLength = reader.ReadInt32();

            var data = new float[samplesLength];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = reader.ReadSingle();
            }

            return new AudioClip(info.Path,
                                 meta.GUID, data, sampleRate, 
                                 samplesLength, channels, sampleFormat);
        }

        public void UpdateAsset(ref readonly AssetInfo info, AudioClip asset, AssetMeta meta, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}