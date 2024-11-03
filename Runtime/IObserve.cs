using System;

namespace UnitySignals
{
    public interface IObserve<TValueType> where TValueType : IComparable<TValueType>
    {
        public void ValueChanged(IEmitSignals<TValueType> sender, TValueType oldValue, TValueType newValue);
    }
}