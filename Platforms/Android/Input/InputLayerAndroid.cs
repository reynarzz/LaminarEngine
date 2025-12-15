using Android.Runtime;
using Android.Views;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Android
{
    internal class InputLayerAndroid : InputLayerBase
    {
        public override TouchInput Touch => _touchInput;
        public override vec2 MousePosition => _mousePosition;

        private readonly TouchInput _touchInput;
        private vec2 _mousePosition;
        private readonly Dictionary<int, int> _pointerToSlot = new();

        public InputLayerAndroid()
        {
            _touchInput = new TouchInput();
        }

        public override void Initialize()
        {

        }

        internal override void UpdateLayer()
        {
            base.UpdateLayer();

            int activeCount = 0;

            for (int i = 0; i < _touchInput.State.Length; i++)
            {
                ref var t = ref _touchInput.State[i];

                switch (t.Type)
                {
                    case TouchEvent.Down:
                        t.Type = TouchEvent.Move;
                        activeCount++;
                        break;

                    case TouchEvent.Move:
                        activeCount++;
                        break;

                    case TouchEvent.Up:
                        t.Type = TouchEvent.None;
                        t.PointerId = -1;
                        t.Position = vec2.Zero;
                        break;
                }
            }

            _touchInput.TouchCount = activeCount;
        }

        public override bool GetKey(KeyCode key)
        {
            return false;
        }

        public override bool GetKeyDown(KeyCode key)
        {
            return false;
        }

        public override bool GetKeyUp(KeyCode key)
        {
            return false;
        }

        public override bool GetMouse(MouseButton button)
        {
            return false;
        }

        public override bool GetMouseDown(MouseButton button)
        {
            return false;
        }

        public override bool GetMouseUp(MouseButton button)
        {
            return false;
        }

        private int AllocateSlot(int pointerId)
        {
            for (int i = 0; i < _touchInput.State.Length; i++)
            {
                if (_touchInput.State[i].Type == TouchEvent.None ||
                    _touchInput.State[i].Type == TouchEvent.Up ||
                    _touchInput.State[i].Type == TouchEvent.Cancel)
                {
                    _pointerToSlot[pointerId] = i;
                    _touchInput.State[i].PointerId = pointerId;
                    return i;
                }
            }

            return -1;
        }

        private int GetSlot(int pointerId)
        {
            return _pointerToSlot.TryGetValue(pointerId, out var slot) ? slot : -1;
        }

        public void OnGenericMotionEvent(MotionEvent? e)
        {
            if ((e.Source & InputSourceType.Joystick) != 0 &&
                 e.Action == MotionEventActions.Move)
            {
                float lx = e.GetAxisValue(Axis.X);
                float ly = e.GetAxisValue(Axis.Y);
                float rx = e.GetAxisValue(Axis.Rx);
                float ry = e.GetAxisValue(Axis.Ry);
            }
        }

        public bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent? e)
        {
            return true;
        }

        public bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent? e)
        {
            return true;
        }

        public bool OnTouchEvent(MotionEvent? e)
        {
            if (e == null)
            {
                return false;
            }

            var action = e.ActionMasked;
            int index = e.ActionIndex;

            switch (action)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    {
                        int pointerId = e.GetPointerId(index);
                        int slot = AllocateSlot(pointerId);
                        if (slot < 0)
                        {
                            break;
                        }

                        var pos = new vec2(e.GetX(index), e.GetY(index));
                        ref var state = ref _touchInput.State[slot];

                        state.Position = pos;
                        state.Type = TouchEvent.Down;

                        _mousePosition = pos;
                        break;
                    }

                case MotionEventActions.Move:
                    {
                        for (int i = 0; i < e.PointerCount; i++)
                        {
                            int pointerId = e.GetPointerId(i);
                            int slot = GetSlot(pointerId);
                            if (slot < 0)
                            {
                                continue;
                            }

                            ref var state = ref _touchInput.State[slot];
                            state.Position = new vec2(e.GetX(i), e.GetY(i));

                            if (state.Type != TouchEvent.Down)
                            {
                                state.Type = TouchEvent.Move;
                            }
                        }
                        break;
                    }

                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    {
                        int pointerId = e.GetPointerId(index);
                        int slot = GetSlot(pointerId);
                        if (slot >= 0)
                        {
                            ref var state = ref _touchInput.State[slot];
                            state.Position = new vec2(e.GetX(index), e.GetY(index));
                            state.Type = TouchEvent.Up;
                            _pointerToSlot.Remove(pointerId);
                        }
                        break;
                    }

                case MotionEventActions.Cancel:
                    {
                        foreach (var kv in _pointerToSlot)
                        {
                            int slot = kv.Value;
                            _touchInput.State[slot].Type = TouchEvent.Up;
                            _touchInput.State[slot].PointerId = -1;
                        }
                        _pointerToSlot.Clear();
                        break;
                    }
            }

            return true;
        }

        public override void Close()
        {

        }
    }
}
