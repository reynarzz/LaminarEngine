using Engine.Types;
using GlmNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Serialization;

namespace Engine
{
    [UniqueComponent]
    public class Animator : Component, ILateUpdatableComponent
    {
        [SerializedField]
        private Dictionary<string, AnimationState> _states = new();

        internal IDictionary<string, AnimationState> States => _states;

        private AnimationPlayer _animPlayer = new();
        private AnimationState _nextState;

        private float _transitionTime;
        private float _transitionDuration;
        public event Action<Animator> OnUpdate;

        [ShowFieldNoSerialize]
        public AnimationState CurrentState { get; private set; }
        [SerializedField] public AnimatorParameters Parameters { get; private set; } = new AnimatorParameters();

        internal float CurrentStateTime => _animPlayer.CurrentTime;

        public void AddState(AnimationState state)
        {
            _states[state.Name] = state;

            if (CurrentState == null)
            {
                SetState(state);
            }
        }

        public void SetState(string name)
        {
            if (_states.TryGetValue(name, out var state))
            {
                CurrentState = state;
                _animPlayer.Play(state.Clip);
            }
        }

        public AnimationState GetState(string name)
        {
            _states.TryGetValue(name, out var state);
            return state;
        }

        public void SetState(AnimationState state)
        {
            if (!_states.ContainsKey(state.Name))
            {
                AddState(state);
            }

            CurrentState = state;
            _animPlayer.Play(state.Clip);
        }

        void ILateUpdatableComponent.OnLateUpdate()
        {
            UpdateFrames();
        }

        private void UpdateFrames()
        {
            if (CurrentState == null)
            {
                return;
            }

            var transition = CurrentState.CheckTransitions(Parameters);
            if (transition != null && _nextState == null)
            {
                if (_states.TryGetValue(transition.ToState, out _nextState))
                {
                    _transitionTime = 0f;
                    _transitionDuration = transition.BlendTime;
                }
            }

            if (_nextState != null)
            {
                _transitionTime += Time.DeltaTime;

                if (_transitionTime >= _transitionDuration)
                {
                    CurrentState = _nextState;
                    _nextState = null;
                    _transitionDuration = 0;
                    _transitionTime = 0;
                    _animPlayer.Play(CurrentState.Clip);
                }
            }

            _animPlayer.Update();

            OnUpdate?.Invoke(this);
        }

        public void Play(string state)
        {
            _states.TryGetValue(state, out _nextState);
            _transitionDuration = 0;
            _transitionTime = 0;
            UpdateFrames();
        }

        public float GetFloat(string property)
        {
            return _animPlayer.GetFloat(property);
        }

        public vec2 GetVec2(string property)
        {
            return _animPlayer.GetVec2(property);
        }

        public vec3 GetVec3(string property)
        {
            return _animPlayer.GetVec3(property);
        }

        public quat GetQuat(string property)
        {
            return _animPlayer.GetQuat(property);
        }

        public Color GetColor(string property)
        {
            return _animPlayer.GetColor(property);
        }

        public Sprite GetSprite(string property)
        {
            return _animPlayer.GetSprite(property);
        }

        //public void GetProperty(string property, ref CurveEvaluatedResult result)
        //{
        //    _animPlayer.GetProperty(property, ref result);
        //}

        /// <summary>
        /// Removes all states, and restarts the animator
        /// </summary>
        public void Clear()
        {
            _states.Clear();
            CurrentState = null;
            _nextState = null;
            _transitionTime = 0;
            _transitionDuration = 0;
            _animPlayer.Clear();
        }
    }
}
