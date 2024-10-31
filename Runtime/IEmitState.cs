using System;

namespace ObservableGadgets
{
    public interface IEmitState<TState>
    { 
        public delegate void StateChangeHandler(IEmitState<TState> emitter, TState oldState, TState newState);
        
        public void AddStateChangeHandler(StateChangeHandler handler);
        public void RemoveStateChangeHandler(StateChangeHandler handler);

        public TState GetState();
    }
}