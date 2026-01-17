using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Editor.AssemblyHotReload
{
    internal class BuildLogger : ILogger
    {
        public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;
        public string Parameters { get; set; }

        public void Initialize(IEventSource source)
        {
            // source.BuildStarted += (_, e) => Debug.Info("Build started");
            // source.BuildFinished += (_, e) => Debug.Info($"Build finished: {e.Succeeded}");

            //source.ProjectStarted += (_, e) => Debug.Info($"Project: {e.ProjectFile}");
            //source.TargetStarted += (_, e) => Debug.Info($"Target: {e.TargetName}");

            source.ErrorRaised += (_, e) => Debug.Error($"{e.File}({e.LineNumber},{e.ColumnNumber}): {e.Message}");
            source.WarningRaised += (_, e) => Debug.Warn($"{e.File}({e.LineNumber},{e.ColumnNumber}): {e.Message}");
            //source.StatusEventRaised += (_, e) => Debug.Info($"Status: {e.Message}");
        }

        public void Shutdown()
        {
        }
    }

}
