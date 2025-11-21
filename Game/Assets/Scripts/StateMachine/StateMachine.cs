using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public interface IAIStateMachineContext { }

    public class StateMachine<T> where T: IAIStateMachineContext
    {
        private readonly Dictionary<Type, StateBase<T>> _states = new();
        private StateBase<T> _currentState;
        public StateMachine(T context, StateBase<T>[] states)
        {
            foreach (var state in states)
            {
                _states[state.GetType()] = state;
                state.FSM = this;
                state.Context = context;
                state.OnInit();
            }
        }

        public void ChangeState<TState>() where TState : StateBase<T>
        {
            if (!_states.TryGetValue(typeof(TState), out var state))
            {
                Debug.Error($"State {typeof(TState).Name} not found in state machine.");
            }

            _currentState?.OnExit();
            _currentState = state;
            _currentState?.Enter();
        }

        public void SetInitialState<TState>() where TState : StateBase<T>
        {
            ChangeState<TState>();
        }

        public void OnUpdate()
        {
            _currentState?.Update();
        }

        public void OnLateUpdate()
        {
            _currentState?.LateUpdate();
        }

        public void OnFixedUpdate()
        {
            _currentState?.FixedUpdate();
        }
    }
}
