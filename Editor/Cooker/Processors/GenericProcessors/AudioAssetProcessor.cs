using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Engine;
using SoundFlow;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace Editor.Cooker
{
    internal class AudioAssetProcessor : IAssetProcessor
    {
        private readonly static MiniAudioEngine _engine = new MiniAudioEngine();

        AssetProccesResult IAssetProcessor.Process(BinaryReader reader, AssetMeta meta, CookingPlatform platform)
        {
            var format = AudioFormat.DvdHq;

            using var provider = new StreamDataProvider(_engine, format, reader.BaseStream);

            float[] buffer = new float[provider.Length];

            int samplesData = provider.ReadBytes(buffer);

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write(format.SampleRate);
            bw.Write(format.Channels);
            bw.Write((int)format.Format);
            bw.Write(samplesData);

            // Write all float samples
            foreach (var sample in buffer)
            {
                bw.Write(sample);
            }

            return new AssetProccesResult()
            {
                IsSuccess = true,
                Data = ms.ToArray()
            };
        }
    }
}