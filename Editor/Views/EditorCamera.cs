using Engine;
using Engine.Utils;
using GlmNet;
using ImGuiNET;
using System;
using System.Numerics;

namespace Editor
{
    internal class EditorCamera : ICamera
    {
        private vec3 _pivot = new vec3(0, 0, 0);
        private float _distance = 45.0f;

        private Quaternion _rotation = Quaternion.Identity;

        private Vector2 _screenSize;
        private vec3 _worldPosition;
        private const float MinDistance = 0.01f;
        private const float MaxDistance = 3000.0f;

        public mat4 Projection { get; private set; }
        public mat4 ViewMatrix { get; private set; }

        public bool IsEnabled => true;
        public bool IsValid => true;
        public Color BackgroundColor => new Color(0.2f, 0.2f, 0.2f, 1.0f);

        public RenderTexture RenderTexture { get; set; }
        RenderTexture ICamera.OutRenderTexture { get; set; }
        public int Priority { get; set; }
        public vec3 Forward => GetAxis(Vector3.UnitZ, _rotation);
        public vec3 Right => GetAxis(Vector3.UnitX, _rotation);
        public vec3 Up => GetAxis(Vector3.UnitY, _rotation);
        public vec3 WorldPosition => _worldPosition;
        public float NearPlane { get; set; } = 0.001f;
        public float FarPlane { get; set; } = 10000.0f;
        public float Fov { get; set; } = 60;
        public float Aspect => _screenSize.X / _screenSize.Y;
        public float OrthographicSize { get; set; } = 32;
        public CameraProjectionMode ProjectionMode { get; set; } = CameraProjectionMode.Perspective;

        public vec4 Viewport { get; set; } = new vec4(0, 0, 1, 1);

        public EditorCamera(float aspect = 16f / 9f)
        {
            _screenSize = new Vector2(Screen.Width, Screen.Height);
            UpdateProjection();
            UpdateView();
        }

        private void UpdateProjection()
        {
            if (ProjectionMode == CameraProjectionMode.Perspective)
            {
                Projection = MathUtils.Perspective(glm.radians(Fov), _screenSize.X / _screenSize.Y, NearPlane, FarPlane);
            }
            else
            {
                float orthoHeight = OrthographicSize;
                float orthoWidth = orthoHeight * (_screenSize.X / _screenSize.Y);

                float left = -orthoWidth;
                float right = orthoWidth;
                float bottom = -orthoHeight;
                float top = orthoHeight;

                Projection = MathUtils.Ortho(left, right, bottom, top, NearPlane, FarPlane);
            }
        }
        public void Update()
        {
            var size = ImGui.GetWindowSize();

            if ((int)_screenSize.X != (int)size.X || (int)_screenSize.Y != (int)size.Y)
            {
                _screenSize.X = size.X;
                _screenSize.Y = size.Y;
                UpdateProjection();
            }

            var io = ImGui.GetIO();
            if (!ImGui.IsWindowHovered() && !ImGui.IsWindowFocused())
                return;

            vec2 mouseDelta = new vec2(io.MouseDelta.X, io.MouseDelta.Y);

            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                const float sensitivity = 0.005f;


                float dx = mouseDelta.x * sensitivity;
                float dy = mouseDelta.y * sensitivity;

                Vector3 worldUp = Vector3.UnitY;
                Vector3 right = Vector3.Transform(Vector3.UnitX, _rotation);

                Quaternion qYaw = Quaternion.CreateFromAxisAngle(worldUp, dx);
                Quaternion qPitch = Quaternion.CreateFromAxisAngle(right, dy);

                _rotation = Quaternion.Normalize(qYaw * qPitch * _rotation);
            }

            if (ImGui.IsMouseDown(ImGuiMouseButton.Middle))
            {
                float fovY = glm.radians(60.0f);

                float worldHeight = 2.0f * _distance * MathF.Tan(fovY * 0.5f);
                float worldWidth = worldHeight * (_screenSize.X / _screenSize.Y);

                float unitsPerPixelY = worldHeight / _screenSize.Y;
                float unitsPerPixelX = worldWidth / _screenSize.X;

                _pivot -= Right * mouseDelta.x * unitsPerPixelX;
                _pivot += Up * mouseDelta.y * unitsPerPixelY;
            }

            if (io.MouseWheel != 0 && ImGui.IsWindowHovered())
            {
                const float zoomSpeed = 0.1f;
                _distance *= 1.0f - io.MouseWheel * zoomSpeed;
                _distance = Mathf.Clamp(_distance, MinDistance, MaxDistance);

                if (ProjectionMode == CameraProjectionMode.Orthographic)
                {
                    OrthographicSize = _distance;
                    UpdateProjection();
                }
            }

            UpdateView();
        }
        public void MoveThirdPerson(float deltaTime, float speed = 5.0f)
        {
            vec3 move = new vec3(0, 0, 0);

            var io = ImGui.GetIO();

            // Build movement vector from keys
            if (ImGui.IsKeyDown(ImGuiKey.W)) move += Forward;
            if (ImGui.IsKeyDown(ImGuiKey.S)) move -= Forward;
            if (ImGui.IsKeyDown(ImGuiKey.D)) move += Right;
            if (ImGui.IsKeyDown(ImGuiKey.A)) move -= Right;

            if (move.length() > 0.0f)
            {
                // Flatten movement to XZ plane (ground-aligned)
                move.y = 0.0f;
                move = glm.normalize(move);

                _pivot += move * speed * deltaTime;
                UpdateView();
            }
        }
        private void UpdateView()
        {
            _worldPosition = _pivot - Forward * _distance;
            ViewMatrix = MathUtils.LookAt(_worldPosition, _pivot, Up);
        }

        private vec3 GetAxis(Vector3 unit, Quaternion rot)
        {
            Vector3 f = Vector3.Transform(unit, rot);
            return glm.normalize(new vec3(f.X, f.Y, f.Z));
        }

    }
}
