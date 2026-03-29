using Editor.Data;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal abstract class ProjectBuildStage : BuildStage
    {
        private BuildParameters _parameters;
        private ProjectInstance _instance;
        private readonly ILogger _logger;

        protected class DefaultPropertyConsts
        {
            internal const string DEFINE_CONSTANTS = "DefineConstants";
        }

        protected ProjectBuildStage(ILogger logger)
        {
            _logger = logger;
        }

        public sealed override async Task<BuildStageResult> Execute()
        {
            OnBeforeBuild();

            var projectCollection = new ProjectCollection(GetBuildProperties());
            var project = projectCollection.LoadProject(GetCSProjPath());

            // Logger
            _parameters = new BuildParameters(projectCollection)
            {
                Loggers = [_logger]
            };

            _instance = project.CreateProjectInstance();

            // Restore packages, such as nugget.
            BuildManager.DefaultBuildManager.Build(_parameters, new BuildRequestData(_instance, ["Restore"]));

            // Project build.
            var result = BuildManager.DefaultBuildManager.Build(_parameters, new BuildRequestData(_instance, GetTargetsToBuild(), null,
                                                                BuildRequestDataFlags.ReplaceExistingProjectInstance));

            var buildResult = new BuildStageResult()
            {
                IsSuccess = result.OverallResult == BuildResultCode.Success,
            };

            if (buildResult.IsSuccess)
            {
                OnBuildSuccess();
            }
            else
            {
                OnBuildFailed();
            }

            return buildResult;
        }
        protected virtual void OnBeforeBuild() { }
        protected virtual void OnBuildSuccess() { }
        protected virtual void OnBuildFailed() { }

        protected abstract Dictionary<string, string> GetBuildProperties();
        protected abstract string GetCSProjPath();
        protected abstract string[] GetTargetsToBuild();

        protected T GetBuildSettings<T>(PlatformBuild platform) where T : PlatformBuildSettings
        {
            return GetBuildSettings(platform) as T;
        }

        protected PlatformBuildSettings GetBuildSettings(PlatformBuild platform)
        {
            return EditorProjectDataManager.BuildSettings.GetBuildSettings(platform);
        }
    }
}
