using System;

namespace Editor.Build
{

    internal abstract class PlatformBuilder
    {
        private readonly Dictionary<Type, BuildStage> _buildStages = new();

        protected PlatformBuilder(BuildStage[] buildStages)
        {
            foreach (var stage in buildStages)
            {
                _buildStages.Add(stage.GetType(), stage);
            }
        }

        protected void AddStage(BuildStage stage)
        {
            if (stage != null && !_buildStages.ContainsKey(stage.GetType()))
            {
                _buildStages.Add(stage.GetType(), stage);
            }
        }

        protected void RemoveStage(BuildStage stage)
        {
            _buildStages.Remove(stage.GetType());
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

            foreach (var (type, stage) in _buildStages)
            {
                if (stage.ShouldExecute())
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