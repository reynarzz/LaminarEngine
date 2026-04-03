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

                const float threshold = 0.01f;

                if (state.Type != TouchEvent.Up && state.Type != TouchEvent.None)
                {
                    if (state.IsDownEventConsumed)
                    {
                        if (MathF.Abs(state.Delta.x) < threshold && MathF.Abs(state.Delta.y) < threshold)
                        {
                            state.Type = TouchEvent.Stationary;
                        }
                        else
                        {
                            state.Type = TouchEvent.Move;
                        }
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
                    Debug.Log($"id: {i}, evt: {state.Type} pos: {state.Position}, delta: {state.Delta}");
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
                SetTouchState(_touchIds[touch], touch, view, TouchEvent.Move);
            }
        }

        public void OnTouchesEnded(NSSet touches, GLKView view)
        {
            foreach (UITouch touch in touches)
            {
                SetTouchState(_touchIds[touch], touch, view, TouchEvent.Up);
                ReleasePointerId(touch);
            }
        }

        private void SetTouchState(int id, UITouch touch, GLKView view, TouchEvent tEvent)
        {
            ref var state = ref Touch.State[id];
            state.IsUpEventConsumed = false;
            state.IsDownEventConsumed = false;

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

        public void OnTouchesCancelled(NSSet touches, GLKView view)
        {
            foreach (UITouch touch in touches)
            {
                SetTouchState(_touchIds[touch], touch, view, TouchEvent.Up);
                ReleasePointerId(touch);
            }
        }

        private readonly Dictionary<UITouch, int> _touchIds = new();
        private readonly Queue<int> _freeIds = new();
        private int _nextId = 0;
        private const int MaxTouches = 10;

        private int GetPointerId(UITouch touch)
        {
            if (_touchIds.TryGetValue(touch, out int id))
            {
                return id;
            }

            if (_freeIds.Count > 0)
            {
                id = _freeIds.Dequeue();
            }
            else
            {
                if (_nextId >= MaxTouches)
                {
                    return -1;
                }

                id = _nextId++;
            }

            _touchIds[touch] = id;
            return id;
        }

        private void ReleasePointerId(UITouch touch)
        {
            if (_touchIds.TryGetValue(touch, out int id))
            {
                _touchIds.Remove(touch);
                _freeIds.Enqueue(id);
            }
        }
        
        public override void Close()
        {
        }
    }
}