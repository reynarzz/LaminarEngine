using Engine;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class EditorBuilder : PlatformBuilder
    {
        internal EditorBuilder() : base([new EditorProjectBuildStage(),
                                          new EditorAssetsBuildStage()])
        {
        }

        protected override bool IsBuildNeeded()
        {
            var currentGameDllFolder = EditorPaths.HookFolderAbsolutePath;

            if (!Directory.Exists(currentGameDllFolder))
            {
                Directory.CreateDirectory(currentGameDllFolder);
                return true;
            }

            if (!File.Exists(EditorPaths.GameHookDLLAbsolutePath))
            {
                return true;
            }

            // Check all the files to verify of any changed.
            var outputTime = File.GetLastWriteTimeUtc(EditorPaths.GameHookDLLAbsolutePath);

            if (!File.Exists(EditorPaths.CompiledGameDllAbsolutePath))
            {
                Debug.Log($"Original '{EditorPaths.GAME_PROJECT_NAME}.dll' is non existent.");
                return true;
            }

            var files = Directory.EnumerateFiles(Paths.GetAssetsFolderPath(), "*.cs", SearchOption.AllDirectories);
            return files.Any(f => File.GetLastWriteTimeUtc(f) > outputTime);
        }
    }
}
