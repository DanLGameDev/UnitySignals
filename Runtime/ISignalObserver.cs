using System;

namespace DGP.UnitySignals
{
    public interface ISignalObserver<TValueType>
    {
        public void SignalValueChanged(IEmitSignals<TValueType> emitter, TValueType oldValue, TValueType newValue);
    }
}