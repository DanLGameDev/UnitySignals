using System;

namespace DGP.UnitySignals
{
    public interface ISignalObserver<TValueType> where TValueType : IEquatable<TValueType>
    {
        public void SignalValueChanged(IEmitSignals<TValueType> emitter, TValueType oldValue, TValueType newValue);
    }
}