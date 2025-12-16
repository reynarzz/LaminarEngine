using Android.OS;
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
        public override GamepadInput Gamepad => _gamepad;
        public override vec2 MousePosition => _mousePosition;

        private readonly TouchInput _touchInput;
        private readonly GamepadInput _gamepad;
        private readonly Dictionary<int, int> _pointerToSlot;
        private const float _deathZone = 0.001f;
        private vec2 _mousePosition;

        public InputLayerAndroid()
        {
            _touchInput = new();
            _gamepad = new();
            _pointerToSlot = new();
        }

        public override void Initialize()
        {
        }

        internal override void UpdateLayer()
        {
            int activeCount = 0;

            for (int i = 0; i < _touchInput.State.Length; i++)
            {
                ref var state = ref _touchInput.State[i];

                switch (state.Type)
                {
                    case TouchEvent.Down:
                        if (!state.IsDownEventConsumed)
                        {
                            state.IsDownEventConsumed = true;
                        }
                        else
                        {
                            state.Type = TouchEvent.Stationary;
                        }
                        activeCount++;
                        break;
                    case TouchEvent.Stationary:
                        activeCount++;
                        break;
                    case TouchEvent.Move:

                        const long MoveStopDelayMs = 80;

                        long now = SystemClock.UptimeMillis();

                        if (now - state.LastMoveTimeMs > MoveStopDelayMs)
                        {
                            state.Type = TouchEvent.Stationary;
                            state.Delta = vec2.Zero;
                        }
                        activeCount++;
                        break;
                    case TouchEvent.Up:
                        if (!state.IsUpEventConsumed)
                        {
                            activeCount++;
                            state.IsUpEventConsumed = true;
                        }
                        else
                        {
                            state.Type = TouchEvent.None;
                            state.PointerId = -1;
                            state.IsUpEventConsumed = false;
                            state.IsDownEventConsumed = false;
                            state.Position = vec2.Zero;
                            state.Delta = vec2.Zero;
                            state.PrevPosition = vec2.Zero;
                        }

                        break;
                }
            }

            _touchInput.TouchCount = activeCount;
        }

        private int AllocateSlot(int pointerId)
        {
            for (int i = 0; i < _touchInput.State.Length; i++)
            {
                if (_touchInput.State[i].Type == TouchEvent.None ||
                    _touchInput.State[i].Type == TouchEvent.Up)
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
                        state.PrevPosition = pos;
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
                            state.Delta = state.Position - state.PrevPosition;
                            state.PrevPosition = state.Position;
                            state.LastMoveTimeMs = SystemClock.UptimeMillis();
                            if (state.Type != TouchEvent.Down && state.Delta.Magnitude > _deathZone)
                            {
                                state.Type = TouchEvent.Move;
                            }

                            _mousePosition = state.Position;
                        }

                        // TODO: Set mouse position for finger id.
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
                            state.Delta = state.Position - state.PrevPosition;
                            state.PrevPosition = state.Position;
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
                        }
                        _pointerToSlot.Clear();
                        break;
                    }
            }

            return true;
        }


        public bool OnGenericMotionEvent(MotionEvent e)
        {
            const InputSourceType GamepadSources = InputSourceType.Gamepad | InputSourceType.Joystick | InputSourceType.Dpad;

            if ((e.Source & GamepadSources) == 0)
                return false;

            if (e.Action != MotionEventActions.Move)
                return false;

            float lx = GetAxis(e, Axis.X);
            float ly = -GetAxis(e, Axis.Y);

            float rx = GetAxis(e, Axis.Z);
            float ry = -GetAxis(e, Axis.Rz);

            float lt = GetTrigger(e, Axis.Ltrigger);
            float rt = GetTrigger(e, Axis.Rtrigger);

            return true;
        }
        private float GetAxis(MotionEvent e, Axis axis)
        {
            var range = e.Device.GetMotionRange(axis, e.Source);
            if (range == null)
            {
                return 0f;
            }

            float v = e.GetAxisValue(axis);

            if (Math.Abs(v) < range.Flat)
            {
                return 0f;
            }

            return v;
        }

        private float GetTrigger(MotionEvent e, Axis axis)
        {
            var range = e.Device.GetMotionRange(axis, e.Source);
            if (range == null)
                return 0f;

            return Math.Max(0f, e.GetAxisValue(axis));
        }

        public bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent? e)
        {
            HandleGamePadButton(keyCode, e);
            return false;
        }

        private bool HandleGamePadButton([GeneratedEnum] Keycode keyCode, KeyEvent? e)
        {
            // TODO: Send these button events to class Gamepad.

            if ((e.Source & InputSourceType.Gamepad) == 0)
                return false;

            var gamepadId = e.DeviceId;

            switch (keyCode)
            {
                case Keycode.ButtonA:     // Xbox A / PS Cross
                case Keycode.ButtonB:     // Xbox B / PS Circle
                case Keycode.ButtonX:     // Xbox X / PS Square
                case Keycode.ButtonY:     // Xbox Y / PS Triangle

                case Keycode.ButtonL1:
                case Keycode.ButtonR1:
                case Keycode.ButtonL2:
                case Keycode.ButtonR2:

                case Keycode.ButtonStart:
                case Keycode.ButtonSelect:
                case Keycode.ButtonMode:
                case Keycode.ButtonThumbl:
                case Keycode.ButtonThumbr:


                    // Button down
                    return true;
            }

            return false;
        }

        public bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent? e)
        {
            if ((e.Source & InputSourceType.Gamepad) == 0)
                return false;

            // TODO: 
            return true;
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


        public override void Close()
        {

        }
    }
}
