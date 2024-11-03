using System;
using System.Collections.Generic;

namespace UnitySignals.Signals
{
    public abstract class Signal<TValueType> : IReceiveSignals<TValueType>, IEmitSignals<TValueType> where TValueType : IComparable<TValueType>
    {
        private TValueType _value;
        
        private readonly HashSet<IObserve<TValueType>> _interfaceObservers = new();
        private readonly HashSet<Action<TValueType>> _actionObservers = new();
        
        protected Signal(TValueType initialValue)
        {
            _value = initialValue;
        }

        // IEmitSignals
        public TValueType GetValue() => _value;
        public event IEmitSignals<TValueType>.SignalChangedHandler OnValueChange;
        
        #region Observer Management
        public void AddObserver(IObserve<TValueType> observer) => _interfaceObservers.Add(observer);
        public void AddObserver(Action<TValueType> observer) => _actionObservers.Add(observer);
        public void RemoveObserver(IObserve<TValueType> observer) => _interfaceObservers.Remove(observer);
        public void RemoveObserver(Action<TValueType> observer) => _actionObservers.Remove(observer);
        #endregion
        
        // IReceiveSignals
        public void SetValue(TValueType newValue) => SetValueInternal(newValue);

        /// <summary>
        /// Changes the value of the signal and invokes the state change handlers.
        /// </summary>
        /// <param name="newValue">The desired new state</param>
        /// <returns>False if the state did not change or true if it did. Expected to be used to trigger additional handlers or similar in derived classes</returns>
        protected virtual bool SetValueInternal(TValueType newValue)
        {
            if (_value.Equals(newValue))
                return false;

            TValueType oldValue = _value;
            _value = newValue;

            EmitChanges(oldValue, newValue);
            
            return true;
        }
        
        protected virtual void EmitChanges(TValueType oldValue, TValueType newValue)
        {
            OnValueChange?.Invoke(this, oldValue, newValue);
            
            foreach (var observer in _interfaceObservers)
                observer.ValueChanged(this, oldValue, newValue);
            
            foreach (var observer in _actionObservers)
                observer.Invoke(newValue);
        }
    }
}