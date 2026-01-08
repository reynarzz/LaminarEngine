using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Graphics;
using Engine.Types;
using Engine.Utils;
using GlmNet;
using SharedTypes;

namespace Engine
{
    public enum CameraProjectionMode
    {
        Orthographic,
        Perspective
    }

    public struct FrustumCorners
    {
        public vec3 NearTopLeft;
        public vec3 NearTopRight;
        public vec3 NearBottomRight;
        public vec3 NearBottomLeft;

        public vec3 FarTopLeft;
        public vec3 FarTopRight;
        public vec3 FarBottomRight;
        public vec3 FarBottomLeft;

        public Bounds GetAABB()
        {
            vec3 min = NearTopLeft;
            vec3 max = NearTopLeft;

            void Include(vec3 p)
            {
                min.x = Math.Min(min.x, p.x);
                min.y = Math.Min(min.y, p.y);
                min.z = Math.Min(min.z, p.z);

                max.x = Math.Max(max.x, p.x);
                max.y = Math.Max(max.y, p.y);
                max.z = Math.Max(max.z, p.z);
            }

            Include(NearTopRight);
            Include(NearBottomRight);
            Include(NearBottomLeft);
            Include(FarTopLeft);
            Include(FarTopRight);
            Include(FarBottomRight);
            Include(FarBottomLeft);

            return new Bounds()
            {
                Min = min,
                Max = max
            };
        }
    }

    [UniqueComponent]
    public class Camera : Component, ICamera
    {
        public mat4 Projection { get; private set; }
        public mat4 ViewMatrix => glm.inverse(Transform.WorldMatrix);
        bool ICamera.IsEnabled => IsEnabled && Actor && Actor.IsActiveInHierarchy;

        [SerializedField] public Color BackgroundColor { get; set; } = Color.Gray;
        private CameraProjectionMode _projectionMode;
        [SerializedField("Projection")]
        public CameraProjectionMode ProjectionMode
        {
            get => _projectionMode;
            set
            {
                if (_projectionMode == value)
                    return;

                _projectionMode = value;
                UpdateCurrent();
            }
        }
        private float _fov = 60;
        [SerializedField("Field of view")]
        public float Fov
        {
            get => _fov;
            set
            {
                _fov = value;
                UpdateCurrent();
            }
        }
        private float _orthoSize;
        [SerializedField]
        public float OrthographicSize
        {
            get => _orthoSize;
            set
            {
                _orthoSize = value;
                UpdateCurrent();
            }
        }

        private float _nearPlane = 0.1f;
        private float _farPlane = 100.0f;
        [SerializedField] public float NearPlane { get => _nearPlane; set { _nearPlane = Math.Clamp(value, 0.0001f, FarPlane - 1); UpdateCurrent(); } }
        [SerializedField] public float FarPlane { get => _farPlane; set { _farPlane = Math.Clamp(value, NearPlane + 1, 5000); UpdateCurrent(); } }
        public float Aspect => Viewport.z / Viewport.w;
        [SerializedField] public RenderTexture RenderTexture { get; set; }
        RenderTexture ICamera.OutRenderTexture { get; set; }

        [SerializedField] public int Priority { get; set; } = 0;
        public vec3 WorldPosition => Transform.WorldPosition;
        public vec3 Forward => Transform.Forward;
        public vec3 Right => Transform.Right;
        public vec3 Up => Transform.Up;

        public vec4 _viewport;
        public vec4 Viewport
        {
            get
            {
                _viewport.z = Screen.Width;
                _viewport.w = Screen.Height;

                return _viewport;
            }
            set
            {
                _viewport.x = value.x;
                _viewport.y = value.y;
            }
        }

        bool ICamera.IsAlive => IsAlive;

        protected override void OnAwake()
        {
            base.OnAwake();

            OrthographicSize = 32;
            Fov = 60.0f;
            WindowManager.Window.OnWindowChanged += OnWindowChanged;
            UpdateCurrent();
            //UpdatePerspective();
        }

        private void OnWindowChanged(int width, int height)
        {
            UpdateCurrent();
        }

        private void UpdateCurrent()
        {
            float aspect = Viewport.z / Viewport.w;

            if (_projectionMode == CameraProjectionMode.Orthographic)
            {
                float orthoHeight = OrthographicSize;
                float orthoWidth = orthoHeight * aspect;

                float left = -orthoWidth;
                float right = orthoWidth;
                float bottom = -orthoHeight;
                float top = orthoHeight;

                Projection = MathUtils.Ortho(left, right, bottom, top, NearPlane, FarPlane);
            }
            else
            {
                Projection = MathUtils.Perspective(glm.radians(Fov), aspect, NearPlane, FarPlane);
            }
        }

        private FrustumCorners GetOrthoFrustumCornersViewSpace(float left, float right, float bottom,
                                                                float top, float near, float far)
        {
            FrustumCorners c;

            // Near plane
            c.NearTopLeft = new vec3(left, top, -near);
            c.NearTopRight = new vec3(right, top, -near);
            c.NearBottomRight = new vec3(right, bottom, -near);
            c.NearBottomLeft = new vec3(left, bottom, -near);

            // Far plane
            c.FarTopLeft = new vec3(left, top, -far);
            c.FarTopRight = new vec3(right, top, -far);
            c.FarBottomRight = new vec3(right, bottom, -far);
            c.FarBottomLeft = new vec3(left, bottom, -far);

            return c;
        }


        private FrustumCorners GetPerspectiveFrustumCornersViewSpace(float fov, float aspect,
                                                                           float near, float far)
        {
            FrustumCorners corners;

            float tanFov = MathF.Tan(glm.radians(fov) * 0.5f);

            float nh = near * tanFov;
            float nw = nh * aspect;
            float fh = far * tanFov;
            float fw = fh * aspect;

            // Near plane
            corners.NearTopLeft = new vec3(-nw, nh, -near);
            corners.NearTopRight = new vec3(nw, nh, -near);
            corners.NearBottomRight = new vec3(nw, -nh, -near);
            corners.NearBottomLeft = new vec3(-nw, -nh, -near);

            // Far plane
            corners.FarTopLeft = new vec3(-fw, fh, -far);
            corners.FarTopRight = new vec3(fw, fh, -far);
            corners.FarBottomRight = new vec3(fw, -fh, -far);
            corners.FarBottomLeft = new vec3(-fw, -fh, -far);

            return corners;
        }

        private FrustumCorners TransformFrustumCornersToWorld(FrustumCorners viewSpaceCorners, mat4 worldMatrix)
        {
            FrustumCorners w;
            w.NearTopLeft = new vec3(worldMatrix * new vec4(viewSpaceCorners.NearTopLeft, 1));
            w.NearTopRight = new vec3(worldMatrix * new vec4(viewSpaceCorners.NearTopRight, 1));
            w.NearBottomRight = new vec3(worldMatrix * new vec4(viewSpaceCorners.NearBottomRight, 1));
            w.NearBottomLeft = new vec3(worldMatrix * new vec4(viewSpaceCorners.NearBottomLeft, 1));
            w.FarTopLeft = new vec3(worldMatrix * new vec4(viewSpaceCorners.FarTopLeft, 1));
            w.FarTopRight = new vec3(worldMatrix * new vec4(viewSpaceCorners.FarTopRight, 1));
            w.FarBottomRight = new vec3(worldMatrix * new vec4(viewSpaceCorners.FarBottomRight, 1));
            w.FarBottomLeft = new vec3(worldMatrix * new vec4(viewSpaceCorners.FarBottomLeft, 1));
            return w;
        }
        public Bounds GetFrustumBoundsWorld()
        {
            float aspect = Viewport.z / Viewport.w;

            FrustumCorners cornersVS;
            if (ProjectionMode == CameraProjectionMode.Orthographic)
            {
                float orthoHeight = OrthographicSize;
                float orthoWidth = orthoHeight * aspect;

                float left = -orthoWidth;
                float right = orthoWidth;
                float bottom = -orthoHeight;
                float top = orthoHeight;

                cornersVS = GetOrthoFrustumCornersViewSpace(left, right, bottom, top, NearPlane, FarPlane);
            }
            else
            {
                cornersVS = GetPerspectiveFrustumCornersViewSpace(Fov, aspect, NearPlane, FarPlane);
            }

            FrustumCorners cornersWS = TransformFrustumCornersToWorld(cornersVS, Transform.WorldMatrix);
            return cornersWS.GetAABB();
        }


        protected internal override void OnDestroy()
        {
            base.OnDestroy();
            WindowManager.Window.OnWindowChanged -= OnWindowChanged;
        }
    }
}
