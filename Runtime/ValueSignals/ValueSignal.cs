using System;

namespace DGP.UnitySignals.ValueSignals
{
    public abstract class ValueSignal<TValueType> : SignalBase<TValueType> where TValueType : IEquatable<TValueType>
    {
        private TValueType _value;
        
        public override TValueType GetValue() => _value;

        protected ValueSignal(TValueType value = default(TValueType)) => _value = value;

        public virtual void SetValue(TValueType value)
        {
            TValueType oldValue = _value;
            _value = value;
            NotifyObservers(oldValue, _value);
        }
    }
}