using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace Game
{
    internal class CameraViewportAdjust : ScriptBehavior
    {
        [SerializedField] private vec2 _aspectRatio = new vec2(16.0f, 9.0f);
        protected override void OnAwake()
        {
            base.OnAwake();

            var cam = GetComponent<Camera>();

            float screenWidth = Screen.Width;
            float screenHeight = Screen.Height;

            float targetWidth = screenWidth;
            float targetHeight = screenWidth * (_aspectRatio.y / _aspectRatio.x);

            if (targetHeight > screenHeight)
            {
                targetHeight = screenHeight;
                targetWidth = screenHeight * (_aspectRatio.x / _aspectRatio.y);
            }

            float viewportWidth = targetWidth / screenWidth;
            float viewportHeight = targetHeight / screenHeight;

            float x = (1.0f - viewportWidth) * 0.5f;
            float y = (1.0f - viewportHeight) * 0.5f;

            cam.Viewport = new vec4(x, y, viewportWidth, viewportHeight);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            return;
            var transform = Transform;

            const float PixelsPerUnit = 48.0f;

            float unitsPerPixel = 1.0f / PixelsPerUnit;

            float x = (float)Math.Round(transform.WorldPosition.x / unitsPerPixel) * unitsPerPixel;
            float y = (float)Math.Round(transform.WorldPosition.y / unitsPerPixel) * unitsPerPixel;

            transform.WorldPosition = new vec3(x, y, transform.WorldPosition.z);
        }
    }
}
