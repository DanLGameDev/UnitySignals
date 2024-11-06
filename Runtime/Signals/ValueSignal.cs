using System;
using Codice.Utils;

namespace DGP.UnitySignals.Signals
{
    public abstract class ValueSignal<TValueType> : SignalBase<TValueType> where TValueType : IComparable<TValueType>
    {
        private TValueType _value;
        
        public override TValueType GetValue() => _value;

        protected ValueSignal(TValueType value) => _value = value;
        protected ValueSignal() : this(default(TValueType)) { }

        public virtual void SetValue(TValueType value)
        {
            TValueType oldValue = _value;
            _value = value;
            NotifyObservers(oldValue, _value);
        }
    }
}