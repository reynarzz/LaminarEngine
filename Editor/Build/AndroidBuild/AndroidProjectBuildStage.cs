using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class AndroidProjectBuildStage : ProjectBuildStage
    {
        private readonly string[] _targets = ["SignAndroidPackage"];
            
        private readonly string[] _buildFilesExt = { ".aab", ".apk", ".idsig" };
        public AndroidProjectBuildStage() : base(new BuildLogger()
        {
            DebugStatus = true
        })
        { }

        protected override Dictionary<string, string> GetBuildProperties()
        {
            return new()
            {
                ["Configuration"] = "Release",
                ["Platform"] = "AnyCPU",
                ["AndroidSdkDirectory"] = GetAndroidSdkPath(),
                ["AndroidKeyStore"] = "false",
                ["AndroidSigningKeyAlias"] = "myalias",
                ["AndroidSigningKeyPass"] = "mypassword",
                ["AndroidSigningStorePass"] = "storepassword",
                ["OutputPath"] = EditorPaths.AndroidPublishFolderRoot + "/"
            };
        }

        protected override void OnBuildSuccess()
        {
            Directory.CreateDirectory(EditorPaths.ShipAndroidFolderRoot);

            foreach (var file in Directory.EnumerateFiles(EditorPaths.AndroidPublishFolderRoot))
            {
                if (_buildFilesExt.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                {
                    File.Copy(file, Path.Combine(EditorPaths.ShipAndroidFolderRoot, Path.GetFileName(file)), overwrite: true);
                }
            }
        }

        private string GetAndroidSdkPath()
        {
            var sdkPath = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT") ?? Environment.GetEnvironmentVariable("ANDROID_HOME");

            if (string.IsNullOrEmpty(sdkPath))
                throw new Exception("Android SDK path not found. Check Visual Studio Android settings.");

            return sdkPath;
        }

        protected override string GetCSProjPath()
        {
            return Path.Combine(EditorPaths.AndroidProjectRoot, EditorPaths.ANDROID_PROJECT_FULL_NAME);
        }

        protected override string[] GetTargetsToBuild()
        {
            return _targets;
        }
    }
}
