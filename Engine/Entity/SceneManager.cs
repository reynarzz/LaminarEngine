using Engine.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class SceneManager
    {
        public static Scene ActiveScene { get; private set; } = new Scene();
        private static readonly List<Scene> _scenesToDestroy = new List<Scene>();
        public static void LoadScene(string name)
        {
        }

        public static void Test_LoadScene(Scene scene)
        {
            RenderingLayer.Test_ClearBatches();

            _scenesToDestroy.Add(ActiveScene);
            ActiveScene = scene;
        }

        internal static void OnCleanUpUpdate()
        {
            ActiveScene.DeletePending();

            foreach (var scene in _scenesToDestroy)
            {
                scene.Destroy();
            }
            if(_scenesToDestroy.Count > 0)
            {
                _scenesToDestroy.Clear();
            }
        }
    }
}
