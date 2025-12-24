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
        private float _distance = 5.0f;

        private Quaternion _rotation = Quaternion.Identity;

        private Vector2 _screenSize;
        private const float MinDistance = 0.1f;
        private const float MaxDistance = 1000.0f;

        public mat4 Projection { get; private set; }
        public mat4 ViewMatrix { get; private set; }

        public bool IsEnabled => true;
        public bool IsAlive => true;
        public Color BackgroundColor => new Color(0.2f, 0.2f, 0.2f, 1.0f);

        public RenderTexture RenderTexture { get; set; }
        RenderTexture ICamera.OutRenderTexture { get; set; }

        public EditorCamera(float aspect = 16f / 9f)
        {
            Projection = MathUtils.Perspective(glm.radians(60.0f), aspect, 0.01f, 1000.0f);
            UpdateView();
        }

        public void Update()
        {
            var size = ImGui.GetWindowSize();

            if ((int)_screenSize.X != (int)size.X || (int)_screenSize.Y != (int)size.Y)
            {
                _screenSize.X = (int)size.X;
                _screenSize.Y = (int)size.Y;

                Projection = MathUtils.Perspective(glm.radians(60.0f),size.X / size.Y,0.01f,1000.0f);
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

                vec3 right = GetRight();
                vec3 up = GetUp();

                _pivot -= right * mouseDelta.x * unitsPerPixelX;
                _pivot += up * mouseDelta.y * unitsPerPixelY;
            }

            if (io.MouseWheel != 0)
            {
                const float zoomSpeed = 0.1f;
                _distance *= 1.0f - io.MouseWheel * zoomSpeed;
                _distance = Mathf.Clamp(_distance, MinDistance, MaxDistance);
            }

            UpdateView();
        }
        public void MoveThirdPerson(float deltaTime, float speed = 5.0f)
        {
            vec3 move = new vec3(0, 0, 0);

            var io = ImGui.GetIO();

            // Build movement vector from keys
            if (ImGui.IsKeyDown(ImGuiKey.W)) move += GetForward();
            if (ImGui.IsKeyDown(ImGuiKey.S)) move -= GetForward();
            if (ImGui.IsKeyDown(ImGuiKey.D)) move += GetRight();
            if (ImGui.IsKeyDown(ImGuiKey.A)) move -= GetRight();

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
            vec3 forward = GetForward();
            vec3 up = GetUp();

            vec3 position = _pivot - forward * _distance;
            ViewMatrix = MathUtils.LookAt(position, _pivot, up);
        }

        private vec3 GetForward()
        {
            Vector3 f = Vector3.Transform(Vector3.UnitZ, _rotation);
            return glm.normalize(new vec3(f.X, f.Y, f.Z));
        }

        private vec3 GetRight()
        {
            Vector3 r = Vector3.Transform(Vector3.UnitX, _rotation);
            return glm.normalize(new vec3(r.X, r.Y, r.Z));
        }

        private vec3 GetUp()
        {
            Vector3 u = Vector3.Transform(Vector3.UnitY, _rotation);
            return glm.normalize(new vec3(u.X, u.Y, u.Z));
        }
    }
}
