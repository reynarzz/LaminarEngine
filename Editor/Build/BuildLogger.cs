using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor.Views;
using Engine;
using Microsoft.Build.Framework;

namespace Editor
{
    internal class BuildLogger : ILogger
    {
        public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;
        public bool DebugStatus { get; set; }
        public string Parameters { get; set; }

        public static string CurrentStatus { get; private set; } = string.Empty;

        public void Initialize(IEventSource source)
        {
            // source.BuildStarted += (_, e) => Debug.Info("Build started");
            // source.BuildFinished += (_, e) => Debug.Info($"Build finished: {e.Succeeded}");

            //source.ProjectStarted += (_, e) => Debug.Info($"Project: {e.ProjectFile}");
            //source.TargetStarted += (_, e) => Debug.Info($"Target: {e.TargetName}");

            source.ErrorRaised += (_, e) =>
            {
                // if (e.ProjectFile.EndsWith(EditorPaths.GAME_PROJECT_FULL_NAME))
                {
                    var index = e.File.IndexOf("Assets");
                    var error = $"{e.File.Substring(index, e.File.Length - index)}({e.LineNumber},{e.ColumnNumber}): {e.Message}";
                    Debug.Error(error);
                    ConsoleEditorView.AddError(error); // Remove from here

                }
            };

            source.WarningRaised += (_, e) =>
            {
                if (e.ProjectFile.EndsWith(EditorPaths.GAME_PROJECT_FULL_NAME))
                {
                    var index = e.File.IndexOf("Assets");
                    var warn = $"{e.File.Substring(index, e.File.Length - index)}({e.LineNumber},{e.ColumnNumber}): {e.Message}";

                    Debug.Warn(warn);

                    ConsoleEditorView.AddWarning(warn); // Remove from here
                }
            };

            if (DebugStatus)
            {
                source.StatusEventRaised += (_, e) =>
                {
                    CurrentStatus = e.Message;
                    Debug.Info($"{e.Message}");
                };
            }
        }

        public void Shutdown()
        {
        }
    }

}
