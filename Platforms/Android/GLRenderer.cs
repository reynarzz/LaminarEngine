using Android.Content.Res;
using Android.Opengl;
using Game;
using Javax.Microedition.Khronos.Opengles;
using SharedTypes;
using System.Text;

namespace Engine.Android
{
    public class GLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
    {
        private GFSEngine _engine;
        private BinaryReader _reader;
        private readonly GLView _glView;
        private const string GameDataFile = "GameData.gfs";
        public GLRenderer(GLView view)
        {
            _glView = view;

            _reader = LoadGameData();// new BinaryReader(, Encoding.UTF8, leaveOpen: false);
        }

        // TODO: move this to another class.
        private BinaryReader LoadGameData()
        {
            string basePath = Application.Context.GetExternalFilesDir(null).AbsolutePath;
            string gameDataFilePath = Path.Combine(basePath, GameDataFile);
            
            // TODO: check gameFile version.

            Directory.CreateDirectory(basePath);
            // --if (!File.Exists(gameDataFilePath))
            {
                // First launch copy from assets
                using (var assetStream = Application.Context.Assets.Open(GameDataFile))
                using (var outStream = File.Create(gameDataFilePath))
                {
                    assetStream.CopyTo(outStream);
                }
                Console.WriteLine("Create GameDataPath: " + gameDataFilePath);
            }
            //else
            //{


            //    Console.WriteLine("Exist GameDataPath: " + gameDataFilePath);
            //}

            FileStream stream = File.OpenRead(gameDataFilePath);
            return new BinaryReader(stream, Encoding.UTF8);
        }

        public void OnDrawFrame(IGL10? gl)
        {
            // TODO: split rendering, and rest of the loop in two different threads.
            if (_engine != null)
            {
                _engine.Update();
            }
        }

        public void OnSurfaceChanged(IGL10? gl, int width, int height)
        {
            _glView.UpdateView(width, height);
        }

        public void OnSurfaceCreated(IGL10? gl, Javax.Microedition.Khronos.Egl.EGLConfig? config)
        {
            if (_engine == null)
            {
                _engine = new GFSEngine(_glView, new GameApplication(), _reader);
            }
            else
            {
                // Here manage context loss
            }
        }
    }
}
