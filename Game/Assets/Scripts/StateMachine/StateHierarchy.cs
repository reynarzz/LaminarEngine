using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public abstract class StateBase<T> where T : IAIStateMachineContext
    {
        protected Dictionary<Type, StateBase<T>> SubStates { get; } = new();
        protected StateBase<T> Parent { get; set; }
        public T Context { get; set; }
        public StateMachine<T> FSM { get; set; }
        private StateBase<T> _currentSubState;

        protected StateBase() { }
        protected StateBase(StateBase<T>[] subsStates)
        {
            foreach (var subState in subsStates)
            {
                SubStates[subState.GetType()] = subState;
            }
        }
        public virtual void OnInit()
        {
            foreach (var (_, subState) in SubStates)
            {
                subState.FSM = FSM;
                subState.Context = Context;
                subState.Parent = this;
                subState.OnInit();
            }
        }

        public void Enter() 
        {
            if(_currentSubState != null)
            {
                _currentSubState.Enter();
            }
            else
            {
                OnEnter();
            }
        }

        public void Update()
        {
            if (_currentSubState != null)
            {
                _currentSubState.Update();
            }
            else
            {
                OnUpdate();
            }
        }

        public void LateUpdate()
        {
            if(_currentSubState != null)
            {
                _currentSubState.LateUpdate();
            }
            else
            {
                OnLateUpdate();
            }
        }

        public void FixedUpdate()
        {
            if (_currentSubState != null)
            {
                _currentSubState.FixedUpdate();
            }
            else
            {
                OnFixedUpdate();
            }
        }

        public void Exit()
        {
            if(_currentSubState != null)
            {
                _currentSubState.Exit();
            }
            else
            {
                OnExit();
            }

            _currentSubState = null;
        }

        public virtual void OnEnter() { }
        public virtual void OnLateUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnExit() { }
        public virtual void OnUpdate() { }

        protected virtual void ChangeSubState<TSub>() where TSub : StateBase<T>
        {
            if (!SubStates.TryGetValue(typeof(TSub), out var state))
            {
                throw new InvalidOperationException($"State {typeof(TSub).Name} not found in state machine.");
            }
            _currentSubState?.OnExit();
            _currentSubState = state;
            _currentSubState.FSM = FSM;
            _currentSubState.Context = Context;
            _currentSubState.Parent = this;
            _currentSubState.Enter();
        }
        protected void ReturnToParent()
        {
            _currentSubState?.OnExit();
            Parent?.SetActiveSubState(null);
        }
        internal void SetActiveSubState(StateBase<T> sub)
        {
            _currentSubState = sub;
        }
    }
}
