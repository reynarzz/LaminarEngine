using Engine;
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

        private float _yaw = 0.0f;
        private float _pitch = 0.0f;
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
            Projection = glm.perspective(glm.radians(60.0f), aspect, 0.1f, 1000.0f);
            UpdateView();
        }

        public void Update()
        {
            var size = ImGui.GetWindowSize();

            if ((int)_screenSize.X != (int)size.X || (int)_screenSize.Y != (int)size.Y)
            {
                _screenSize.X = (int)size.X;
                _screenSize.Y = (int)size.Y;

                Projection = glm.perspective(glm.radians(60.0f), size.X / size.Y, 0.1f, 1000.0f);
            }

            var io = ImGui.GetIO();
            if (!ImGui.IsWindowHovered() && !ImGui.IsWindowFocused())
                return;

            vec2 mouseDelta = new vec2(io.MouseDelta.X, io.MouseDelta.Y);

            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                float sensitivity = 0.005f;
                _yaw -= mouseDelta.x * sensitivity;
                _pitch -= mouseDelta.y * sensitivity;

                _pitch = Mathf.Clamp(_pitch, -1.55f, 1.55f);
            }

            if (ImGui.IsMouseDown(ImGuiMouseButton.Middle))
            {
                float fovY = glm.radians(60.0f);
                float worldHeight = 2.0f * _distance * MathF.Tan(fovY * 0.5f);
                float unitsPerPixel = worldHeight / _screenSize.Y;

                vec3 right = GetRight();
                vec3 up = GetUp();

                _pivot -= right * mouseDelta.x * unitsPerPixel;
                _pivot += up * mouseDelta.y * unitsPerPixel;
            }

            if (io.MouseWheel != 0)
            {
                float zoomSpeed = 0.1f;
                _distance *= 1.0f - io.MouseWheel * zoomSpeed;
                _distance = Mathf.Clamp(_distance, MinDistance, MaxDistance);
            }

            UpdateView();
        }

        private void UpdateView()
        {
            vec3 direction = new vec3(MathF.Cos(_pitch) * MathF.Sin(_yaw),
                                      MathF.Sin(_pitch),
                                      MathF.Cos(_pitch) * MathF.Cos(_yaw));

            vec3 position = _pivot - direction * _distance;
            ViewMatrix = glm.lookAt(position, _pivot, new vec3(0, 1, 0));
        }

        private vec3 GetRight()
        {
            return glm.normalize(glm.cross(GetForward(), new vec3(0, 1, 0)));
        }

        private vec3 GetUp()
        {
            return glm.normalize(glm.cross(GetRight(), GetForward()));
        }

        private vec3 GetForward()
        {
            return glm.normalize(_pivot - GetPosition());
        }

        private vec3 GetPosition()
        {
            vec3 dir = new vec3(MathF.Cos(_pitch) * MathF.Sin(_yaw),
                                MathF.Sin(_pitch),
                                MathF.Cos(_pitch) * MathF.Cos(_yaw));

            return _pivot - dir * _distance;
        }
    }
}
