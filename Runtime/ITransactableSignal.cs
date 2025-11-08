namespace DGP.UnitySignals
{
    public interface ITransactableSignal
    {
        void FlushNotifications();
    }

    public interface ITransactableSignal<TValueType> : ITransactableSignal
    {
        bool SetValueSilently(TValueType newValue);
    }
}