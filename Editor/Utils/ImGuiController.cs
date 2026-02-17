using System;
using Editor.Utils;
using Engine;
using GLFW;
using ImGuiNET;
using Engine;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Editor
{
    public unsafe class ImGuiController
    {
        public ImGuiController()
        {
            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            var root = Path.Combine(assemblyDir.Substring(0, assemblyDir.LastIndexOf("Editor")), "Editor/Data");


            var io = ImGui.GetIO();

            var fontFilePath = $"{root}/NotoSansDisplay-VariableFont_wdth,wght.ttf";

            if (File.Exists(fontFilePath))
            {
                io.Fonts.AddFontFromFileTTF(fontFilePath, 18.3f);
            }
            else
            {
                io.Fonts.AddFontDefault();
            }

            float sx, sy;
            Glfw.GetWindowContentScale(WindowStandalone.NativeWindow, out sx, out sy);

            io.DisplayFramebufferScale = new Vector2(sx, sy);

            var path = $"{root}/imgui.ini";
            byte[] bytes = Encoding.UTF8.GetBytes(path + '\0');

            byte* iniPath = (byte*)NativeMemory.Alloc((nuint)bytes.Length);
            bytes.CopyTo(new Span<byte>(iniPath, bytes.Length));
            io.NativePtr->IniFilename = iniPath;

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
            io.BackendFlags |= ImGuiBackendFlags.RendererHasViewports;

            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;
            io.KeyRepeatDelay = 0.35f; // default ~0.25
            io.KeyRepeatRate = 0.05f; // default ~0.05
            io.MouseDoubleClickTime = 0.8f;
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

            Styles2();
        }

        private void Styles2()
        {
            var style = ImGui.GetStyle();
            var green = EditorColors.MainColor.ToVector4();
            style.Alpha = 1.0f;
            float rounding = 3.0f;

            style.DisabledAlpha = 0.6000000238418579f;
            style.WindowPadding = new Vector2(4.0f, 5.0f);
            style.WindowRounding = rounding;
            style.WindowBorderSize = 0.0f; // was zero, TODO
            style.WindowMinSize = new Vector2(300.0f, 100.0f);
            style.WindowTitleAlign = new Vector2(0.0f, 0.5f);
            style.WindowMenuButtonPosition = ImGuiDir.None;
            style.ChildRounding = rounding;
            style.ChildBorderSize = 1.0f;
            style.PopupRounding = rounding;
            style.PopupBorderSize = 1.65f;
            style.FramePadding = new Vector2(6.0f, 3.0f);
            style.FrameRounding = rounding;
            style.FrameBorderSize = 0.0f;
            style.ItemSpacing = new Vector2(8.0f, 4.0f);
            style.ItemInnerSpacing = new Vector2(8.0f, 4.0f);
            style.CellPadding = new Vector2(4.0f, 2.0f);
            style.IndentSpacing = 21.0f;
            style.ColumnsMinSpacing = 6.0f;
            style.ScrollbarSize = 11.0f;
            style.ScrollbarRounding = 2.5f;
            style.GrabMinSize = 13.0f;
            style.GrabRounding = rounding;
            style.TabRounding = rounding;
            style.TabBorderSize = 0.0f;
            style.TabMinWidthForCloseButton = 0.0f;
            style.ColorButtonPosition = ImGuiDir.Right;
            style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
            style.SelectableTextAlign = new Vector2(0.0f, 0.0f);

            style.Colors[(int)ImGuiCol.Text] = new Vector4(1, 1, 1, 1);
            style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.5921569f, 0.5921569f, 0.5921569f, 1);
            style.Colors[(int)ImGuiCol.WindowBg] = EditorColors.Background.ToVector4();
            style.Colors[(int)ImGuiCol.ChildBg] = EditorColors.Background.ToVector4();
            style.Colors[(int)ImGuiCol.PopupBg] = EditorColors.Background.ToVector4();

            float borderColor = 0.1f;
            style.Colors[(int)ImGuiCol.Border] = new Vector4(borderColor, borderColor, borderColor, 1);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(borderColor, borderColor, borderColor, 0.5f);

            style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.11f, 0.11f, 0.11156863f, 1);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.23f, 0.23f, 0.23f, 1);
            style.Colors[(int)ImGuiCol.FrameBgActive] = green;
              
            style.Colors[(int)ImGuiCol.TitleBg] =  new Vector4(0.14509805f, 0.14509805f, 0.14901961f, 1);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.08f, 0.08f, 0.08f, 1); // Window title background
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.14509805f, 0.14509805f, 0.14901961f, 1);
                  
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.2f, 0.2f, 0.21568628f, 1);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.2f, 0.2f, 0.21568628f, 0);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.32156864f, 0.32156864f, 0.33333334f, 1);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.3529412f, 0.3529412f, 0.37254903f, 1);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.3529412f, 0.3529412f, 0.37254903f, 1);

            style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.9f, 0.9f, 0.9f, 1);
            style.Colors[(int)ImGuiCol.SliderGrab] = green;//new Vector4(0.11372549f, 0.5921569f, 0.9254902f, 1);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.1f, 0.1f, 0.1f, 1.0f); //new Vector4(0.0f, 0.46666667f, 0.78431374f, 1);

            style.Colors[(int)ImGuiCol.Button] = new Vector4(0.2f, 0.2f, 0.21568628f, 1);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.23f, 0.23f, 0.23f, 1);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.23f, 0.23f, 0.23f, 1);

            style.Colors[(int)ImGuiCol.Header] = new Vector4(0.2f, 0.2f, 0.21568628f, 1);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.23f, 0.23f, 0.23f, 1);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.25f, 0.25f, 0.25f, 1);

            style.Colors[(int)ImGuiCol.Separator] =
            style.Colors[(int)ImGuiCol.SeparatorHovered] =
            style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.30588236f, 0.30588236f, 0.30588236f, 1);

            //style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.14509805f, 0.14509805f, 0.14901961f, 1);
            //style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.2f, 0.2f, 0.21568628f, 1);
            //style.Colors[(int)ImGuiCol.ResizeGripActive] = style.Colors[(int)ImGuiCol.ResizeGripHovered];

            style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.14509805f, 0.14509805f, 0.14901961f, 1);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = style.Colors[(int)ImGuiCol.Border];
            style.Colors[(int)ImGuiCol.ResizeGripActive] = style.Colors[(int)ImGuiCol.Border];


            style.Colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(0.32156864f, 0.32156864f, 0.33333334f, 1);
            style.Colors[(int)ImGuiCol.DockingPreview] = new Vector4(0, 0, 0, 0.6f);

            var tabColor = EditorColors.TabColor.ToVector4();//new Vector4(0.133f, 0.545f, 0.133f, 1);
            var tabUnfocused = new Vector4(tabColor.X * 0.5f, tabColor.Y * 0.5f, tabColor.Z * 0.5f, 1);
               
            style.Colors[(int)ImGuiCol.TabDimmedSelected] = tabColor;
            style.Colors[(int)ImGuiCol.TabSelected] = tabColor;
            style.Colors[(int)ImGuiCol.Tab] = tabUnfocused;
            style.Colors[(int)ImGuiCol.TabHovered] = tabColor;
            //style.Colors[(int)ImGuiCol.TabUnfocused] = tabUnfocused;
            //style.Colors[(int)ImGuiCol.TabUnfocusedActive] = tabColor;
            style.Colors[(int)ImGuiCol.TabSelectedOverline] = tabColor;
            style.Colors[(int)ImGuiCol.TabDimmed] = tabUnfocused; 
            style.Colors[(int)ImGuiCol.TabDimmedSelectedOverline] = default;
            style.Colors[(int)ImGuiCol.TabHorizontalLine] = new Vector4(0.1f, 0.1f, 0.1f, 0.0f);
             
            style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.0f, 0.46666667f, 0.78431374f, 1);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.11372549f, 0.5921569f, 0.9254902f, 1);
            style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.0f, 0.46666667f, 0.78431374f, 1);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.11372549f, 0.5921569f, 0.9254902f, 1);

            style.Colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.1882353f, 0.1882353f, 0.2f, 1);
            style.Colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.30980393f, 0.30980393f, 0.34901962f, 1);
            style.Colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.22745098f, 0.22745098f, 0.24705882f, 1);
            style.Colors[(int)ImGuiCol.TableRowBg] = new Vector4(0, 0, 0, 0);
            style.Colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1, 1, 1, 0.06f);

            style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.0f, 0.46666667f, 0.78431374f, 1);
            style.Colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.14509805f, 0.14509805f, 0.14901961f, 1);
            // style.Colors[(int)ImGuiCol.NavHighlight] = new Vector4(0.14509805f, 0.14509805f, 0.14901961f, 1);
            style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1, 1, 1, 0.7f);
            style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.8f, 0.8f, 0.8f, 0.2f);
            style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.14509805f, 0.14509805f, 0.14901961f, 0.2f);
        }
    }
}