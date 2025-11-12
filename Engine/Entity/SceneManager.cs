using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class SceneManager
    {
        public static Scene ActiveScene { get; internal set; } = new Scene();

        public static void LoadScene(string name)
        {
        }

        public static void Test_LoadScene(Scene scene)
        {
            ActiveScene.Destroy();
            // ActiveScene = scene;
        }
    }
}
