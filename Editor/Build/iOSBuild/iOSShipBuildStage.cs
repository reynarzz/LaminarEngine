using GlmNet;
using Engine;
using System;
using System.Text;

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

    internal enum DeviceOrientation
    {
        Any,
        PortraitAny,
        PortraitUp,
        PortraitDown,
        LandscapeAny,
        LandscapeLeft,
        LandscapeRight
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
                ["MOBILE"] = "true",
                ["BUILD_MOBILE"] = "true",
                ["OutputPath"] = EditorPaths.iOSPublishFolderRoot
            };

            if (isRelease)
            {
                props["Optimize"] = "true";
                props["DebugType"] = "None";
                props["DebugSymbols"] = "false";
                props["MtouchUseLlvm"] = "true";
                props["MtouchDebug"] = "false";
                props["GeneratedSymbols"] = "true";
                // props["PublishTrimmed"] = "true";
                // props["TrimMode"] = "link";
            }
            else
            {
                props["Optimize"] = "false";
                props["DebugSymbols"] = "true";
                props["MtouchDebug"] = "true";
                props["DebugType"] = "Embedded";
            }
        }

        protected override void OnBeforeBuild()
        {
            var buildTypeSettings = GetBuildSettings<iOSBuildSettings>(PlatformBuild.IOS);

            UpdateAppConfig(buildTypeSettings);
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
            return $"{version.x}.{version.y}.{version.z}";
        }

        private readonly static string[] _deviceOrientationNames =
        [
            "UIInterfaceOrientationPortrait",
            "UIInterfaceOrientationPortraitUpsideDown",
            "UIInterfaceOrientationLandscapeLeft",
            "UIInterfaceOrientationLandscapeRight"
        ];

        private readonly static Dictionary<DeviceOrientation, int[]> _orientationMapper = new()
        {
            [DeviceOrientation.Any] = [0, 1, 2, 3],
            [DeviceOrientation.PortraitAny] = [0, 1],
            [DeviceOrientation.PortraitUp] = [0],
            [DeviceOrientation.PortraitDown] = [1],
            [DeviceOrientation.LandscapeAny] = [2, 3],
            [DeviceOrientation.LandscapeLeft] = [2],
            [DeviceOrientation.LandscapeRight] = [3],
        };

        private const string TEMPLATE_DEVICE_ORIENTATION_ID = "$__LAM_DEVICE_ORIENTATION__";
        private const string TEMPLATE_APP_NAME_ID = "$__LAM_APP_NAME__";
        private const string TEMPLATE_BUNDLE_ID_ID = "$__LAM_BUNDLE_ID__";
        private const string TEMPLATE_SHORT_VERSION_ID = "$__LAM_SHORT_VERSION__";
        private const string TEMPLATE_BUNDLE_VERSION_ID = "$__LAM_BUNDLE_VERSION__";
        private const string TEMPLATE_BUNDLE_NAME_ID = "$__LAM_BUNDLE_NAME__";
        
        private void UpdateAppConfig(iOSBuildSettings settings)
        {
            var typeSettings = settings.GetCurrentBuildTypeSettings();

            var template = File.ReadAllText(EditorPaths.InfoPlistFileTemplateFullPath);

            template = template.Replace(TEMPLATE_APP_NAME_ID, typeSettings.ApplicationName);
            template = template.Replace(TEMPLATE_BUNDLE_NAME_ID, typeSettings.ApplicationName);
            template = template.Replace(TEMPLATE_BUNDLE_ID_ID, typeSettings.PackageName);
            
            if (!_orientationMapper.TryGetValue(settings._orientation, out var mappedIdxs))
            {
                mappedIdxs = _orientationMapper[DeviceOrientation.Any];
            }

            var orientationTags = new StringBuilder();

            foreach (var idx in mappedIdxs)
            {
                orientationTags.AppendLine($"<string>{_deviceOrientationNames[idx]}</string>");
            }

            template = template.Replace(TEMPLATE_DEVICE_ORIENTATION_ID, orientationTags.ToString());

            var shortVersion = GetVersion(typeSettings.ShortVersion);
            template = template.Replace(TEMPLATE_SHORT_VERSION_ID, shortVersion);
            
            template = template.Replace(TEMPLATE_BUNDLE_VERSION_ID, typeSettings.BundleVersion.ToString());
            
            File.WriteAllText(EditorPaths.InfoPlistFileBuildFullPath, template);
        }
    }
}