using Engine;
using GLFW;
using GlmNet;
using ImGuiNET;
using SharedTypes;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static OpenGL.GL;

namespace Editor
{
    public unsafe static class ImguiImplOpenGL3
    {
        // See: https://github.com/ImGuiNET/ImGui.NET/issues/527
        internal struct ImDrawCmd_fixed
        {
            public Vector4 ClipRect;
            public ulong TextureId;
            public uint VtxOffset;
            public uint IdxOffset;
            public uint ElemCount;
            public nint UserCallback;
            public unsafe void* UserCallbackData;
            public int UserCallbackDataSize;
            public int UserCallbackDataOffset;
        }
        internal struct ImDrawCmdPtr_fixed
        {
            public unsafe ImDrawCmd_fixed* NativePtr { get; }

            public unsafe nint GetTexID()
            {
                return ImGuiNative.ImDrawCmd_GetTexID((ImDrawCmd*)NativePtr);
            }
        }

        struct RendererData
        {
            public int FontTexture;
            public int ShaderHandle;
            public int UniformLocationTex;
            public int UniformLocationProjMtx;
            public int AttribLocationVtxPos;
            public int AttribLocationVtxUV;
            public int AttribLocationVtxColor;
            public int VboHandle;
            public int EboHandle;
            // FIXME: ??
            public bool HasPolygonMode;
            public bool HasClipOrigin;

            public int GlslVersion;
        }

        static RendererData* GetBackendData()
        {
            return ImGui.GetCurrentContext() == 0 ? null : (RendererData*)ImGui.GetIO().BackendRendererUserData;
        }
        static byte[] BackendName = "custom_impl_opengl3"u8.ToArray();
        private static IWindow _window;

        public static bool Init(IWindow window)
        {
            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            var root = Path.Combine(assemblyDir.Substring(0, assemblyDir.LastIndexOf("Editor")), "Editor/Data");

            _window = window;
            ImAllGui.InitImAllGui();

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

            RendererData* bd = (RendererData*)NativeMemory.AllocZeroed((uint)sizeof(RendererData));
            bd->GlslVersion = 330;

            io.BackendRendererUserData = (IntPtr)bd;
            io.NativePtr->BackendRendererName = (byte*)Unsafe.AsPointer(ref BackendName);

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
            io.BackendFlags |= ImGuiBackendFlags.RendererHasViewports;

            InitMultiViewportSupport();

            NewFrame();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

           // SetPerFrameImGuiData(1f / 60f, window.PhysicalWidth, window.PhysicalHeight);
            //Styles();
            Styles2();

            return true;
        }

        private static void Styles2()
        {
            var style = ImGui.GetStyle();
            var green = new Vector4(0.133f, 0.545f, 0.133f, 1.0f);
            style.Alpha = 1.0f;
            float rounding = 2.0f;

            style.DisabledAlpha = 0.6000000238418579f;
            style.WindowPadding = new Vector2(4.0f, 5.0f);
            style.WindowRounding = rounding;
            style.WindowBorderSize = 0.0f;
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
            style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.14509805f, 0.14509805f, 0.14901961f, 1);
            style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.14509805f, 0.14509805f, 0.14901961f, 1);
            style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.14509805f, 0.14509805f, 0.14901961f, 1);

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
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.2f, 0.2f, 0.21568628f, 1);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.32156864f, 0.32156864f, 0.33333334f, 1);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.3529412f, 0.3529412f, 0.37254903f, 1);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.3529412f, 0.3529412f, 0.37254903f, 1);

            style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.0f, 0.46666667f, 0.78431374f, 1);
            style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.11372549f, 0.5921569f, 0.9254902f, 1);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.0f, 0.46666667f, 0.78431374f, 1);

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

            var tabColor = new Vector4(0.133f, 0.545f, 0.133f, 1);
            var tabUnfocused = new Vector4(tabColor.X * 0.5f, tabColor.Y * 0.5f, tabColor.Z * 0.5f, 1);

            style.Colors[(int)ImGuiCol.TabDimmedSelected] = tabColor;
            style.Colors[(int)ImGuiCol.TabSelected] = tabColor;
            style.Colors[(int)ImGuiCol.Tab] = tabColor;
            style.Colors[(int)ImGuiCol.TabHovered] = tabColor;
            //style.Colors[(int)ImGuiCol.TabUnfocused] = tabUnfocused;
            //style.Colors[(int)ImGuiCol.TabUnfocusedActive] = tabColor;
            style.Colors[(int)ImGuiCol.TabSelectedOverline] = tabColor;
            style.Colors[(int)ImGuiCol.TabDimmed] = tabUnfocused;

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
        private static void Styles()
        {
            var style = ImGui.GetStyle();

            // Rounding

            float rounding = 2.0f;
            style.WindowRounding = rounding;
            style.ChildRounding = rounding;
            style.FrameRounding = rounding;
            style.PopupRounding = rounding;
            style.ScrollbarRounding = rounding;
            style.GrabRounding = rounding;
            style.TabRounding = rounding;

            // Sizes
            style.WindowPadding = new Vector2(8, 8);
            style.FramePadding = new Vector2(6, 4);
            style.ItemSpacing = new Vector2(8, 4);
            style.ItemInnerSpacing = new Vector2(6, 4);
            style.IndentSpacing = 18.0f;
            style.ScrollbarSize = 12.0f;
            style.GrabMinSize = 15.0f;
            style.TabMinWidthForCloseButton = 0.0f;

            // Borders
            style.WindowBorderSize = 0.0f;
            style.ChildBorderSize = 0.0f;
            style.PopupBorderSize = 0.0f;
            style.FrameBorderSize = 0.0f;
            style.TabBorderSize = 0.0f;

            // Colors
            var colors = style.Colors;

            // Backgrounds
            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.14509803f, 0.14509803f, 0.1490196f, 1.0f);
            colors[(int)ImGuiCol.ChildBg] = new Vector4(0.12f, 0.12f, 0.12f, 1.0f);
            colors[(int)ImGuiCol.PopupBg] = new Vector4(0.12f, 0.12f, 0.12f, 1.0f);

            // Frames 
            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.20f, 0.20f, 0.20f, 1.0f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.25f, 0.25f, 0.25f, 1.0f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.28f, 0.28f, 0.28f, 1.0f);

            // Titles
            colors[(int)ImGuiCol.TitleBg] = new Vector4(0.12f, 0.12f, 0.12f, 1.0f);
            colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.16f, 0.16f, 0.16f, 1.0f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.10f, 0.10f, 0.10f, 1.0f);

            // Buttons
            colors[(int)ImGuiCol.Button] = new Vector4(0.22f, 0.22f, 0.22f, 1.0f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.30f, 0.30f, 0.30f, 1.0f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.35f, 0.35f, 0.35f, 1.0f);

            // Tabs
            colors[(int)ImGuiCol.Tab] = new Vector4(0.16f, 0.16f, 0.16f, 1.0f);
            colors[(int)ImGuiCol.TabHovered] = new Vector4(0.26f, 0.26f, 0.26f, 1.0f);

            // Headers: tree nodes, selectable
            colors[(int)ImGuiCol.Header] = new Vector4(0.22f, 0.22f, 0.22f, 1.0f);
            colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.28f, 0.28f, 0.28f, 1.0f);
            colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.32f, 0.32f, 0.32f, 1.0f);

            // Selection
            colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.35f, 0.35f, 0.35f, 0.8f);

            // Scrollbar
            colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.10f, 0.10f, 0.10f, 0.2f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.30f, 0.30f, 0.30f, 0.5f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.40f, 0.40f, 0.40f, 1.0f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.45f, 0.45f, 0.45f, 1.0f);

            // Text
            colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.95f, 0.95f, 1.0f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.55f, 0.55f, 0.55f, 1.0f);

            // Resize grips
            colors[(int)ImGuiCol.ResizeGrip] = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

            // Docking splitter 
            colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.3f, 0.3f, 0.3f, 0.7f);

            // For the actual splitter lines
            colors[(int)ImGuiCol.Separator] = new Vector4(0.35f, 0.35f, 0.35f, 1.0f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.45f, 0.45f, 0.45f, 1.0f);
            colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.55f, 0.55f, 0.55f, 1.0f);
        }

        public static void SetPerFrameImGuiData(float deltaSeconds, int width, int height)
        {
            ImGuiIOPtr io = ImGui.GetIO();

            int winW, winH;
            int fbW, fbH;
            Glfw.GetWindowSize(WindowStandalone.NativeWindow, out winW, out winH);
            Glfw.GetFramebufferSize(WindowStandalone.NativeWindow, out fbW, out fbH);

            float scaleX = (float)fbW / winW;
            float scaleY = (float)fbH / winH;

            io.DisplayFramebufferScale = new Vector2(scaleX, scaleY);
            io.DisplaySize = new Vector2(width, height);
            io.DeltaTime = deltaSeconds;
        }

        public static void Shutdown()
        {
            var io = ImGui.GetIO();

            RendererData* bd = (RendererData*)io.NativePtr->BackendRendererUserData;

            ShutdownMultiViewportSupport();

            io.NativePtr->BackendRendererName = null;
            io.NativePtr->BackendRendererUserData = null;

            io.BackendFlags &= ~(ImGuiBackendFlags.RendererHasVtxOffset);

            NativeMemory.Free(bd);
        }

        public static void NewFrame()
        {
            RendererData* bd = GetBackendData();

            if (bd->ShaderHandle == 0)
            {
                CreateDeviceObjects();
            }
            if (bd->FontTexture == 0)
            {
                CreateFontsTexture();
            }
        }

        public static void SetupRenderState(ImDrawDataPtr drawData, int fbWidth, int fbHeight, int vao)
        {
            RendererData* bd = GetBackendData();

            glEnable(GL_BLEND);
            glBlendEquation(GL_FUNC_ADD);
            glBlendFuncSeparate(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA, GL_ONE, GL_ONE_MINUS_SRC_ALPHA);
            glDisable(GL_CULL_FACE);
            glDisable(GL_DEPTH_TEST);
            glDisable(GL_STENCIL_TEST);
            glEnable(GL_SCISSOR_TEST);
            // FIXME: check for 3.1
            glDisable(GL_PRIMITIVE_RESTART);

            glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);

            bool clip_origin_lower_left = true;
            //ClipOrigin clip_origin = (ClipOrigin)glGetInteger(GetPName.ClipOrigin);
            //if (clip_origin == ClipOrigin.UpperLeft)
            //    clip_origin_lower_left = false;

            glViewport(0, 0, fbWidth, fbHeight);
            float L = drawData.DisplayPos.X;
            float R = drawData.DisplayPos.X + drawData.DisplaySize.X;
            float T = drawData.DisplayPos.Y;
            float B = drawData.DisplayPos.Y + drawData.DisplaySize.Y;
            if (clip_origin_lower_left == false)
                (T, B) = (B, T); // Swap top and bottom if origin is upper left.
            mat4 mvp = glm.ortho(L, R, B, T, -1, 1);
            glUseProgram((uint)bd->ShaderHandle);
            glUniform1i(bd->UniformLocationTex, 0);
            glUniformMatrix4fv(bd->UniformLocationProjMtx, 1, false, mvp.to_array());

            glBindSampler(0, 0);

            glBindVertexArray((uint)vao);
            glBindBuffer(GL_ARRAY_BUFFER, (uint)bd->VboHandle);
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, (uint)bd->EboHandle);
            glEnableVertexAttribArray((uint)bd->AttribLocationVtxPos);
            glEnableVertexAttribArray((uint)bd->AttribLocationVtxUV);
            glEnableVertexAttribArray((uint)bd->AttribLocationVtxColor);
            glVertexAttribPointer((uint)bd->AttribLocationVtxPos, 2, GL_FLOAT, false, sizeof(ImDrawVert), 0);
            glVertexAttribPointer((uint)bd->AttribLocationVtxUV, 2, GL_FLOAT, false, sizeof(ImDrawVert), 8);
            glVertexAttribPointer((uint)bd->AttribLocationVtxColor, 4, GL_UNSIGNED_BYTE, true, sizeof(ImDrawVert), 16);
        }

        struct array4
        {
            public int _0, _1, _2, _3;
        }

        public static unsafe void RenderDrawData(ImDrawDataPtr drawData)
        {
            int fbWidth = (int)(drawData.DisplaySize.X * drawData.FramebufferScale.X);
            int fbHeight = (int)(drawData.DisplaySize.Y * drawData.FramebufferScale.Y);
            if (fbWidth <= 0 || fbHeight <= 0)
                return;

            int last_active_texture = glGetInteger(GL_ACTIVE_TEXTURE);
            glActiveTexture(GL_TEXTURE0);
            int last_program = glGetInteger(GL_CURRENT_PROGRAM);
            int last_texture = glGetInteger(GL_TEXTURE_BINDING_2D);
            int last_sampler = glGetInteger(GL_SAMPLER_BINDING);
            int last_array_buffer = glGetInteger(GL_ARRAY_BUFFER_BINDING);
            int last_vao = glGetInteger(GL_VERTEX_ARRAY_BINDING);

            // OpenGL 3.0 & 3.1 have separate polygon modes for front and back.
            array4 last_polygon_mode = default;
            glGetIntegerv(GL_POLYGON_MODE, &last_polygon_mode._0);

            array4 last_viewport = default;
            glGetIntegerv(GL_VIEWPORT, &last_viewport._0);

            array4 last_scissor_box = default;
            glGetIntegerv(GL_SCISSOR_BOX, &last_scissor_box._0);

            int last_blend_src_rgb;
            glGetIntegerv(GL_BLEND_SRC_RGB, &last_blend_src_rgb);

            int last_blend_dst_rgb;
            glGetIntegerv(GL_BLEND_DST_RGB, &last_blend_dst_rgb);

            int last_blend_src_alpha;
            glGetIntegerv(GL_BLEND_SRC_ALPHA, &last_blend_src_alpha);

            int last_blend_dst_alpha;
            glGetIntegerv(GL_BLEND_DST_ALPHA, &last_blend_dst_alpha);

            int last_blend_equation_rgb;
            glGetIntegerv(GL_BLEND_EQUATION_RGB, &last_blend_equation_rgb);

            int last_blend_equation_alpha;
            glGetIntegerv(GL_BLEND_EQUATION_ALPHA, &last_blend_equation_alpha);

            bool last_enable_blend = glIsEnabled(GL_BLEND);
            bool last_enable_cull_face = glIsEnabled(GL_CULL_FACE);
            bool last_enable_depth_test = glIsEnabled(GL_DEPTH_TEST);
            bool last_enable_stencil_test = glIsEnabled(GL_STENCIL_TEST);
            bool last_enable_scissor_test = glIsEnabled(GL_SCISSOR_TEST);

            // FIXME: Check for >= 3.1
            bool last_enable_primitive_restart = glIsEnabled(GL_PRIMITIVE_RESTART);

            int vao = (int)glGenVertexArray();
            SetupRenderState(drawData, fbWidth, fbHeight, vao);

            var clipOff = drawData.DisplayPos;
            var clipScale = drawData.FramebufferScale;


            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr drawList = drawData.CmdLists[n];

                nint vtx_buffer_size = drawList.VtxBuffer.Size * (int)sizeof(ImDrawVert);
                nint idx_buffer_size = drawList.IdxBuffer.Size * (int)sizeof(ushort);
                glBufferData(GL_ARRAY_BUFFER, (int)vtx_buffer_size, drawList.VtxBuffer.Data, GL_STREAM_DRAW);
                glBufferData(GL_ELEMENT_ARRAY_BUFFER, (int)idx_buffer_size, drawList.IdxBuffer.Data, GL_STREAM_DRAW);

                for (int cmd_i = 0; cmd_i < drawList.CmdBuffer.Size; cmd_i++)
                {
                    // FIXME: This is a hack to make 32-bit builds work. See: https://github.com/ImGuiNET/ImGui.NET/issues/527
                    ImDrawCmdPtr_fixed cmdPtr = new ImPtrVector<ImDrawCmdPtr_fixed>(drawList.NativePtr->CmdBuffer, sizeof(ImDrawCmd_fixed))[cmd_i];
                    ref ImDrawCmd_fixed cmd = ref Unsafe.AsRef<ImDrawCmd_fixed>(cmdPtr.NativePtr);
                    if (cmd.UserCallback != 0)
                    {
                        // FIXME: ...
                        nint ImDrawCallback_ResetRenderState = -8;
                        if (cmd.UserCallback == ImDrawCallback_ResetRenderState)
                        {
                            SetupRenderState(drawData, fbWidth, fbHeight, vao);
                        }
                        else
                        {
                            throw new NotImplementedException("User callbacks are not implemented yet...");
                        }
                    }
                    else
                    {
                        Vector2 clip_min = new((cmd.ClipRect.X - clipOff.X) * clipScale.X, (cmd.ClipRect.Y - clipOff.Y) * clipScale.Y);
                        Vector2 clip_max = new((cmd.ClipRect.Z - clipOff.X) * clipScale.X, (cmd.ClipRect.W - clipOff.Y) * clipScale.Y);
                        if (clip_max.X <= clip_min.X || clip_max.Y <= clip_min.Y)
                            continue;

                        glScissor((int)clip_min.X, (int)((float)fbHeight - clip_max.Y), (int)(clip_max.X - clip_min.X), (int)(clip_max.Y - clip_min.Y));

                        glBindTexture(GL_TEXTURE_2D, (uint)cmdPtr.GetTexID());

                        glDrawElementsBaseVertex(GL_TRIANGLES, (int)cmd.ElemCount, GL_UNSIGNED_SHORT, (void*)(cmd.IdxOffset * sizeof(ushort)), (int)cmd.VtxOffset);
                    }
                }
            }

            glDeleteVertexArray((uint)vao);

            if (last_program == 0 || glIsProgram((uint)last_program)) glUseProgram((uint)last_program);
            glBindTexture(GL_TEXTURE_2D, (uint)last_texture);
            glBindSampler(0, (uint)last_sampler);
            glActiveTexture(last_active_texture);
            glBindVertexArray((uint)last_vao);
            glBindBuffer(GL_ARRAY_BUFFER, (uint)last_array_buffer);
            glBlendEquationSeparate(last_blend_equation_rgb, last_blend_equation_alpha);
            glBlendFuncSeparate(last_blend_src_rgb, last_blend_dst_rgb, last_blend_src_alpha, last_blend_dst_alpha);
            if (last_enable_blend) glEnable(GL_BLEND); else glDisable(GL_BLEND);
            if (last_enable_cull_face) glEnable(GL_CULL_FACE); else glDisable(GL_CULL_FACE);
            if (last_enable_depth_test) glEnable(GL_DEPTH_TEST); else glDisable(GL_DEPTH_TEST);
            if (last_enable_stencil_test) glEnable(GL_STENCIL_TEST); else glDisable(GL_STENCIL_TEST);
            if (last_enable_scissor_test) glEnable(GL_SCISSOR_TEST); else glDisable(GL_SCISSOR_TEST);
            if (last_enable_primitive_restart) glEnable(GL_PRIMITIVE_RESTART); else glDisable(GL_PRIMITIVE_RESTART);

            if (true)
            {
                // FIXME:
                // if (bd->HasPolygonMode) {
                //     if (bd->GlVersion <= 310 || bd->GlProfileIsCompat) {
                //          glPolygonMode(GL_FRONT, (GLenum)last_polygon_mode[0]);
                //          glPolygonMode(GL_BACK, (GLenum)last_polygon_mode[1]);
                //     } else {
                //          glPolygonMode(GL_FRONT_AND_BACK, (GLenum)last_polygon_mode[0]);
                //     }
                // }
                glPolygonMode(GL_FRONT_AND_BACK, last_polygon_mode._0);
            }

            glViewport(last_viewport._0, last_viewport._1, last_viewport._2, last_viewport._3);
            glScissor(last_scissor_box._0, last_scissor_box._1, last_scissor_box._2, last_scissor_box._3);
        }

        static void CreateFontsTexture()
        {
            var io = ImGui.GetIO();
            RendererData* bd = GetBackendData();

            ImGuiNative.ImFontAtlas_AddFontDefault(io.Fonts.NativePtr, null);
            //io.Fonts.AddFontDefault();
            io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out int width, out int height);

            int last_texture = glGetInteger(GL_TEXTURE_BINDING_2D);
            bd->FontTexture = (int)glGenTexture();
            glBindTexture(GL_TEXTURE_2D, (uint)bd->FontTexture);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
            glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, (IntPtr)pixels);

            io.Fonts.SetTexID(bd->FontTexture);

            glBindTexture(GL_TEXTURE_2D, (uint)last_texture);
        }

        static void DestroyFontsTexture()
        {
            var io = ImGui.GetIO();
            RendererData* bd = GetBackendData();

            if (bd->FontTexture != 0)
            {
                glDeleteTexture((uint)bd->FontTexture);
                io.Fonts.SetTexID(0);
                bd->FontTexture = 0;
            }
        }

        static bool CheckShader(int handle, string desc)
        {
            int status = 0;
            int logLength = 0;

            glGetShaderiv((uint)handle, GL_COMPILE_STATUS, &status);
            glGetShaderiv((uint)handle, GL_INFO_LOG_LENGTH, &logLength);
            if (status == 0)
            {
                Console.Error.WriteLine($"ERROR: ImguiImplOpenGL3.CheckShader: Failed to compile {desc}!");
            }
            if (logLength > 1)
            {
                string log = glGetShaderInfoLog((uint)handle);
                Console.Error.WriteLine(log);
            }
            return status == 1;
        }

        static bool CheckProgram(int handle, string desc)
        {
            int status = 0;
            int logLength = 0;
            glGetProgramiv((uint)handle, GL_LINK_STATUS, &status);
            glGetProgramiv((uint)handle, GL_INFO_LOG_LENGTH, &logLength);
            if (status == 0)
            {
                Console.Error.WriteLine($"ERROR: ImguiImplOpenGL3.CheckProgram: Failed to link {desc}!");
            }
            if (logLength > 1)
            {
                string log = glGetProgramInfoLog((uint)handle);
                Console.Error.WriteLine(log);
            }
            return status == 1;
        }

        static void CreateDeviceObjects()
        {
            RendererData* bd = GetBackendData();

            int last_texture = glGetInteger(GL_TEXTURE_BINDING_2D);
            int last_array_buffer = glGetInteger(GL_ARRAY_BUFFER_BINDING);
            int last_pixel_unpack_buffer = glGetInteger(GL_PIXEL_UNPACK_BUFFER_BINDING);
            int last_vertex_array = glGetInteger(GL_VERTEX_ARRAY_BINDING);

            string vertex_shader_glsl_120 =
                """
                uniform mat4 ProjMtx;
                attribute vec2 Position;
                attribute vec2 UV;
                attribute vec4 Color;
                varying vec2 Frag_UV;
                varying vec4 Frag_Color;
                void main()
                {
                    Frag_UV = UV;
                    Frag_Color = Color;
                    gl_Position = vec4(Position.xy,0,1) * ProjMtx;
                }
                """;

            string vertex_shader_glsl_130 =
                """
                uniform mat4 ProjMtx;
                in vec2 Position;
                in vec2 UV;
                in vec4 Color;
                out vec2 Frag_UV;
                out vec4 Frag_Color;
                void main()
                {
                    Frag_UV = UV;
                    Frag_Color = Color;
                    gl_Position = vec4(Position.xy,0,1) * ProjMtx;
                }
                """;

            string vertex_shader_glsl_300_es =
                """
                precision highp float;
                layout(location = 0) in vec2 Position;
                layout(location = 1) in vec2 UV;
                layout(location = 2) in vec4 Color;
                uniform mat4 ProjMtx;
                out vec2 Frag_UV;
                out vec4 Frag_Color;
                void main()
                {
                    Frag_UV = UV;
                    Frag_Color = Color;
                    gl_Position = vec4(Position.xy,0,1) * ProjMtx;
                }
                """;

            string vertex_shader_glsl_410_core =
                """
                layout(location = 0) in vec2 Position;
                layout(location = 1) in vec2 UV;
                layout(location = 2) in vec4 Color;
                uniform mat4 ProjMtx;
                out vec2 Frag_UV;
                out vec4 Frag_Color;
                void main()
                {
                    Frag_UV = UV;
                    Frag_Color = Color;
                    gl_Position = vec4(Position.xy,0,1) * ProjMtx;
                }
                """;

            string fragment_shader_glsl_120 =
                """
                #ifdef GL_ES
                    precision mediump float;
                #endif
                uniform sampler2D Texture;
                varying vec2 Frag_UV;
                varying vec4 Frag_Color;
                void main()
                {
                    gl_FragColor = Frag_Color * texture2D(Texture, Frag_UV.st);
                }
                """;

            string fragment_shader_glsl_130 =
                """
                uniform sampler2D Texture;
                in vec2 Frag_UV;
                in vec4 Frag_Color;
                out vec4 Out_Color;
                void main()
                {
                    Out_Color = Frag_Color * texture2D(Texture, Frag_UV.st);
                }
                """;

            string fragment_shader_glsl_300_es =
                """
                precision mediump float;
                uniform sampler2D Texture;
                in vec2 Frag_UV;
                in vec4 Frag_Color;
                layout(location = 0) out vec4 Out_Color;
                void main()
                {
                    Out_Color = Frag_Color * texture2D(Texture, Frag_UV.st);
                }
                """;

            string fragment_shader_glsl_410_core =
                """
                in vec2 Frag_UV;
                in vec4 Frag_Color;
                uniform sampler2D Texture;
                layout(location = 0) out vec4 Out_Color;
                void main()
                {
                    Out_Color = Frag_Color * texture2D(Texture, Frag_UV.st);
                }
                """;


            string fragment_330 = @"#version 330 core
            in vec2 Frag_UV;
            in vec4 Frag_Color;

            uniform sampler2D Texture;

            out vec4 Out_Color;

            void main()
            {
                Out_Color = Frag_Color * texture(Texture, Frag_UV);
            }
            ";

            string vertex_330 = @"#version 330 core

            layout (location = 0) in vec2 Position;
            layout (location = 1) in vec2 UV;
            layout (location = 2) in vec4 Color;

            uniform mat4 ProjMtx;

            out vec2 Frag_UV;
            out vec4 Frag_Color;

            void main()
            {
                Frag_UV = UV;
                Frag_Color = Color;
                gl_Position = ProjMtx * vec4(Position.xy, 0.0, 1.0);
            }

            ";

            string vertex_shader;
            string fragment_shader;
            if (bd->GlslVersion < 130)
            {
                vertex_shader = vertex_shader_glsl_120;
                fragment_shader = fragment_shader_glsl_120;
            }
            else if (bd->GlslVersion >= 410)
            {
                vertex_shader = vertex_shader_glsl_410_core;
                fragment_shader = fragment_shader_glsl_410_core;
            }
            else if (bd->GlslVersion == 300)
            {
                vertex_shader = vertex_shader_glsl_300_es;
                fragment_shader = fragment_shader_glsl_300_es;
            }
            else if (bd->GlslVersion == 330)
            {
                vertex_shader = vertex_330;
                fragment_shader = fragment_330;
            }
            else
            {
                vertex_shader = vertex_330;
                fragment_shader = fragment_330;
            }

            if (bd->GlslVersion != 330)
            {
                vertex_shader = vertex_shader.Insert(0, $"#version {bd->GlslVersion}{Environment.NewLine}");
                fragment_shader = fragment_shader.Insert(0, $"#version {bd->GlslVersion}{Environment.NewLine}");
            }

            uint vert = glCreateShader(GL_VERTEX_SHADER);
            // FIXME: Version string...
            glShaderSource(vert, vertex_shader);
            glCompileShader(vert);
            CheckShader((int)vert, "vertex shader");

            uint frag = glCreateShader(GL_FRAGMENT_SHADER);
            // FIXME: Version string...
            glShaderSource((uint)frag, fragment_shader);
            glCompileShader((uint)frag);
            CheckShader((int)frag, "fragment shader");

            bd->ShaderHandle = (int)glCreateProgram();
            glAttachShader((uint)bd->ShaderHandle, (uint)vert);
            glAttachShader((uint)bd->ShaderHandle, (uint)frag);
            glLinkProgram((uint)bd->ShaderHandle);
            CheckProgram(bd->ShaderHandle, "shader program");

            glDetachShader((uint)bd->ShaderHandle, (uint)vert);
            glDetachShader((uint)bd->ShaderHandle, (uint)frag);
            glDeleteShader((uint)vert);
            glDeleteShader((uint)frag);

            bd->UniformLocationTex = glGetUniformLocation((uint)bd->ShaderHandle, "Texture");
            bd->UniformLocationProjMtx = glGetUniformLocation((uint)bd->ShaderHandle, "ProjMtx");
            bd->AttribLocationVtxPos = glGetAttribLocation((uint)bd->ShaderHandle, "Position");
            bd->AttribLocationVtxUV = glGetAttribLocation((uint)bd->ShaderHandle, "UV");
            bd->AttribLocationVtxColor = glGetAttribLocation((uint)bd->ShaderHandle, "Color");

            bd->VboHandle = (int)glGenBuffer();
            bd->EboHandle = (int)glGenBuffer();

            CreateFontsTexture();

            glBindTexture(GL_TEXTURE_2D, (uint)last_texture);
            glBindBuffer(GL_ARRAY_BUFFER, (uint)last_array_buffer);
            glBindBuffer(GL_PIXEL_UNPACK_BUFFER, (uint)last_pixel_unpack_buffer);
            glBindVertexArray((uint)last_vertex_array);
        }

        static void DestroyDeviceObjects()
        {
            RendererData* bd = GetBackendData();

            if (bd->VboHandle != 0)
            {
                glDeleteBuffer((uint)bd->VboHandle);
                bd->VboHandle = 0;
            }

            if (bd->EboHandle != 0)
            {
                glDeleteBuffer((uint)bd->EboHandle);
                bd->EboHandle = 0;
            }

            if (bd->ShaderHandle != 0)
            {
                glDeleteProgram((uint)bd->ShaderHandle);
                bd->ShaderHandle = 0;
            }

            DestroyFontsTexture();
        }

        static void InitMultiViewportSupport()
        {
            var platformIO = ImGui.GetPlatformIO();
            platformIO.Renderer_RenderWindow = (IntPtr)(delegate* unmanaged[Cdecl]<ImGuiViewportPtr, void>)&Renderer_RenderWindow;
        }

        static void ShutdownMultiViewportSupport()
        {
            ImGui.DestroyPlatformWindows();
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static void Renderer_RenderWindow(ImGuiViewportPtr viewport)
        {
            if (viewport.Flags.HasFlag(ImGuiViewportFlags.NoRendererClear))
            {
                glClearColor(0, 0, 0, 1);
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            }
            RenderDrawData(viewport.DrawData);
        }
    }
}