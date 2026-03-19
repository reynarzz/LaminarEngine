using SoundFlow.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AudioClip : Asset
    {
        public TimeSpan Duration { get; }
        public int SampleRate { get; }
        public int Channels { get; }

        internal int SampleFormat { get; }
        internal float[] RawAudioData { get; }
        
        public AudioClip(string path, Guid refid, float[] rawData, int sampleRate, int totalSamples, int channels, int sampleFormat) : base(refid)
        {
            Duration = TimeSpan.FromSeconds((float)totalSamples / (sampleRate * channels));
            SampleRate = sampleRate;
            Channels = channels;
            SampleFormat = sampleFormat;
            RawAudioData = rawData;
        }

        protected override void OnUpdateResource(object data, Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
