using System;
using System.Collections.Generic;

namespace DGP.UnitySignals
{
    public abstract class SignalBase<TSignalType> : IEmitSignals<TSignalType> where TSignalType : IComparable<TSignalType>
    {
        public event IEmitSignals<TSignalType>.SignalChangedHandler OnSignalChanged;
        
        private readonly HashSet<ISignalObserver<TSignalType>> _objectObservers = new();
        private readonly HashSet<Action<TSignalType>> _delegateObservers = new();
        
        public void AddObserver(ISignalObserver<TSignalType> observer) => _objectObservers.Add(observer);
        public void RemoveObserver(ISignalObserver<TSignalType> observer) => _objectObservers.Remove(observer);

        public void AddObserver(Action<TSignalType> observer) => _delegateObservers.Add(observer);
        public void RemoveObserver(Action<TSignalType> observer) => _delegateObservers.Remove(observer);

        public abstract TSignalType GetValue();
        protected void NotifyObservers(TSignalType oldValue, TSignalType newValue)
        {
            OnSignalChanged?.Invoke(this, oldValue, newValue);
            
            foreach (var observer in _objectObservers)
                observer.SignalValueChanged(this, newValue, oldValue);
            
            foreach (var observer in _delegateObservers)
                observer(newValue);
        }
    }
}