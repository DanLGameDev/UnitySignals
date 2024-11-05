using System;

namespace DGP.UnitySignals
{
    public interface ISignalObserver<TValueType> where TValueType : IComparable<TValueType>
    {
        public void SignalValueChanged(IEmitSignals<TValueType> emitter, TValueType newValue, TValueType oldValue);
    }
}