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

        public GLRenderer(GLView view)
        {
            _glView = view;
            AssetManager assets = Application.Context.Assets;

            MemoryStream memStream;
            using (var stream = assets.Open("GameData.gfs"))
            {
                memStream = new MemoryStream();
                stream.CopyTo(memStream);
                memStream.Position = 0;
            }

            _reader = new BinaryReader(memStream, Encoding.UTF8, leaveOpen: false);


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

            if(_engine == null)
            {
                _engine = new GFSEngine(_glView, typeof(GameApplication), _reader);
            }

        }
    }
}
