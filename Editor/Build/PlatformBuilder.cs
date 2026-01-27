using System;

namespace Editor.Build
{

    internal abstract class PlatformBuilder
    {
        private readonly BuildStage[] _buildStages;

        protected PlatformBuilder(BuildStage[] buildStages)
        {
            _buildStages = buildStages;
        }

        public async Task<BuildResult> Build()
        {
            if (!IsBuildNeeded())
            {
                return new BuildResult()
                {
                    IsSucess = true,
                    AnyStageSkippedBuild = true
                };
            }

            OnBeforeBuild();
            BuildResult buildResult = default;

            foreach (var stage in _buildStages)
            {
                if (stage.ShouldBuild())
                {
                    var result = await stage.Execute();

                    if (!result.IsSuccess)
                    {
                        buildResult = new BuildResult()
                        {
                            AnyStageSkippedBuild = false,
                            IsSucess = false
                        };

                        OnAfterBuild(buildResult);

                        return buildResult;
                    }
                }
                else
                {
                    buildResult.AnyStageSkippedBuild = true;
                }
            }

            buildResult.IsSucess = true;

            OnAfterBuild(buildResult);
            return buildResult;
        }

        protected virtual void OnBeforeBuild() { }
        protected virtual void OnAfterBuild(BuildResult result) { }
        protected virtual bool IsBuildNeeded() { return true; }
    }
}