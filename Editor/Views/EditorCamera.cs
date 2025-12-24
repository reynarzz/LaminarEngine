using Engine;
using GlmNet;

namespace Editor
{
    internal class EditorCamera : ICamera
    {
        public bool IsEnabled => true;
        public mat4 Projection => mat4.identity();
        public mat4 ViewMatrix => mat4.identity();
        public Color BackgroundColor => Color.Red;
        public RenderTexture RenderTexture => null;
        public bool IsAlive { get; } = true;

        public void Update()
        {

        }
    }
}
