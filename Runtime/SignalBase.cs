using System;
using System.Collections.Generic;

namespace DGP.UnitySignals
{
    public abstract class SignalBase<TValueType> : IEmitSignals<TValueType>, IDisposable where TValueType : IEquatable<TValueType>
    {
        public event IEmitSignals.SignalChangedDelegate SignalChanged;
        public event IEmitSignals<TValueType>.SignalChangedHandler SignalValueChanged;
        
        private readonly List<IEmitSignals.SignalChangedDelegate> _untypedObservers = new();
        private readonly List<ISignalObserver<TValueType>> _objectObservers = new();
        private readonly List<IEmitSignals<TValueType>.SignalChangedHandler> _delegateObservers = new();
        private readonly List<Action<TValueType>> _actionObservers = new();
        
        public static implicit operator TValueType(SignalBase<TValueType> signal) => signal.GetValue();
        
        // IEmitSignals
        public void AddObserver(IEmitSignals.SignalChangedDelegate observer) => _untypedObservers.Add(observer);
        public void RemoveObserver(IEmitSignals.SignalChangedDelegate observer) => _untypedObservers.Remove(observer);

        // IEmitSignals<TValueType>
        public void AddObserver(ISignalObserver<TValueType> observer) => _objectObservers.Add(observer);
        public void RemoveObserver(ISignalObserver<TValueType> observer) => _objectObservers.Remove(observer);

        public void AddObserver(IEmitSignals<TValueType>.SignalChangedHandler observer) => _delegateObservers.Add(observer);
        public void RemoveObserver(IEmitSignals<TValueType>.SignalChangedHandler observer) => _delegateObservers.Remove(observer);
        
        public void AddObserver(Action<TValueType> observer) => _actionObservers.Add(observer);
        public void RemoveObserver(Action<TValueType> observer) => _actionObservers.Remove(observer);

        public abstract TValueType GetValue();
        
        protected void NotifyObservers(TValueType oldValue, TValueType newValue)
        {
            SignalChanged?.Invoke(this);
            SignalValueChanged?.Invoke(this, oldValue, newValue);
            
            foreach (var observer in _untypedObservers)
                observer(this);
            
            foreach (var observer in _objectObservers)
                observer.SignalValueChanged(this, newValue, oldValue);
            
            foreach (var observer in _delegateObservers)
                observer(this, oldValue, newValue);
            
            foreach (var observer in _actionObservers)
                observer(newValue);
        }
        
        public void ClearObservers()
        {
            _untypedObservers.Clear();
            _objectObservers.Clear();
            _delegateObservers.Clear();
            _actionObservers.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                ClearObservers();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}