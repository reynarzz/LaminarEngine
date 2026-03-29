using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class InitializationWindow : EditorWindow
    {
        public override void OnDraw()
        {
            var winSize = new Vector2(300, 100);
            var viewport = ImGui.GetMainViewport();

            ImGui.SetNextWindowSize(winSize, ImGuiCond.Once);
            ImGui.SetNextWindowPos(viewport.Pos.X + viewport.Size.X * 0.5f - winSize.X * 0.5f,
                                   viewport.Pos.Y + viewport.Size.Y * 0.5f - winSize.Y * 0.5f);
            ImGui.Begin("Initializing Editor", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);
            _fakeProgress += ImGui.GetIO().DeltaTime * 0.04f;

            string fakeMessage = null;

            if (_fakeProgress <= 0.3f)
            {
                fakeMessage = "Compiling GameApplication Domain";
            }
            else if (_fakeProgress < 1.0f)
            {
                fakeMessage = "Importing assets";
            }
            else
            {
                fakeMessage = "Finishing up";
            }

            ImGui.Text($"{fakeMessage}");

            ImGui.ProgressBar(_fakeProgress, new Vector2(270, 20));
            ImGui.End();

        }
        private float _fakeProgress = 0;
        private void DrawInitialization()
        {
            

        }

    }
}
