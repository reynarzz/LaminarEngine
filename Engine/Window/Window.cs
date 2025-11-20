using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Graphics;
using Engine.Graphics.OpenGL;
using GLFW;
using OpenGL;

namespace Engine
{
    public class Window
    {
        public static bool IsFullScreen { get; private set; }
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        private static int _startWidth;
        private static int _startHeight;
        private static string _windowName = "Game";
        public static event Action<int, int> OnWindowChanged;
        private static bool _isInitialized = false;

        private static bool _isMouseVisible = true;
        public static bool MouseVisible
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
        public static string Name
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
        public static Color StartWindowColor { get; private set; }
        internal static bool ShouldClose => _isInitialized && Glfw.WindowShouldClose(NativeWindow);

        public int MonitorCount => Glfw.Monitors.Length;

        internal static GLFW.Window NativeWindow { get; private set; }
        public bool IsInitialized => _isInitialized;

        private static bool _canResize = false;
        public static bool CanResize
        {
            get => _canResize; set
            {
                if (_canResize == value)
                {
                    return;
                }
                _canResize = value;
                Glfw.SetWindowAttribute(NativeWindow, WindowAttribute.Resizable, _canResize);
            }
        }

        internal Window(string name, int width, int height, Color windowColor)
        {
            StartWindowColor = windowColor;
            _windowName = name;
            Width = width;
            Height = height;
            _startWidth = width;
            _startHeight = height;

            _isInitialized = Glfw.Init();

            if (!_isInitialized)
                return;

            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 2);
            Glfw.WindowHint(Hint.Resizable, false);
            Glfw.WindowHint(Hint.Visible, false);
            
            try
            {
                NativeWindow = Glfw.CreateWindow(Width, Height, name, GLFW.Monitor.None, default);
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
            Glfw.MakeContextCurrent(NativeWindow);

            GL.Import(Glfw.GetProcAddress);
            Glfw.SetFramebufferSizeCallback(NativeWindow, (win, width, height) => SetWindowSize(width, height));
            Glfw.SwapInterval(1);
            //Glfw.SetWindowAttribute(NativeWindow, WindowAttribute.Decorated, false);

            GL.glClearColor(windowColor.R, windowColor.G, windowColor.B, 1.0f);

            GL.glClear(GL.GL_COLOR_BUFFER_BIT);
            SwapBuffers();

            Glfw.ShowWindow(NativeWindow);
            //Glfw.SetWindowAttribute(NativeWindow, WindowAttribute.Decorated, true);
            Glfw.RequestWindowAttention(NativeWindow);
        }

        internal static void SwapBuffers()
        {
            Glfw.SwapBuffers(NativeWindow);
        }

        public static void SetWindowSize(int width, int height)
        {
            var mode = Glfw.GetVideoMode(Glfw.PrimaryMonitor);

            Width = Math.Clamp(width, 100, mode.Width);
            Height = Math.Clamp(height, 100, mode.Height);

            Glfw.SetWindowSize(NativeWindow, Width, Height);

            OnWindowChanged?.Invoke(Width, Height);
        }

        public static void SetWindowPosition(int x, int y)
        {
            Glfw.SetWindowPosition(NativeWindow, x, y);
        }

        public static void FullScreen(bool fullscreen, int monitorIndex = 0)
        {
            IsFullScreen = fullscreen;

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

                Width = mode.Width;
                Height = mode.Height;

                OnWindowChanged?.Invoke(Width, Height);

                // Switch to fullscreen
                Glfw.SetWindowMonitor(
                    NativeWindow,
                    monitor,
                    0, 0,
                    mode.Width,
                    mode.Height,
                    mode.RefreshRate
                );

            }
            else
            {
                Width = _startWidth;
                Height = _startHeight;
                OnWindowChanged?.Invoke(Width, Height);

                // Switch back to windowed mode
                Glfw.SetWindowMonitor(
                    NativeWindow,
                    GLFW.Monitor.None,
                    100,
                    100,
                    Width,
                    Height,
                    0
                );
            }
        }
    }
}
