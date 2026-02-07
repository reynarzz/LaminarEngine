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
        public override Task<BuildStageResult> Execute()
        {
            Debug.Log("Trying to install to device.");

            var settings = EditorDataManager.BuildSettings.GetBuildSettings(PlatformBuild.Android) as AndroidBuildSettings;
            var current = settings.GetCurrentBuildTypeSettings();

            var rootFolder = AndroidShipBuildStage.GetOutputFolder(settings);

            var filename = Path.Combine(rootFolder, current.PackageName);

            var value = AdbRunner.Run("adb", $"install {filename}-Signed.apk");
            var IsInstallSuccess = value.exitCode == 0;

            if (IsInstallSuccess)
            {
                Debug.Log("Installation Success.");

                var launchResult = AdbRunner.Run("adb", $"shell am start -n {current.PackageName}/crc64faceced24a29f4d5.MainActivity");

                if (launchResult.exitCode == 0)
                {
                    Debug.Log("App launch Success.");
                }
            }
            else
            {
                Debug.Log($"Couldn't install, check android device's compatibility.");
            }

            return Task.FromResult(new BuildStageResult()
            {
                IsSuccess = IsInstallSuccess,
            });
        }
    }
}
