using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Modifiers;
using SoundFlow.Providers;
using SoundFlow.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    internal class AudioLayer : LayerBase
    {
        private static MiniAudioEngine _engine;
        private static AudioPlaybackDevice _currentDevice;
        private static AudioMixer _masterMixer;
        private const float FIND_DEVICE_RETRY_TIME = 3;
        private float _currentRetryTime = 0;
        private static readonly AudioFormat _defaultFormat = AudioFormat.DvdHq;

        public override Task<LayerInitResult> InitializeAsync()
        {
            try
            {
                _engine = new MiniAudioEngine();
                var defaultDevice = _engine.PlaybackDevices?.FirstOrDefault(x => x.IsDefault) ?? default;

                if (!defaultDevice.IsDefault)
                {
                    Debug.Warn($"No default playback device found. Using first available, Total: {_engine.PlaybackDevices.Length}");
                    defaultDevice = _engine.PlaybackDevices.FirstOrDefault();
                }

                _currentDevice = _engine.InitializePlaybackDevice(defaultDevice, _defaultFormat);
                _currentDevice.Start();

                var mixer = new Mixer(_engine, _defaultFormat, true);
                _masterMixer = new AudioMixer("Master", mixer);
            }
            catch (Exception e)
            {
                Debug.Error(e.ToString());
            }

            // Effects:
            // ParametricEqualizer 

            return Task.FromResult(LayerInitResult.Success);
        }

        internal override void UpdateLayer()
        {
            base.UpdateLayer();

            if (_currentDevice == null && (_currentRetryTime -= Time.DeltaTime) <= 0)
            {
                if (_engine.PlaybackDevices != null)
                {
                    for (int i = 0; i < _engine.PlaybackDevices.Length; i++)
                    {
                        try
                        {
                            _currentDevice = _engine.InitializePlaybackDevice(_engine.PlaybackDevices[i], _defaultFormat);
                            _currentDevice.Start();
                            break;
                        }
                        catch
                        {
                            _currentDevice = null;
                        }
                    }

                    if (_currentDevice == null)
                    {
                        _currentRetryTime = FIND_DEVICE_RETRY_TIME;
                        Debug.Error("No audio device found yet.");
                    }
                }
            }
        }

        internal static SoundPlayer CreateSoundPlayer(AudioFormat format, AudioMixer mixer, RawDataProvider provider)
        {
            var player = new SoundPlayer(_engine, format, provider);

            if (mixer != null)
            {
                mixer.AddPlayer(player);
            }
            else if (GetDeviceSafe(out var device))
            {
                device.MasterMixer.AddComponent(player);
            }

            return player;
        }

        internal static bool GetDeviceSafe(out AudioPlaybackDevice device)
        {
            device = null;
            try
            {
                if (_currentDevice != null && !_currentDevice.IsDisposed && _currentDevice.IsRunning && _currentDevice.MasterMixer != null)
                {
                    device = _currentDevice;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                device = null;
                Debug.Error("No valid audio device is present.");
                return false;
            }
            return true;
        }

        internal static Mixer CreateMixer()
        {
            var mixer = new Mixer(_engine, _defaultFormat);

            if (GetDeviceSafe(out var device))
            {
                device.MasterMixer.AddComponent(mixer);
            }
            return mixer;
        }

        internal static AudioMixer GetMasterAudioMixer()
        {
            return _masterMixer;
        }

        public override void Close()
        {
            _engine.Dispose();
        }
    }
}
