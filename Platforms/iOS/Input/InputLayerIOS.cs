using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using GLKit;
using UIKit;

namespace Engine.IOS
{
    internal class InputLayerIOS : InputLayerBase
    {
        public override TouchInput Touch { get; } = new();
        public override GamepadInput Gamepad { get; } = new();
        public override vec2 MousePosition { get; } = default;

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

        internal override void UpdateLayer()
        {
            base.UpdateLayer();

            Touch.TouchCount = 0;

            for (int i = 0; i < Touch.State.Length; i++)
            {
                ref var state = ref Touch.State[i];

                if (state.Type == TouchEvent.Down)
                {
                    if (state.IsDownEventConsumed)
                    {
                        state.Type = TouchEvent.Stationary;
                    }
                    else
                    {
                        state.IsDownEventConsumed = true;
                    }
                }

                if (state.Type == TouchEvent.Move)
                {
                    const long MoveStopDelayMs = 80;

                    long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    if (now - state.LastMoveTimeMs > MoveStopDelayMs)
                    {
                        state.Type = TouchEvent.Stationary;
                        state.Delta = vec2.Zero;
                    }
                }

                if (state.Type == TouchEvent.Up)
                {
                    if (state.IsUpEventConsumed)
                    {
                        state.Type = TouchEvent.None;
                        state.PointerId = -1;
                        state.IsUpEventConsumed = false;
                        state.IsDownEventConsumed = false;
                        state.Position = vec2.Zero;
                        state.Delta = vec2.Zero;
                        state.PrevPosition = vec2.Zero;
                    }
                    else
                    {
                        state.IsUpEventConsumed = true;
                    }
                }

                if (state.Type != TouchEvent.None)
                {
                    Touch.TouchCount++;
                }
            }
        }

        public void OnTouchesBegan(NSSet touches, GLKView view)
        {
            foreach (UITouch touch in touches)
            {
                var id = GetPointerId(touch);
                SetTouchState(id, touch, view, TouchEvent.Down);
            }
        }

        public void OnTouchesMoved(NSSet touches, GLKView view)
        {
            foreach (UITouch touch in touches)
            {
                var id = GetPointerId(touch);
                SetTouchState(id, touch, view, TouchEvent.Move);
            }
        }

        public void OnTouchesEnded(NSSet touches, GLKView view)
        {
            foreach (UITouch touch in touches)
            {
                var id = GetPointerId(touch);
                SetTouchState(id, touch, view, TouchEvent.Up);
                ReleasePointerId(touch);
            }
        }

        public void OnTouchesCancelled(NSSet touches, GLKView view)
        {
            OnTouchesEnded(touches, view);
        }

        private void SetTouchState(int id, UITouch touch, GLKView view, TouchEvent tEvent)
        {
            if (id < 0 || id >= Touch.State.Length)
            {
                Debug.Log("Invalid touch id: " + id);
                return;
            }

            ref var state = ref Touch.State[id];

            if (state.Type == TouchEvent.Down && !state.IsDownEventConsumed)
            {
                tEvent = TouchEvent.Down;
            }

            state.Type = tEvent;
            state.PointerId = id;
            var currentPos = touch.LocationInView(view);
            var previousPos = touch.PreviousLocationInView(view);

            var scale = (float)view.ContentScaleFactor;
            if (tEvent == TouchEvent.Move)
            {
                state.LastMoveTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            state.Position = new vec2((float)currentPos.X, (float)currentPos.Y) * scale;
            state.Delta = new vec2((float)(currentPos.X - previousPos.X), (float)(currentPos.Y - previousPos.Y)) * scale;
            state.PrevPosition = new vec2((float)previousPos.X, (float)previousPos.Y) * scale;
        }


        private readonly Dictionary<UITouch, int> _touchIds = new();
        private readonly bool[] _freeIds = new bool[TouchInput.MaxTouches];

        private int GetPointerId(UITouch touch)
        {
            if (_touchIds.TryGetValue(touch, out int id))
            {
                return id;
            }

            for (int i = 0; i < _freeIds.Length; i++)
            {
                if (!_freeIds[i])
                {
                    _freeIds[i] = true;
                    _touchIds[touch] = i;
                    return i;
                }
            }

            // Max touches used.
            return -1;
        }

        private void ReleasePointerId(UITouch touch)
        {
            if (_touchIds.TryGetValue(touch, out int id))
            {
                _touchIds.Remove(touch);
                _freeIds[id] = false;
            }
        }

        public override void Close()
        {
        }
    }
}