using Engine;
using Engine.Types;
using GlmNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{

    [RequireComponent(typeof(Camera))]
    public class CameraShake : ScriptBehavior
    {
        [RequiredProperty]
        public Camera Camera { get; set; }
        public float ShakeForce { get; set; } = 1;
        public float ShakeAmplitude { get; set; } = 3;
        public bool IsShaking = false;
        public float FallOffSpeed { get; set; } = 1;
        public static CameraShake Instance { get; private set; }
        private vec3 _lastShake;

        protected override void OnAwake()
        {
            Instance = this;
        }

        protected override void OnUpdate()
        {
            Camera.Transform.WorldPosition -= _lastShake;

            if (IsShaking && ShakeAmplitude > 0)
            {
                ShakeAmplitude -= FallOffSpeed * Time.DeltaTime;
                if (ShakeAmplitude < 0) ShakeAmplitude = 0;

                float t = Time.TimeCurrent * ShakeForce;

                vec2 baseShake = Mathf.Noise2(t);
                vec2 detail1 = Mathf.Noise2(t * 4f) * 0.5f;
                vec2 detail2 = Mathf.Noise2(t * 12f) * 0.25f;

                vec2 shake2D = (baseShake + detail1 + detail2) / 1.75f;

                _lastShake = new vec3(shake2D.x, shake2D.y, 0) * ShakeAmplitude;

                Camera.Transform.WorldPosition += _lastShake;
            }
            else
            {
                _lastShake = vec3.Zero;
            }
        }

        public void BurstShake(float shakeForce, float shakeAmplitude, float time)
        {
            ShakeForce = shakeForce;
            ShakeAmplitude = shakeAmplitude;

            IEnumerator Timer()
            {
                IsShaking = true;
                float timer = time;
                while (timer > 0)
                {
                    timer -= Time.DeltaTime;
                    yield return null;
                }
                IsShaking = false;
                ShakeAmplitude = 0f; 
            }

            StartCoroutine(Timer());
        }
    }
}
