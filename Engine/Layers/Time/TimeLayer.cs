using System;
using System.Diagnostics;

namespace Engine.Layers
{
    internal class TimeLayer : LayerBase
    {
        private Stopwatch _stopwatch;
        private float _lastFrameTime;
        private float _timePast;
        private float _unscaledTimePast;
        private const float _timeWrapLength = 10;
        public override void Initialize()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _lastFrameTime = 0f;
            _timePast = 0;
            _unscaledTimePast = 0;
        }

        internal override void UpdateLayer()
        {
            float currentTime = (float)_stopwatch.Elapsed.TotalSeconds;
            float unscaledDeltaTime = (currentTime - _lastFrameTime);

            if (unscaledDeltaTime > 0.1)
            {
                unscaledDeltaTime = 0.016f;
            }

            float deltaTime = unscaledDeltaTime * Time.TimeScale;

            _lastFrameTime = currentTime;
            _timePast += deltaTime;

            _unscaledTimePast += unscaledDeltaTime;

            // Update the Time static class
            Time.DeltaTime = deltaTime;
            Time.UnscaledDeltaTime = unscaledDeltaTime;
            Time.UnscaledTime = _unscaledTimePast;
            Time.TimeCurrent = _timePast;
            Time.SinceStarted = currentTime;
            Time.UnscaledTimeWrap = Mathf.Wrap(_unscaledTimePast, _timeWrapLength);
            Time.TimeCurrentWrap = Mathf.Wrap(_timePast, _timeWrapLength);

            Time.FPS = Time.UnscaledDeltaTime > 0f ? 1f / Time.UnscaledDeltaTime : 0f;
        }

        public override void Close()
        {
            _stopwatch.Stop();
        }
    }
}