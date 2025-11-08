namespace DGP.UnitySignals
{
    public interface ISettableSignal<TValueType>
    {
        void SetValue(TValueType newValue);
    }
}