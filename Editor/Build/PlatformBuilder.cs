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
                    DidBuild = false
                };
            }

            foreach (var stage in _buildStages)
            {
                var result = await stage.Execute();

                if (!result.IsSuccess)
                {
                    return new BuildResult()
                    {
                        DidBuild = false,
                        IsSucess = false
                    };
                }
            }

            return new()
            {
                IsSucess = true,
                DidBuild = true
            };
        }

        protected virtual void OnBeforeBuild() { }
        protected virtual void OnAfterBuild() { }

        protected abstract bool IsBuildNeeded();
    }
}