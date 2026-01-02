#if DESKTOP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Engine.Graphics;
using GLFW;
using OpenGL;

namespace Engine
{
    public class WindowStandalone : IWindow
    {
        private bool _isFullScreen;
        public bool IsFullScreen
        {
            get => _isFullScreen;
            set
            {
                if (_isFullScreen == value)
                    return;

                _isFullScreen = value;

                FullScreen(value);
            }
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
        private SizeCallback _onResizeCallback;
        private WindowCallback _closeCallback;
        private int _startWidth;
        private int _startHeight;
        private string _windowName = "Game";
        public event Action<int, int> OnWindowChanged;
        private static bool _isInitialized = false;
        private bool dragging = false;
        private double startCursorX, startCursorY;
        private int startWinX, startWinY;
        public event Action OnWindowClose;
        private bool _isMouseVisible = true;
        public bool CursorVisible
        {
            get => _isMouseVisible;

            set
            {
                _isMouseVisible = value;

                if (_isMouseVisible)
                {
                    Glfw.SetInputMode(NativeWindow, InputMode.Cursor, (int)CursorMode.Normal);
                }
                else
                {
                    Glfw.SetInputMode(NativeWindow, InputMode.Cursor, (int)CursorMode.Disabled);
                }
            }
        }
        public string Name
        {
            get => _windowName;
            set
            {
                if (_windowName == value)
                {
                    return;
                }

                _windowName = value;
                GLFW.Glfw.SetWindowTitle(NativeWindow, _windowName);
            }
        }
        public Color StartWindowColor { get; private set; }
        public bool ShouldClose => _isInitialized && Glfw.WindowShouldClose(NativeWindow);

        public int MonitorCount => Glfw.Monitors.Length;

        internal static GLFW.Window NativeWindow { get; private set; }
        public bool IsInitialized => _isInitialized;

        private bool _canResize = false;
        public bool CanResize
        {
            get => _canResize;
            set
            {
                if (_canResize == value)
                {
                    return;
                }
                _canResize = value;
                Glfw.SetWindowAttribute(NativeWindow, WindowAttribute.Resizable, _canResize);
            }
        }
        public int _physicalWidth;
        public int _physicalHeight;

        public int PhysicalWidth => _physicalWidth;
        public int PhysicalHeight => _physicalHeight;

        public int OffsetX => 0;
        public int OffsetY => 0;

        private void OnMouseButton(IntPtr window, GLFW.MouseButton button, GLFW.InputState state, GLFW.ModifierKeys modifiers)
        {
            if (button != GLFW.MouseButton.Left) return;

            if (state == GLFW.InputState.Press)
            {
                dragging = true;

                Glfw.GetCursorPosition(NativeWindow, out startCursorX, out startCursorY);
                Glfw.GetWindowPosition(NativeWindow, out startWinX, out startWinY);
            }
            else if (state == GLFW.InputState.Release)
            {
                dragging = false;
            }
        }

        private void OnCursorPos(IntPtr window, double xpos, double ypos)
        {
            if (!dragging) return;

            Glfw.GetCursorPosition(NativeWindow, out double curX, out double curY);

            double dx = curX - startCursorX;
            double dy = curY - startCursorY;

            int newX = (int)(startWinX + dx);
            int newY = (int)(startWinY + dy);

            Glfw.SetWindowPosition(NativeWindow, newX, newY);
        }
        internal WindowStandalone(string name, int width, int height, Color windowColor) : this(name, width, height, windowColor, null)
        {

        }
        internal WindowStandalone(string name, int width, int height, Color windowColor, TextureDescriptor image)
        {
            StartWindowColor = windowColor;
            _windowName = name;
            _startWidth = width;
            _startHeight = height;
            _physicalWidth = width;
            _physicalHeight = height;

            _isInitialized = Glfw.Init();

            if (!_isInitialized)
                return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Glfw.WindowHint(Hint.OpenglProfile, Profile.Compatibility);
                Glfw.WindowHint(Hint.ContextVersionMajor, 3);
                Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // NOTE: test this
                Glfw.WindowHint(Hint.OpenglProfile, Profile.Compatibility);
                Glfw.WindowHint(Hint.ContextVersionMajor, 3);
                Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
                Glfw.WindowHint(Hint.ContextVersionMajor, 3);
                Glfw.WindowHint(Hint.ContextVersionMinor, 2);
            }

            Glfw.WindowHint(Hint.Resizable, false);
            Glfw.WindowHint(Hint.Visible, false);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, Constants.True);
            Glfw.WindowHint(Hint.Samples, 4);

            //Glfw.WindowHint(Hint.SrgbCapable, Constants.True);
            //Glfw.WindowHint(Hint.Doublebuffer, Constants.True);

            try
            {
                NativeWindow = Glfw.CreateWindow(width, height, name, GLFW.Monitor.None, default);
            }
            catch
            {
                _isInitialized = false;
                return;
            }
            // Create a window
            if (NativeWindow == GLFW.Window.None)
            {
                Console.WriteLine("Failed to create GLFW window");
                Glfw.Terminate();
                return;
            }
            else
            {
                Console.WriteLine("GLFW window sucess");

            }
            Glfw.MakeContextCurrent(NativeWindow);
            // Glfw.SetMouseButtonCallback(NativeWindow, OnMouseButton);
            // Glfw.SetCursorPositionCallback(NativeWindow, OnCursorPos);

            GL.Import(Glfw.GetProcAddress);

            _onResizeCallback = FrameBufferSizeCallback;
            _closeCallback = OnCloseWindow;

            Glfw.SetFramebufferSizeCallback(NativeWindow, _onResizeCallback);

            Glfw.SwapInterval(1);
            // Glfw.SetWindowAttribute(NativeWindow, WindowAttribute.Decorated, false);

            GL.glClearColor(windowColor.R, windowColor.G, windowColor.B, 1.0f);
            GL.glClear(GL.GL_COLOR_BUFFER_BIT);
            SwapBuffers();

            Glfw.ShowWindow(NativeWindow);
            //Glfw.SetWindowAttribute(NativeWindow, WindowAttribute.Decorated, true);
            Glfw.RequestWindowAttention(NativeWindow);
            Glfw.SetCloseCallback(NativeWindow, _closeCallback);

            Glfw.GetFramebufferSize(NativeWindow, out width, out height);
            Width = width;
            Height = height;

            LoadIconRandom(image);
        }
        private void LoadIconRandom(TextureDescriptor image)
        {
            if (image == null)
                return;

            unsafe
            {
                fixed (byte* col = image.Buffer)
                {
                    Glfw.SetWindowIcon(NativeWindow, 1, [new Image(image.Width, image.Height, (nint)col)]);
                }
            }
        }
        public void SwapBuffers()
        {
            Glfw.SwapBuffers(NativeWindow);
        }

        private void OnCloseWindow(IntPtr x)
        {
            OnWindowClose?.Invoke();
        }
        private void FrameBufferSizeCallback(IntPtr win, int width, int height)
        {
            Width = width;
            Height = height;

            Glfw.GetWindowSize(NativeWindow, out int pW, out int pH);
            _physicalWidth = pW;
            _physicalHeight = pH;

            OnWindowChanged?.Invoke(Width, Height);
        }
        public void SetWindowSize(int width, int height)
        {
            var mode = Glfw.GetVideoMode(Glfw.PrimaryMonitor);

            Glfw.SetWindowSize(NativeWindow, width, height);

            _physicalWidth = width;
            _physicalHeight = height;

            Glfw.GetFramebufferSize(NativeWindow, out width, out height);
            Width = width;
            Height = height;

            OnWindowChanged?.Invoke(Width, Height);
        }

        public void SetWindowPosition(int x, int y)
        {
            Glfw.SetWindowPosition(NativeWindow, x, y);
        }

        public void FullScreen(bool fullscreen, int monitorIndex = 0)
        {
            // Glfw.SetWindowAttribute(NativeWindow, WindowAttribute.Decorated, !fullscreen);
            if (fullscreen)
            {
                if (Glfw.Monitors.Length <= monitorIndex)
                {
                    Debug.Error($"Monitor index '{monitorIndex}' is bigger than physical monitors '{Glfw.Monitors.Length}'.");
                    return;
                }

                // Get primary monitor and video mode
                GLFW.Monitor monitor = Glfw.Monitors[monitorIndex];
                var mode = Glfw.GetVideoMode(monitor);

                // Switch to fullscreen
                Glfw.SetWindowMonitor(
                    NativeWindow,
                    monitor,
                    0, 0,
                    mode.Width,
                    mode.Height,
                    mode.RefreshRate
                );


                Glfw.GetFramebufferSize(NativeWindow, out var width, out var height);
                Width = width;
                Height = height;
                _physicalWidth = Width;
                _physicalHeight = Height;
                /*
                _physicalWidth = mode.Width;
                _physicalHeight = mode.Height;
                */
                OnWindowChanged?.Invoke(Width, Height);

            }
            else
            {
                // Switch back to windowed mode
                Glfw.SetWindowMonitor(
                    NativeWindow,
                    GLFW.Monitor.None,
                    100,
                    100,
                    _startWidth,
                    _startHeight,
                    0
                );

                _physicalWidth = _startWidth;
                _physicalHeight = _startHeight;

                Glfw.GetFramebufferSize(NativeWindow, out var width, out var height);
                Width = width;
                Height = height;

                OnWindowChanged?.Invoke(Width, Height);
            }
        }

        public void CloseWindow()
        {
            Glfw.SetWindowShouldClose(NativeWindow, true);
        }
    }
}
#endif