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
    internal class AndroidInstallBuildStage : BuildStage
    {
        public override async Task<BuildStageResult> Execute()
        {
            Debug.Log("Trying to install to device.");

            var settings = EditorDataManager.BuildSettings.GetBuildSettings(PlatformBuild.Android) as AndroidBuildSettings;
            var current = settings.GetCurrentBuildTypeSettings();

            var rootFolder = AndroidProjectBuildStage.GetOutputFolder(settings);

            var filename = Path.Combine(rootFolder, current.PackageName);

            var value = AdbRunner.Run("adb", $"install {filename}-Signed.apk");
            AdbRunner.Run("adb", $"shell am start -n {current.PackageName}/crc64faceced24a29f4d5.MainActivity");

            Debug.Log($"Installing stage ended: {(value.exitCode == 0? "success": "fail")}");

            return new BuildStageResult()
            {
                IsSuccess = value.exitCode == 0
            };
        }
    }
}
