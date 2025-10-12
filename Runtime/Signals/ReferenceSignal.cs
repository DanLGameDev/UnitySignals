using System;
using System.Collections.Generic;

namespace DGP.UnitySignals.Signals
{
    public class ReferenceSignal<TObjectType> : IEmitSignals<TObjectType>, IDisposable
        where TObjectType : class
    {
        public event IEmitSignals.SignalChangedDelegate SignalChanged;
        public event IEmitSignals<TObjectType>.SignalChangedHandler SignalValueChanged;

        private TObjectType _value;
        private readonly List<IEmitSignals.SignalChangedDelegate> _untypedObservers = new();
        private readonly List<ISignalObserver<TObjectType>> _objectObservers = new();
        private readonly List<IEmitSignals<TObjectType>.SignalChangedHandler> _delegateObservers = new();
        private readonly List<Action<TObjectType>> _actionObservers = new();

        public TObjectType GetValue() => _value;
        public static implicit operator TObjectType(ReferenceSignal<TObjectType> signal) => signal?.GetValue();

        public ReferenceSignal(TObjectType value = null) => _value = value;

        public virtual void SetValue(TObjectType value)
        {
            if (ReferenceEquals(_value, value))
                return;

            TObjectType oldValue = _value;
            _value = value;
            NotifyObservers(oldValue, value);
        }

        // IEmitSignals
        public void AddObserver(IEmitSignals.SignalChangedDelegate observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _untypedObservers.Add(observer);
        }

        public void RemoveObserver(IEmitSignals.SignalChangedDelegate observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _untypedObservers.Remove(observer);
        }

        // IEmitSignals<TObjectType>
        public void AddObserver(ISignalObserver<TObjectType> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _objectObservers.Add(observer);
        }

        public void RemoveObserver(ISignalObserver<TObjectType> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _objectObservers.Remove(observer);
        }

        public void AddObserver(IEmitSignals<TObjectType>.SignalChangedHandler observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _delegateObservers.Add(observer);
        }

        public void RemoveObserver(IEmitSignals<TObjectType>.SignalChangedHandler observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _delegateObservers.Remove(observer);
        }

        public void AddObserver(Action<TObjectType> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _actionObservers.Add(observer);
        }

        public void RemoveObserver(Action<TObjectType> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _actionObservers.Remove(observer);
        }

        protected void NotifyObservers(TObjectType oldValue, TObjectType newValue)
        {
            SignalChanged?.Invoke(this);
            SignalValueChanged?.Invoke(this, oldValue, newValue);

            foreach (var observer in _untypedObservers)
                observer(this);

            foreach (var observer in _objectObservers)
                observer.SignalValueChanged(this, oldValue, newValue);

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

        public void Dispose()
        {
            ClearObservers();
            GC.SuppressFinalize(this);
        }
    }
}