using Editor.Serialization;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    internal static class EditorAssetFileCreator
    {
        internal static void CreateScene(string relativeDir, string assetName)
        {
            if(string.IsNullOrEmpty(relativeDir))
            {
                return;
            }
            var newScene = new Scene(Path.GetFileNameWithoutExtension(relativeDir), Guid.NewGuid());
            var sceneIR = SceneSerializer.SerializeScene(newScene);
            var json = EditorJsonUtils.Serialize(sceneIR);

            var directory = EditorPaths.GetAbsolutePathSafe(relativeDir);
            var absPath = Path.Combine(directory, assetName + ".scene");

            if (Directory.Exists(directory))
            {
                File.WriteAllText(absPath, json);
            }
            else
            {
                Debug.Error($"Directory doesn't exists: {directory}, can't create asset");
            }

            EditorAssetUtils.RefreshAssetDatabase();
        }
    }
}
