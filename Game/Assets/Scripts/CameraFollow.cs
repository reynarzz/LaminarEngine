using Engine;
using Engine.Types;
using GlmNet;
using Engine;
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
        [SerializedField] public Transform Target { get; set; }
        [SerializedField] public float FollowSpeed { get; set; } = 5f;

        private vec2 deadZoneSize = new vec2(0f, 4f);
        [SerializedField] private float smoothTime = 0.2f;
        private vec3 velocity;
        [ShowFieldNoSerialize(true)] public Bounds LevelBounds { get; set; }
        [RequiredProperty] private Camera _camera;
        private readonly float _zPosition = -10;
        [SerializedField] private bool _clampBounds = true;
       // [ShowFieldNoSerialize] private bool _showBounds;

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
            Transform.WorldPosition = AdjustPositionInsideBounds(new vec3(Target.WorldPosition.x, Target.WorldPosition.y, _zPosition));
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

            var smoothPos = Mathf.SmoothDamp(camPos, targetCameraPos, ref velocity, smoothTime);

            if (_clampBounds)
            {
                var clampedTarget = AdjustPositionInsideBounds(targetCameraPos);
                //var smooth = Mathf.Lerp(camPos, clampedTarget, FollowSpeed * Time.DeltaTime);
                var smooth = Mathf.SmoothDamp(camPos, clampedTarget, ref velocity, smoothTime);


                var final = AdjustPositionInsideBounds(smooth);

                if (final.x != smooth.x)
                    velocity.x = 0;

                if (final.y != smooth.y)
                    velocity.y = 0;

                Transform.WorldPosition = final;
            }
            else
            {
                Transform.WorldPosition = smoothPos;
            }
            //Transform.WorldPosition = Easing.Apply(EasingType.EaseOutElastic, Transform.WorldPosition, AdjustPositionInsideBounds(targetCameraPos), Time.DeltaTime);

            if (Physics2D.DrawColliders)
            {
                Debug.DrawBox(new vec3(Transform.WorldPosition.x, Transform.WorldPosition.y, 0), new vec3(deadZoneSize.x, deadZoneSize.y, 0), Color.Green);
            }
        }
        protected override void OnDrawGizmo()
        {
            if (_showBounds)
            {
                // Debug.DrawBox(Transform.WorldPosition, LevelBounds.Max, Color.Cyan);
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