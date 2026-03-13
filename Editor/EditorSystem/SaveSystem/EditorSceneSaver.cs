using Editor.Serialization;
using Editor.Utils;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class EditorSceneSaver : IFileSaver
    {
        public void Write(Guid refId, string relativePath)
        {
            var scene = SceneManager.Scenes.First(x => x.GetID() == refId);
            var sceneIR = SceneSerializer.SerializeScene(scene);
            var absPath = EditorPaths.GetAbsolutePathSafe(relativePath);

            File.WriteAllText(absPath, EditorJsonUtils.Serialize(sceneIR));
        }
    }
}
