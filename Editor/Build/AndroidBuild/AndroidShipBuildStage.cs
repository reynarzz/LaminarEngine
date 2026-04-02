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

        protected override void GetAllBuildProperties(out Dictionary<string, string> props, out PlatformBuildSettings settings)
        {
            var settingsAndroid = GetBuildSettings<AndroidBuildSettings>(PlatformBuild.Android);
            settings = settingsAndroid;
            var buildTypeSettings = settingsAndroid.GetCurrentBuildTypeSettings();

            var packageName = AndroidConsts.DEFAULT_APP_PACKAGE_NAME;

            if (!string.IsNullOrEmpty(buildTypeSettings.PackageName))
            {
                packageName = buildTypeSettings.PackageName;
            }

            props = new Dictionary<string, string>()
            {
                ["Configuration"] = settingsAndroid.Type == BuildType.Release ? "Release" : "Debug",
                ["Platform"] = "AnyCPU",
                ["AndroidSdkDirectory"] = GetAndroidSdkPath(),
                ["AndroidKeyStore"] = "false",
                ["AndroidSigningKeyAlias"] = settingsAndroid.KeyAlias,
                ["AndroidSigningKeyPass"] = settingsAndroid.KeyPass,
                ["AndroidSigningStorePass"] = settingsAndroid.StorePass,
                ["OutputPath"] = EditorPaths.AndroidPublishFolderRoot + "/",
                ["AndroidApplicationLabel"] = buildTypeSettings.ApplicationName,
                ["DefineConstants"] = "$(DefineConstants);ANDROID;MOBILE",
                ["ApplicationId"] = packageName,
                ["ApplicationDisplayVersion"] = GetVersion(settingsAndroid.Version),
                ["SupportedOSPlatformVersion"] = ((int)settingsAndroid.MinimumApiLevel).ToString(),
                ["AndroidTargetSdkVersion"] = ((int)settingsAndroid.TargetApiLevel).ToString(),
                ["AndroidBuild"] = "true",
                ["BUILD_MOBILE"] = "true",
            };

            if (settings.NativeAOT)
            {
                props["AndroidAotMode"] = "Normal";
                props["AotAssemblies"] = "true"; 
                props["EnableLLVM"] = "true";
                props["AndroidLinkMode"] = "SdkAndUser";
                props["AndroidEnableProfiledAot"] = "false";
            }

            // NOTE: Not sure why do I have to define build type, msbuild should do it by default.
            if (settingsAndroid.Type == BuildType.Release)
            {
                props["DefineConstants"] = "ANDROID;MOBILE;RELEASE";
                props["DebugType"] = "None";
                props["Optimize"] = "true";
                props["AndroidStripMode"] = "All";
                props["PublishTrimmed"] = "true";
                props["TrimMode"] = "link";
            }
            else
            {
                props["DefineConstants"] = "ANDROID;MOBILE;DEBUG";
                props["DebugSymbols"] = "true";
                props["DebugType"] = "Embedded";
                props["Optimize"] = "false";
                props["AndroidStripMode"] = "None";
            }
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
        internal const string DEFAULT_APP_PACKAGE_NAME = $"com.application.company";
        internal const string BUILD_TARGET = "SignAndroidPackage";
        internal const string INSTALL_TARGET = "Install";
        internal const string START_TARGET = "Start";
    }
}
