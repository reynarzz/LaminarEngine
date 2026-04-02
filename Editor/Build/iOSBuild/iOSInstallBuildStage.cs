using Editor.Data;
using Editor.Utils;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class iOSInstallBuildStage : BuildStage
    {
        public override Task<BuildStageResult> Execute()
        {
            Debug.Log("Trying to install to device.");

            return Task.FromResult(new BuildStageResult()
            {
                IsSuccess = true,
            });
        }
    }
}
