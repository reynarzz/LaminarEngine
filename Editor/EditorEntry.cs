using Engine;
using ImGuiNET;

namespace Editor
{
    internal class EditorEntry
    {
        private Window _win;
        internal void Init()
        {
            _win = new Window("Editor", 1024, 640, Color.Black);
            _win.CanResize = true;
           
            ImguiImplOpenGL3.Init(_win.Width, _win.Height);
            var input = new ImGuiGLFW(Window.NativeWindow);
            input.Init();

            _win.OnWindowChanged += (w, h) =>
            {

            };

            while (!_win.ShouldClose)
            {
                GLFW.Glfw.PollEvents();

                OpenGL.GL.glClearColor(1, 0, 0, 1);
                OpenGL.GL.glClear(OpenGL.GL.GL_COLOR_BUFFER_BIT);

                ImguiImplOpenGL3.SetPerFrameImGuiData(1, _win.Width, _win.Height);

                ImGui.NewFrame();

                input.NewFrame();

                // ImGui.Text("Hello world");

                ImGui.ShowDemoWindow();

                ImGui.Render();
                ImguiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

                _win.SwapBuffers();
            }
        }
    }
}
