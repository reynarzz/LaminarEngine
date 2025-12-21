using System;
using GLFW;
using ImGuiNET;

namespace Editor
{
    public sealed class ImGuiGLFW
    {
        private readonly Window _window;
        private readonly Cursor[] _mouseCursors = new Cursor[(int)ImGuiMouseCursor.COUNT];

        private MouseButtonCallback _mouseButtonCb;
        private MouseCallback _scrollCb;
        private KeyCallback _keyCb;
        private CharCallback _charCb;

        public ImGuiGLFW(Window window)
        {
            _window = window;
        }

        public void Init(bool installCallbacks = true)
        {
            var io = ImGui.GetIO();

            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;
            io.KeyRepeatDelay = 0.35f; // default ~0.25
            io.KeyRepeatRate = 0.05f; // default ~0.05
            CreateCursors();

            if (installCallbacks)
            {
                InstallCallbacks();
            }
        }

        private void CreateCursors()
        {
            _mouseCursors[(int)ImGuiMouseCursor.Arrow] = Glfw.CreateStandardCursor(CursorType.Arrow);
            _mouseCursors[(int)ImGuiMouseCursor.TextInput] = Glfw.CreateStandardCursor(CursorType.Beam);
            _mouseCursors[(int)ImGuiMouseCursor.ResizeNS] = Glfw.CreateStandardCursor(CursorType.ResizeVertical);
            _mouseCursors[(int)ImGuiMouseCursor.ResizeEW] = Glfw.CreateStandardCursor(CursorType.ResizeHorizontal);
            _mouseCursors[(int)ImGuiMouseCursor.Hand] = Glfw.CreateStandardCursor(CursorType.Hand);
        }

        private void InstallCallbacks()
        {
            _mouseButtonCb = OnMouseButton;
            _scrollCb = OnScroll;
            _keyCb = OnKey;
            _charCb = OnChar;

            Glfw.SetMouseButtonCallback(_window, _mouseButtonCb);
            Glfw.SetScrollCallback(_window, _scrollCb);
            Glfw.SetKeyCallback(_window, _keyCb);
            Glfw.SetCharCallback(_window, _charCb);
        }

        private void OnMouseButton(IntPtr wnd, MouseButton button, InputState state, ModifierKeys mods)
        {
            var io = ImGui.GetIO();
            int index = (int)button;
            if (index >= 0 && index < 5)
            {
                io.AddMouseButtonEvent(index, state == InputState.Press);
            }
        }

        private void OnScroll(IntPtr wnd, double x, double y)
        {
            ImGui.GetIO().AddMouseWheelEvent((float)x, (float)y);
        }

        private void OnKey(IntPtr wnd, Keys key, int scancode, InputState state, ModifierKeys mods)
        {
            var io = ImGui.GetIO();

            // Ignore repeats: ImGui handles key repeat internally
            if (state == InputState.Repeat)
                return;

            if (TryMapKey(key, out ImGuiKey imguiKey))
            {
                io.AddKeyEvent(imguiKey, state == InputState.Press);
            }

            // Modifiers (must be updated every key event)
            io.AddKeyEvent(ImGuiKey.ModCtrl, mods.HasFlag(ModifierKeys.Control));
            io.AddKeyEvent(ImGuiKey.ModShift, mods.HasFlag(ModifierKeys.Shift));
            io.AddKeyEvent(ImGuiKey.ModAlt, mods.HasFlag(ModifierKeys.Alt));
            io.AddKeyEvent(ImGuiKey.ModSuper, mods.HasFlag(ModifierKeys.Super));
        }


        private void OnChar(IntPtr wnd, uint c)
        {
            ImGui.GetIO().AddInputCharacter(c);
        }

        public void NewFrame()
        {
            var io = ImGui.GetIO();

            double x, y;
            Glfw.GetCursorPosition(_window, out x, out y);
            io.AddMousePosEvent((float)x, (float)y);

            UpdateMouseCursor();
        }

        private void UpdateMouseCursor()
        {
            var io = ImGui.GetIO();

            if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.NoMouseCursorChange))
            {
                return;
            }

            var cursor = ImGui.GetMouseCursor();
            if (cursor == ImGuiMouseCursor.None || io.MouseDrawCursor)
            {
                Glfw.SetInputMode(_window, InputMode.Cursor, (int)CursorMode.Hidden);
            }
            else
            {
                Glfw.SetCursor(_window, _mouseCursors[(int)cursor]);
                Glfw.SetInputMode(_window, InputMode.Cursor, (int)CursorMode.Normal);
            }
        }

        private static bool TryMapKey(Keys key, out ImGuiKey imguiKey)
        {
            imguiKey = key switch
            {
                Keys.Tab => ImGuiKey.Tab,
                Keys.Left => ImGuiKey.LeftArrow,
                Keys.Right => ImGuiKey.RightArrow,
                Keys.Up => ImGuiKey.UpArrow,
                Keys.Down => ImGuiKey.DownArrow,
                Keys.PageUp => ImGuiKey.PageUp,
                Keys.PageDown => ImGuiKey.PageDown,
                Keys.Home => ImGuiKey.Home,
                Keys.End => ImGuiKey.End,
                Keys.Insert => ImGuiKey.Insert,
                Keys.Delete => ImGuiKey.Delete,
                Keys.Backspace => ImGuiKey.Backspace,
                Keys.Space => ImGuiKey.Space,
                Keys.Enter => ImGuiKey.Enter,
                Keys.Escape => ImGuiKey.Escape,
                Keys.A => ImGuiKey.A,
                Keys.C => ImGuiKey.C,
                Keys.V => ImGuiKey.V,
                Keys.X => ImGuiKey.X,
                Keys.Y => ImGuiKey.Y,
                Keys.Z => ImGuiKey.Z,
                _ => ImGuiKey.None
            };

            return imguiKey != ImGuiKey.None;
        }
    }
}
