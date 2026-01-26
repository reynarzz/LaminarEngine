using Editor.AssemblyHotReload;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Editor.Views
{
    internal class FooterBarView : EditorWindow
    {
        public override void OnDraw()
        {
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();

            ImGui.SetNextWindowPos(viewport.Pos + new Vector2(0, viewport.Size.Y - 25));
            ImGui.SetNextWindowSize(new Vector2(viewport.Size.X, 25));
            ImGui.SetNextWindowViewport(viewport.ID);

            var footerFlags = ImGuiWindowFlags.NoTitleBar |
                                     ImGuiWindowFlags.NoCollapse |
                                     ImGuiWindowFlags.NoResize |
                                     ImGuiWindowFlags.NoMove |
                                     ImGuiWindowFlags.NoBringToFrontOnFocus |
                                     ImGuiWindowFlags.NoNavFocus |
                                     ImGuiWindowFlags.NoDocking;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Vector2.Zero);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.13f, 0.13f, 0.13f, 1.0f));
            OnBeginWindow("FooterSpace", footerFlags, false);

            if (GameAssemblyBuilder.IsBuilding)
            {
                ImGui.Text("Compiling...");
            }
            OnEndWindow();
            ImGui.PopStyleColor();

            ImGui.PopStyleVar(4);

        }
    }
}
