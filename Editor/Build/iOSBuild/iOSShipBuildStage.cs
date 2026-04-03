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
    public class iOSConsts
    {
        internal const string DEFAULT_APP_NAME = "Application";
        internal const string DEFAULT_APP_PACKAGE_NAME = "com.application.company";
        internal const string BUILD_TARGET = "Build";
        internal const string PUBLISH_TARGET = "Publish";
        internal const string INSTALL_TARGET = "Install";
        internal const string START_TARGET = "Start";
    }

    internal class iOSShipBuildStage : ShipBuildStage
    {
        private readonly string[] _buildFilesExt = { ".ipa", ".dSYM" };
        private readonly string[] _targets = [iOSConsts.BUILD_TARGET];

        public iOSShipBuildStage() : base(new BuildLogger { DebugStatus = true })
        {
        }

        protected override void GetAllBuildProperties(out Dictionary<string, string> props, out PlatformBuildSettings settings)
        {
            var settingsiOS = GetBuildSettings<iOSBuildSettings>(PlatformBuild.IOS);
            settings = settingsiOS;
            var buildTypeSettings = settingsiOS.GetCurrentBuildTypeSettings();

            var packageName = iOSConsts.DEFAULT_APP_PACKAGE_NAME;
            if (!string.IsNullOrEmpty(buildTypeSettings.PackageName))
            {
                packageName = buildTypeSettings.PackageName;
            }

            var isRelease = settingsiOS.Type == BuildType.Release;

            var defines = "IOS;MOBILE;" + (isRelease ? "RELEASE" : "DEBUG");

            props = new Dictionary<string, string>()
            {
                ["Configuration"] = isRelease ? "Release" : "Debug",
                ["Platform"] = "iPhone",
                ["BuildIpa"] = "true",
                ["CodesignKey"] = buildTypeSettings.CodesignKey ?? "Apple Development",
                ["CodesignProvision"] = buildTypeSettings.ProvisioningProfile ?? "Automatic",
                ["ApplicationId"] = packageName,
                ["ApplicationTitle"] = buildTypeSettings.ApplicationName,
                ["DefineConstants"] = defines,
                ["iOSBuild"] = "true",
                ["MOBILE"]="true",
                ["BUILD_MOBILE"] = "true",
                ["OutputPath"] = EditorPaths.iOSPublishFolderRoot
            };

            if (isRelease)
            {
                props["Optimize"] = "true";
                props["DebugType"] = "None";
                props["DebugSymbols"] = "false";
                props["MtouchUseLlvm"] = "true";
                // props["PublishTrimmed"] = "true";
                // props["TrimMode"] = "link";
            }
            else
            {
                props["Optimize"] = "false";
                props["DebugSymbols"] = "true";
                props["DebugType"] = "Embedded";
            }
        }

        protected override void OnBeforeBuild()
        {
            var buildTypeSettings = GetBuildSettings<iOSBuildSettings>(PlatformBuild.IOS).GetCurrentBuildTypeSettings();
            UpdateAppName(buildTypeSettings.ApplicationName);
            var bin = EditorPaths.iOSProjectRoot + "/bin";
            var obj = EditorPaths.iOSProjectRoot + "/obj";
            
            Directory.Delete(bin, true);
            Directory.Delete(obj, true);

            // In case the build fails due to a tooling error, this will make sure that the build directories exist,
            //  otherwise the editor will have to be reopened.
            Directory.CreateDirectory(bin);
            Directory.CreateDirectory(obj);
        }

        protected override void OnBuildSuccess()
        {
            var settings = GetBuildSettings<iOSBuildSettings>(PlatformBuild.IOS);
            var rootOutputFolder = GetOutputFolder(settings);
            Directory.CreateDirectory(rootOutputFolder);

            foreach (var file in Directory.EnumerateFiles(EditorPaths.iOSPublishFolderRoot))
            {
                if (_buildFilesExt.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                {
                    File.Copy(file, Path.Combine(rootOutputFolder, Path.GetFileName(file)), overwrite: true);
                }
            }
        }

        internal static string GetOutputFolder(iOSBuildSettings settings) // ugly
        {
            var rootOutputFolder = EditorPaths.iOSShipFolderRoot;
            var buildTypeSettings = settings.GetCurrentBuildTypeSettings();
            if (!string.IsNullOrEmpty(buildTypeSettings.OutputPath))
            {
                rootOutputFolder = Paths.ClearPathSeparation(buildTypeSettings.OutputPath);
            }

            return rootOutputFolder;
        }

        protected override string GetCSProjPath()
        {
            return Path.Combine(EditorPaths.iOSProjectRoot, EditorPaths.IOS_PROJECT_FULL_NAME);
        }

        protected override string[] GetTargetsToBuild()
        {
            return _targets;
        }

        private string GetVersion(ivec3 version)
        {
            return $"{version.x}.{version.y}.{version.z}.{0}";
        }

        private void UpdateAppName(string name)
        {
            // var stringsXmlPath = Path.Combine(EditorPaths.AndroidProjectRoot, "Resources", "values", "strings.xml");
           // File.WriteAllText(stringsXmlPath, BuildAndroidStringsXml(name));
        }

        private string BuildAndroidStringsXml(string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = iOSConsts.DEFAULT_APP_NAME;
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
}