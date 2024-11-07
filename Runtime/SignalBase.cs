using System;
using System.Collections.Generic;

namespace DGP.UnitySignals
{
    public abstract class SignalBase<TValueType> : IEmitSignals<TValueType> where TValueType : IEquatable<TValueType>
    {
        public event IEmitSignals<TValueType>.SignalChangedHandler SignalChanged;
        
        private readonly HashSet<ISignalObserver<TValueType>> _objectObservers = new();
        private readonly HashSet<IEmitSignals<TValueType>.SignalChangedHandler> _delegateObservers = new();
        private readonly HashSet<Action<TValueType>> _actionObservers = new();
        
        public static implicit operator TValueType(SignalBase<TValueType> signal) => signal.GetValue();
        
        public void AddObserver(ISignalObserver<TValueType> observer) => _objectObservers.Add(observer);
        public void RemoveObserver(ISignalObserver<TValueType> observer) => _objectObservers.Remove(observer);

        public void AddObserver(IEmitSignals<TValueType>.SignalChangedHandler observer) => _delegateObservers.Add(observer);
        public void RemoveObserver(IEmitSignals<TValueType>.SignalChangedHandler observer) => _delegateObservers.Remove(observer);
        
        public void AddObserver(Action<TValueType> observer) => _actionObservers.Add(observer);
        public void RemoveObserver(Action<TValueType> observer) => _actionObservers.Remove(observer);

        public void ClearObservers()
        {
            _objectObservers.Clear();
            _delegateObservers.Clear();
            _actionObservers.Clear();
        }

        public abstract TValueType GetValue();
        protected void NotifyObservers(TValueType oldValue, TValueType newValue)
        {
            SignalChanged?.Invoke(this, oldValue, newValue);
            
            foreach (var observer in _objectObservers)
                observer.SignalValueChanged(this, newValue, oldValue);
            
            foreach (var observer in _delegateObservers)
                observer(this, oldValue, newValue);
            
            foreach (var observer in _actionObservers)
                observer(newValue);
        }
    }
}