using Engine;
using Engine.Types;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [RequiredComponent(typeof(Camera))]
    public class CameraFollow : ScriptBehavior
    {
        public Transform Target { get; set; }
        public float FollowSpeed { get; set; } = 5f;

        private vec2 deadZoneSize = new vec2(0f, 4f);
        private float smoothTime = 0.2f;
        private vec3 velocity;
        private Camera _cam;
        protected override void OnAwake()
        {
            _cam = GetComponent<Camera>();
        }

        protected override void OnLateUpdate()
        {
            Move();
        }

        public void SetOnTargetImmediate()
        {
            Transform.WorldPosition = new vec3(Target.WorldPosition.x, Target.WorldPosition.y, Transform.WorldPosition.z);
        }
        private void Move()
        {
            if (!Target)
                return;

            var camPos = Transform.WorldPosition;
            // var targetPos = new vec4(Target.GetRenderingWorldMatrix()[3]);
            var targetPos = Target.Transform.WorldPosition;

            float left = camPos.x - deadZoneSize.x * 0.5f;
            float right = camPos.x + deadZoneSize.x * 0.5f;
            float bottom = camPos.y - deadZoneSize.y * 0.5f;
            float top = camPos.y + deadZoneSize.y * 0.5f;

            float newX = camPos.x;
            float newY = camPos.y;

            if (targetPos.x < left) newX = targetPos.x + deadZoneSize.x * 0.5f;
            if (targetPos.x > right) newX = targetPos.x - deadZoneSize.x * 0.5f;

            if (targetPos.y < bottom) newY = targetPos.y + deadZoneSize.y * 0.5f;
            if (targetPos.y > top) newY = targetPos.y - deadZoneSize.y * 0.5f;

            var targetCameraPos = new vec3(newX, newY, camPos.z);

            Transform.WorldPosition = Mathf.SmoothDamp(camPos, targetCameraPos, ref velocity, smoothTime);

            if (Physics2D.DrawColliders)
            {
                Debug.DrawBox(new vec3(Transform.WorldPosition.x, Transform.WorldPosition.y, 0), new vec3(deadZoneSize.x, deadZoneSize.y, 0), Color.Green);
            }
        }
    }
}