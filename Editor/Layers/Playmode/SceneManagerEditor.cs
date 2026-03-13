using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class SceneManagerEditor
    {
        private static readonly List<SerializedEditorScene> _sceneList = new();
        private static readonly List<List<Actor>> _hotReloadActorsSerialized = new();

        private static readonly List<SerializedEditorScene> _editModeScenes = new();

        internal static void SerializeScenesHotReload()
        {
            SerializeScenes(_sceneList, new SceneSerializer.SerializationOptions()
            {
                CollectedPhysicalActors = true,
                RemoveGameDLLComponentsFromActors = true,
            });

            _hotReloadActorsSerialized.Clear();
            foreach (var scene in _sceneList)
            {
                _hotReloadActorsSerialized.Add(scene.Actors);
            }
        }

        internal static void SerializeScenesPlaymode()
        {
            SerializeScenes(_editModeScenes, new SceneSerializer.SerializationOptions()
            {
                CollectedPhysicalActors = false,
                RemoveGameDLLComponentsFromActors = false,
            });
        }

        internal static void SerializeScenes(List<SerializedEditorScene> scenes, in SceneSerializer.SerializationOptions options)
        {
            scenes.Clear();
            for (int i = 1; i < SceneManager.Scenes.Count; i++)
            {
                scenes.Add(SceneSerializer.SerializeSceneEditor(SceneManager.Scenes[i], options));
            }
        }

        internal static void DeserializeScenesHotReload()
        {
            if (_sceneList.Count > 0)
            {
                for (int i = 0; i < _sceneList.Count; i++)
                {
                    var sceneSerialized = _sceneList[i];
                    SceneDeserializer.DeserializeSceneComponents(_hotReloadActorsSerialized[i], sceneSerialized.ActorsData);
                }
            }

            _sceneList.Clear();
            _hotReloadActorsSerialized.Clear();
        }

        internal static void DeserializePlaymodeScene()
        {
            SceneManager.Initialize();

            for (int i = 0; i < _editModeScenes.Count; i++)
            {
                var editorScene = _editModeScenes[i];
                if (SceneManager.Scenes.Count - 1 <= i) // -1 to remove the DontDestroyOnLoad Scene
                {
                    SceneManager.LoadSceneAdditive(editorScene.Name);
                }

                var scene = SceneManager.Scenes[i + 1];
                scene.Name = editorScene.Name;

                SceneDeserializer.DeserializeScene(editorScene.ActorsData, SceneManager.Scenes[i + 1]);
            }
        }
    }
}