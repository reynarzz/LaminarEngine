using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using GlmNet;
using SharedTypes;

namespace Engine
{
    public static partial class Debug
    {
        internal enum LogLevel
        {
            Debug,
            Info,
            Warn,
            Error,
            EngineError,
            Success
        }

        private static readonly object _lock = new object();
        public static string Prefix { get; set; } = "";
        public static void Info<T>(T message,
                                [CallerFilePath] string file = "",
                                [CallerLineNumber] int line = 0,
                                [CallerMemberName] string member = "")
        {
            LogMessage(LogLevel.Info, message, file, line, member);
        }

        public static void Log<T>(T message,
                                [CallerFilePath] string file = "",
                                [CallerLineNumber] int line = 0,
                                [CallerMemberName] string member = "")
        {
            LogMessage(LogLevel.Debug, message, file, line, member);
        }

        public static void Warn<T>(T message,
                                [CallerFilePath] string file = "",
                                [CallerLineNumber] int line = 0,
                                [CallerMemberName] string member = "")
        {
            LogMessage(LogLevel.Warn, message, file, line, member);
        }
        public static void Error<T>(T message,
                                [CallerFilePath] string file = "",
                                [CallerLineNumber] int line = 0,
                                [CallerMemberName] string member = "")
        {
            LogMessage(LogLevel.Error, message, file, line, member);
        }

        internal static void EngineError<T>(T message,
                                [CallerFilePath] string file = "",
                                [CallerLineNumber] int line = 0,
                                [CallerMemberName] string member = "")
        {
            LogMessage(LogLevel.EngineError, message, file, line, member);
        }

        public static void Success<T>(T message,
                                [CallerFilePath] string file = "",
                                [CallerLineNumber] int line = 0,
                                [CallerMemberName] string member = "")
        {
            LogMessage(LogLevel.Success, message, file, line, member);
        }
        
        private static void LogMessage<T>(LogLevel level, T message,
                                string file = "",
                                int line = 0,
                                string member = "")
        {
#if DEBUG
            lock (_lock) // thread-safe color changes
            {
#if !MOBILE
                var prevColor = Console.ForegroundColor;
                Console.ForegroundColor = LevelToColor(level);
#endif
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                string filename = "";
#if ANDROID
                var clearedpath = Paths.ClearPathSeparation(file);
                var index = clearedpath.LastIndexOf('/');
                filename = clearedpath.Substring(index + 1, clearedpath.Length - index -1);
#else
                filename = System.IO.Path.GetFileName(file);
#endif
                //Console.WriteLine($"[{timestamp}] [{level}] {filename}:{line} ({member}) - {message}");
                Console.WriteLine($"{Prefix}[{timestamp}] [{level}] [{filename}:{line}] {message}");
#if !MOBILE
                Console.ForegroundColor = prevColor;
#endif
            }
#endif
            }

        private static ConsoleColor LevelToColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warn => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.EngineError => ConsoleColor.DarkRed,
                LogLevel.Success => ConsoleColor.Green,
                _ => ConsoleColor.White
            };
        }

    }
}