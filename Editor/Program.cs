using Engine;
using ImGuiNET;

namespace Editor
{
    internal class Program
    {
        private static Window _win;
        private static string _text = "hello world";
        static void Main(string[] args)
        {
            _win = new Window("Editor", 1024, 640, Color.Black);
            _win.CanResize = true;

            //var imgui = new DearImGuiWindow(_win.Width, _win.Height);
            CreateContext();


            ImguiImplOpenGL3.Init(_win.Width, _win.Height);

            _win.OnWindowChanged += (w, h) =>
            {
              //  imgui.OnResize(w, h);
            };

            while (!_win.ShouldClose)
            {
                GLFW.Glfw.PollEvents();

                OpenGL.GL.glClearColor(1, 0, 0, 1);
                OpenGL.GL.glClear(OpenGL.GL.GL_COLOR_BUFFER_BIT);

                ImguiImplOpenGL3.SetPerFrameImGuiData(1, _win.Width, _win.Height);

                ImGui.NewFrame();
                ImGui.Text("Hello world");

                ImGui.ShowDemoWindow();         

                ImGui.Render();                 
                ImguiImplOpenGL3.RenderDrawData(ImGui.GetDrawData()); 

                _win.SwapBuffers();
            }
        }

        private static void CreateContext()
        {
           
        }
        
    }
}
