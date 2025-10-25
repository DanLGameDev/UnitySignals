using System;
using System.Collections.Generic;
using UnityEngine;

namespace DGP.UnitySignals.Signals
{
    public class ValueSignal<TValueType> : SignalBase<TValueType>
    {
        private TValueType _value;

        public TValueType Value
        {
            get => _value;
            set => SetValue(value);
        }
        
        public ValueSignal(TValueType value = default(TValueType)) => _value = value;
        
        public override TValueType GetValue() => _value;
        public virtual void SetValue(TValueType value)
        {
            if (EqualityComparer<TValueType>.Default.Equals(_value, value))
                return;
            
            TValueType oldValue = _value;
            _value = value;

            NotifyObservers(oldValue, _value);
        }
    }
}