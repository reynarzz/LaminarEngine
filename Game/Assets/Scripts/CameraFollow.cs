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
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : ScriptBehavior
    {
        public Transform Target { get; set; }
        public float FollowSpeed { get; set; } = 5f;

        private vec2 deadZoneSize = new vec2(0f, 4f);
        private float smoothTime = 0.2f;
        private vec3 velocity;
        public Bounds LevelBounds { get; set; }
        [RequiredProperty] private Camera _camera;
        protected override void OnAwake()
        {
            Actor.DontDestroyOnLoad(this);
        }

        protected override void OnLateUpdate()
        {
            Move();
        }

        public void SetOnTargetImmediate()
        {
            Transform.WorldPosition = AdjustPositionInsideBounds(new vec3(Target.WorldPosition.x, Target.WorldPosition.y, Transform.WorldPosition.z));
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

            Transform.WorldPosition = AdjustPositionInsideBounds(Mathf.SmoothDamp(camPos, AdjustPositionInsideBounds(targetCameraPos), ref velocity, smoothTime));
            
            if (Physics2D.DrawColliders)
            {
                Debug.DrawBox(new vec3(Transform.WorldPosition.x, Transform.WorldPosition.y, 0), new vec3(deadZoneSize.x, deadZoneSize.y, 0), Color.Green);
            }
        }

        private vec3 AdjustPositionInsideBounds(vec3 pos)
        {
            var frustum = _camera.GetFrustumBoundsWorld();
            var targetCameraPos = Mathf.Clamp(pos, LevelBounds.Min + frustum.Extents, LevelBounds.Max - frustum.Extents);
            targetCameraPos.z = pos.z;
            return targetCameraPos;
        }
    }
}