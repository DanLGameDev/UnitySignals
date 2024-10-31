using System;
using System.Collections.Generic;

namespace ObservableGadgets
{
    public abstract class Gadget<TState> : IReceiveState<TState>, IEmitState<TState> where TState : IEquatable<TState>
    {
        private TState _state;
        private readonly List<IEmitState<TState>.StateChangeHandler> _stateChangeHandlers = new List<IEmitState<TState>.StateChangeHandler>();

        protected Gadget(TState initialState)
        {
            _state = initialState;
        }

        // IEmitState
        public void AddStateChangeHandler(IEmitState<TState>.StateChangeHandler handler) => _stateChangeHandlers.Add(handler);
        public void RemoveStateChangeHandler(IEmitState<TState>.StateChangeHandler handler) => _stateChangeHandlers.Remove(handler);

        public TState GetState() => _state;

        // IReceiveState
        public void SetState(TState newState)
        {
            SetStateInternal(newState);
        }

        protected virtual void SetStateInternal(TState newState)
        {
            if (_state.Equals(newState))
                return;

            TState oldState = _state;
            _state = newState;

            foreach (var handler in _stateChangeHandlers)
                handler(this, oldState, newState);
        }
    }
}