using Engine;
using Microsoft.Build.Locator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    public enum PlatformBuild
    {
        GameAppDomain,
        Windows,
        MacOs,
        Android,
        IOS,
        Linux
    }
    public struct BuildResult
    {
        public bool IsSucess;
        public bool AnyStageSkippedBuild;
        public PlatformBuild Platform;
    }

    internal class BuildSystem
    {
        internal static bool IsAnyBuilding { get; private set; } = false;
        // internal static bool IsError { get; private set; } = false;

        private static readonly Dictionary<PlatformBuild, PlatformBuilder> _platformBuilders;
        private readonly static SynchronizationContext _mainContext;
        public static event Action<BuildResult> OnBuildCompleted;
        private static PlatformBuild? _currentPlatformBuilding;

        static BuildSystem()
        {
            _mainContext = SynchronizationContext.Current;
            MSBuildLocator.RegisterDefaults();

            _platformBuilders = new Dictionary<PlatformBuild, PlatformBuilder>()
            {
                { PlatformBuild.GameAppDomain, new EditorBuilder() },
                { PlatformBuild.Windows, new WindowsBuilder() },
                { PlatformBuild.Android, new AndroidBuilder() },
            };
        }

        internal static Task BuildAsync(PlatformBuild platform)
        {
            return BuildAsync(platform, null);
        }
        internal static Task BuildAsync(PlatformBuild platform, Action<BuildResult> resultCallback)
        {
            if (IsAnyBuilding)
            {
                return Task.CompletedTask;
            }
            
            _currentPlatformBuilding = platform;

            return Task.Run(async () =>
            {
                try
                {
                    if (_platformBuilders.TryGetValue(platform, out var builder))
                    {
                        IsAnyBuilding = true;
                        var platformBuildResult = await builder.Build();
                        platformBuildResult.Platform = platform;
                        RaiseBuildCompleted(platformBuildResult, resultCallback);
                    }
                }
                catch(Exception e)
                {
                    Debug.Error(e);
                }
                finally
                {
                    IsAnyBuilding = false;
                    _currentPlatformBuilding = null;
                }
            });
        }

        internal static bool IsBuilding(PlatformBuild platform)
        {
            return platform == _currentPlatformBuilding;
        }

        private static void RaiseBuildCompleted(BuildResult result, Action<BuildResult> resultCallback)
        {
            var ctx = _mainContext;

            if (ctx != null)
            {
                ctx.Post(_ => 
                { 
                    OnBuildCompleted?.Invoke(result);
                }, null);

                ctx.Post(_ =>
                {
                    resultCallback?.Invoke(result);
                }, null);

            }
            else
            {
                OnBuildCompleted?.Invoke(result);
                resultCallback?.Invoke(result);
            }
        }
    }
}