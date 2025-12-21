using Engine;
using Engine.Layers;
using ImGuiNET;

namespace Editor
{
    internal class EditorEntry
    {
        private Window _win;
        private ImGuiGLFW _glfwInput;
        private TimeLayer _time;
        internal void Init()
        {
            _win = new Window("Editor", 1024, 640, Color.Black);
            _win.CanResize = true;
           
            ImguiImplOpenGL3.Init(_win.Width, _win.Height);
            _glfwInput = new ImGuiGLFW(Window.NativeWindow);
            _glfwInput.Init();
            _time = new TimeLayer();
            _time.Initialize();

            _win.OnWindowChanged += (w, h) =>
            {
                Render();
            };

            while (!_win.ShouldClose)
            {
               
                Render();
            }
        }

        private void Render()
        {
            _time.UpdateLayer();
            GLFW.Glfw.PollEvents();

            OpenGL.GL.glClearColor(1, 0, 0, 1);
            OpenGL.GL.glClear(OpenGL.GL.GL_COLOR_BUFFER_BIT);

            ImguiImplOpenGL3.SetPerFrameImGuiData(Time.DeltaTime, _win.Width, _win.Height);

            ImGui.NewFrame();

            _glfwInput.NewFrame();

            ImGui.Text("Hello world");

            ImGui.ShowDemoWindow();

            ImGui.Render();
            ImguiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

            _win.SwapBuffers();

        }
    }
}
