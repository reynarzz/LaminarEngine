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
        public override bool OnGenericMotionEvent(MotionEvent? e)
        {
            if ((e.Source & InputSourceType.Joystick) != 0 &&
                 e.Action == MotionEventActions.Move)
            {
                float lx = e.GetAxisValue(Axis.X);
                float ly = e.GetAxisValue(Axis.Y);
                float rx = e.GetAxisValue(Axis.Rx);
                float ry = e.GetAxisValue(Axis.Ry);
            }

            return base.OnGenericMotionEvent(e);
        }
    }
}
