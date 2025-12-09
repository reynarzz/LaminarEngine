using Android.Opengl;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Android
{
    public partial class GLView : GLSurfaceView
    {
        public override bool OnTouchEvent(MotionEvent? e)
        {
            float x = e.GetX();
            float y = e.GetY();

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    // finger down
                    break;
                case MotionEventActions.Move:
                    // finger move
                    break;
                case MotionEventActions.Up:
                    // finger up
                    break;
            }

            return true;
        }
    }
}
