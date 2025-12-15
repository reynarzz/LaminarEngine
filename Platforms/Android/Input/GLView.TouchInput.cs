using Android.Opengl;
using Android.Views;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Android
{
 
    public partial class GLView : GLSurfaceView
    {
        private void SetTouchState(int id, vec2 position, TouchEvent type)
        {
            TouchInput._state[id].Position = position;
            TouchInput._state[id].Type = type;
            TouchInput._state[id].PointerId = id;
        }

        private void ClearTouches()
        {
            for (int i = 0; i < TouchInput._state.Length; i++)
            {
                TouchInput._state[i].Type = TouchEvent.None;
                TouchInput._state[i].PointerId = -1;
                TouchInput._state[i].Position = vec2.Zero;
            }
        }

        public override bool OnTouchEvent(MotionEvent? e)
        {
            var action = e.ActionMasked;
            int index = e.ActionIndex; 

            switch (action)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    {
                        int id = e.GetPointerId(index);
                        if (TouchInput._state[id].Type == TouchEvent.None ||
                            TouchInput._state[id].Type == TouchEvent.Up)
                        {
                            TouchInput.TouchCount++;
                        }
                        var position = new vec2(e.GetX(index), e.GetY(index));
                        SetTouchState(e.GetPointerId(index), position, TouchEvent.Down);

                        Debug.Log($"Pointer id: '{id}' down");
                        // --Input.MousePosition = position;

                        break;
                    }

                case MotionEventActions.Move:
                    {
                        for (int i = 0; i < e.PointerCount; i++)
                        {
                            SetTouchState(e.GetPointerId(i), new vec2(e.GetX(i), e.GetY(i)), TouchEvent.Move);
                            Debug.Log($"Pointer id: '{e.GetPointerId(i)}' move");

                        }
                        break;
                    }

                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    {
                        TouchInput.TouchCount = Math.Max(0, TouchInput.TouchCount - 1);

                        SetTouchState(e.GetPointerId(index), new vec2(e.GetX(index), e.GetY(index)), TouchEvent.Up);
                        Debug.Log($"Pointer id: '{e.GetPointerId(index)}' up");

                        break;
                    }
                case MotionEventActions.Cancel:
                    {
                        TouchInput.TouchCount = 0;
                        ClearTouches();
                    }
                    break;
            }

            return true;
        }

    }
}
