using Android.Content.Res;
using Android.Opengl;
using Game;
using Javax.Microedition.Khronos.Opengles;
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

        private BinaryReader LoadGameData()
        {
            string basePath = Application.Context.GetExternalFilesDir(null).AbsolutePath;
            string gameDataFilePath = Path.Combine(basePath, GameDataFile);
            Directory.CreateDirectory(basePath);
            if (!File.Exists(gameDataFilePath))
            {
                // First launch copy from assets
                using (var assetStream = Application.Context.Assets.Open(GameDataFile))
                using (var outStream = File.Create(gameDataFilePath))
                {
                    assetStream.CopyTo(outStream);
                }
                Console.WriteLine("Create GameDataPath: " + gameDataFilePath);
            }
            else
            {
                Console.WriteLine("Exist GameDataPath: " + gameDataFilePath);
            }

            FileStream stream = File.OpenRead(gameDataFilePath);
            return new BinaryReader(stream, Encoding.UTF8);
        }

        public void OnDrawFrame(IGL10? gl)
        {
            if (_engine != null)
            {
                _engine.Update();
            }
        }

        public void OnSurfaceChanged(IGL10? gl, int width, int height)
        {

        }

        public void OnSurfaceCreated(IGL10? gl, Javax.Microedition.Khronos.Egl.EGLConfig? config)
        {
            // Here manage context loss

            if (_engine == null)
            {
                _engine = new GFSEngine(_glView, new GameApplication(), _reader);
            }

        }
    }
}
