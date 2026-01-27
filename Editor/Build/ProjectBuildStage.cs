using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
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

        public sealed override async Task<BuildStageResult> Execute()
        {
            var projectCollection = new ProjectCollection(GetBuildProperties());
            var project = projectCollection.LoadProject(GetCSProjPath());

            // Logger
            _parameters = new BuildParameters(projectCollection)
            {
                Loggers = [new BuildLogger()]
            };

            _instance = project.CreateProjectInstance();

            var assetsPath = Path.Combine(EditorPaths.GameRoot, "Library/Build/obj/project.assets.json");

            if (!File.Exists(assetsPath))
            {
                // Restore packages, such as nugget.
                BuildManager.DefaultBuildManager.Build(_parameters, new BuildRequestData(_instance, ["Restore"]));
            }

            var result = BuildManager.DefaultBuildManager.Build(_parameters, new BuildRequestData(_instance, GetTargetsToBuild()));

            return new BuildStageResult()
            {
                IsSuccess = result.OverallResult == BuildResultCode.Success,

            };
        }

        protected abstract Dictionary<string, string> GetBuildProperties();
        protected abstract string GetCSProjPath();
        protected abstract string[] GetTargetsToBuild();
    }
}
