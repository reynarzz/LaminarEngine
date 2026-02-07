using Editor.Data;
using GlmNet;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class AndroidShipBuildStage : ShipBuildStage
    {
        private readonly string[] _buildFilesExt = { ".aab", ".apk", ".idsig" };
        private readonly string[] _targets = [AndroidConsts.BUILD_TARGET];
        public AndroidShipBuildStage() : base(new BuildLogger()
        {
            DebugStatus = true
        })
        { }

        protected override Dictionary<string, string> GetAllBuildProperties()
        {
            var settings = GetBuildSettings<AndroidBuildSettings>(PlatformBuild.Android);
            var buildTypeSettings = settings.GetCurrentBuildTypeSettings();

            var packageName = AndroidConsts.DEFAULT_APP_PACKAGE_NAME;

            if (!string.IsNullOrEmpty(buildTypeSettings.PackageName))
            {
                packageName = buildTypeSettings.PackageName;
            }

            var properties = new Dictionary<string, string>()
            {
                ["Configuration"] = settings.Type == BuildType.Release ? "Release" : "Debug",
                ["Platform"] = "AnyCPU",
                ["AndroidSdkDirectory"] = GetAndroidSdkPath(),
                ["AndroidKeyStore"] = "false",
                ["AndroidSigningKeyAlias"] = settings.KeyAlias,
                ["AndroidSigningKeyPass"] = settings.KeyPass,
                ["AndroidSigningStorePass"] = settings.StorePass,
                ["OutputPath"] = EditorPaths.AndroidPublishFolderRoot + "/",
                ["AndroidApplicationLabel"] = buildTypeSettings.ApplicationName,
                ["PublishTrimmed"] = "true",
                ["DefineConstants"] = "$(DefineConstants);ANDROID;MOBILE",
                ["TrimMode"] = "link",
                ["ApplicationId"] = packageName,
                ["ApplicationDisplayVersion"] = GetVersion(settings.Version),
                ["SupportedOSPlatformVersion"] = ((int)settings.MinimumApiLevel).ToString(),
                ["AndroidTargetSdkVersion"] = ((int)settings.TargetApiLevel).ToString(),
            };

            // NOTE: Not sure why do I have to define build type, msbuild should do it by default.
            if(settings.Type == BuildType.Release)
            {
                properties["DefineConstants"] = "$(DefineConstants);ANDROID;MOBILE;RELEASE";
            }
            else
            {
                properties["DefineConstants"] = "$(DefineConstants);ANDROID;MOBILE;DEBUG";
            }
            return properties;
        }

        protected override void OnBeforeBuild()
        {
            var buildTypeSettings = GetBuildSettings<AndroidBuildSettings>(PlatformBuild.Android).GetCurrentBuildTypeSettings();
            UpdateAndroidAppName(buildTypeSettings.ApplicationName);
        }
        protected override void OnBuildSuccess()
        {
            
            var settings = GetBuildSettings<AndroidBuildSettings>(PlatformBuild.Android);
            var rootOutputFolder = GetOutputFolder(settings);
            Directory.CreateDirectory(rootOutputFolder);

            foreach (var file in Directory.EnumerateFiles(EditorPaths.AndroidPublishFolderRoot))
            {
                if (_buildFilesExt.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                {
                    File.Copy(file, Path.Combine(rootOutputFolder, Path.GetFileName(file)), overwrite: true);
                }
            }
        }

        internal static string GetOutputFolder(AndroidBuildSettings settings) // ugly
        {
            var rootOutputFolder = EditorPaths.AndroidShipFolderRoot;
            var buildTypeSettings = settings.GetCurrentBuildTypeSettings();
            if (!string.IsNullOrEmpty(buildTypeSettings.OutputPath))
            {
                rootOutputFolder = Paths.ClearPathSeparation(buildTypeSettings.OutputPath);
            }
            return rootOutputFolder;
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

        private string GetVersion(ivec3 version)
        {
            return $"{version.x}.{version.y}.{version.z}.{0}";
        }
        private void UpdateAndroidAppName(string name)
        {
            var stringsXmlPath = Path.Combine(EditorPaths.AndroidProjectRoot, "Resources", "values", "strings.xml");
            File.WriteAllText(stringsXmlPath, BuildAndroidStringsXml(name));
        }
        private string BuildAndroidStringsXml(string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = AndroidConsts.DEFAULT_APP_NAME;
            }
            var escapedName = SecurityElement.Escape(applicationName);

            //return $@"<resources>
            //            <string name=""app_name"">{escapedName}</string>
            //            <string name=""app_text"">{escapedName}</string>
            //          </resources>";

            return $@"<resources>
	<string name=""app_name"">{escapedName}</string>
	<string name=""app_text"">{escapedName}</string>
</resources>";
        }
    }
    public class AndroidConsts
    {
        internal const string DEFAULT_APP_NAME = "Application";
        internal const string DEFAULT_APP_PACKAGE_NAME = "com.application.gfs";
        internal const string BUILD_TARGET = "SignAndroidPackage";
        internal const string INSTALL_TARGET = "Install";
        internal const string START_TARGET = "Start";
    }
}
