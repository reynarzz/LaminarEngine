using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal class AnimationPlayer 
    {
        private AnimationClip _currentClip;
        private float _currentTime;
        private bool _isPlaying;

        internal void Play(AnimationClip clip)
        {
            _currentClip = clip;
            _isPlaying = true;
            _currentTime = 0f;
        }

        internal void Update()
        {
            if (_currentClip == null)
            {
                return;
            }
            
            if (_isPlaying)
            {
                _currentTime += Time.DeltaTime;
                if (_currentClip.Loop)
                {
                    _currentTime %= _currentClip.Duration;
                }
                else
                {
                    _currentTime = MathF.Min(_currentTime, _currentClip.Duration);
                }
            }
        }

        public void Pause()
        {
            _isPlaying = false;
        }

        public void Resume()
        {
            _isPlaying = true;
        }

        public void Stop()
        {
            _isPlaying = false;
            _currentTime = 0;
        }

        internal float GetFloat(string property)
        {
            return _currentClip?.EvaluateFloat(property, _currentTime) ?? 0f;
        }
        
        internal Sprite GetSprite(string property)
        {
            return _currentClip?.EvaluateSprite(property, _currentTime);
        }

        internal vec2 GetVec2(string property)
        {
            return _currentClip?.EvaluateVec2(property, _currentTime) ?? default;
        }

        internal vec3 GetVec3(string property)
        {
            return _currentClip?.EvaluateVec3(property, _currentTime) ?? default;
        }

        internal quat GetQuat(string property)
        {
            return _currentClip?.EvaluateQuat(property, _currentTime) ?? default;
        }

        internal Color GetColor(string property)
        {
            return _currentClip?.EvaluateColor(property, _currentTime) ?? default;
        }

        internal Action GetEvent()
        {
            return _currentClip?.EvaluateEvent(_currentTime);
        }
    }
}
