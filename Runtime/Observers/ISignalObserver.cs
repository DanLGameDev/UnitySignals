namespace DGP.UnitySignals.Observers
{
    public interface ISignalObserver<TValueType>
    {
        public void SignalValueChanged(IEmitSignals<TValueType> emitter, TValueType oldValue, TValueType newValue);
    }
}