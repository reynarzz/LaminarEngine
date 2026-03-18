using Android.Runtime;
using Android.Views;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Android
{
    public class AndroidSystem
    {
        private LaminarEngine _engine;
        private readonly BinaryReader _reader;
        private readonly GLView _view;
        private readonly InputLayerAndroid _input;

        public AndroidSystem(GLView view)
        {
            _view = view;
            _input = new InputLayerAndroid();
            _reader = LoadGameData();
        }
        
        // TODO: move this to another class.
        private BinaryReader LoadGameData()
        {
            string basePath = global::Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
            string gameDataFilePath = Path.Combine(basePath, Paths.ASSET_BUILD_DATA_FULL_FILE_NAME);

            // TODO: check gameFile version.

            Directory.CreateDirectory(basePath);
            // --if (!File.Exists(gameDataFilePath))
            {
                // First launch copy from assets
                using (var assetStream = global::Android.App.Application.Context.Assets.Open(Paths.ASSET_BUILD_DATA_FULL_FILE_NAME))
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


        public bool OnTouchEvent(MotionEvent? e)
        { 
            return _input.OnTouchEvent(e);
        }

        public bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent? e)
        {
            return _input.OnKeyDown(keyCode, e);
        }

        public bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent? e)
        {
            return _input.OnKeyUp(keyCode, e);
        }

        public bool OnGenericMotionEvent(MotionEvent? e)
        {
            return _input.OnGenericMotionEvent(e);
        }

        public void OnDrawFrame()
        {
            // TODO: split rendering, and rest of the loop in two different threads.
            if (_engine != null)
            {
                _engine.Update();
            }
        }

        public void OnSurfaceChanged(int width, int height)
        {

        }

        public void OnSurfaceCreated(Javax.Microedition.Khronos.Egl.EGLConfig? config)
        {
            if (_engine == null)
            {
                _engine = new LaminarEngine(_view, new GameApplication(), _input, _reader);
            }
            else
            {
                // Here manage context loss
            }
        }
    }
}
