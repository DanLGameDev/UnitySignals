namespace DGP.UnitySignals.Signals
{
    public class ReferenceSignal<TObjectType> : SignalBase<TObjectType>, ISettableSignal<TObjectType>, ITransactableSignal<TObjectType>
        where TObjectType : class
    {
        private TObjectType _value;
        private TObjectType _pendingOldValue;
        private bool _hasPendingNotification;

        public TObjectType Value
        {
            get => _value;
            set => SetValue(value);
        }
        
        public ReferenceSignal(TObjectType value = null) => _value = value;
        
        public override TObjectType GetValue() => _value;
        
        public virtual void SetValue(TObjectType value)
        {
            if (ReferenceEquals(_value, value))
                return;
            
            TObjectType oldValue = _value;
            _value = value;

            NotifyObservers(oldValue, _value);
        }

        public bool SetValueSilently(TObjectType newValue)
        {
            if (ReferenceEquals(_value, newValue))
                return false;

            if (!_hasPendingNotification)
                _pendingOldValue = _value;

            _value = newValue;
            _hasPendingNotification = true;
            return true;
        }

        public void FlushNotifications()
        {
            if (!_hasPendingNotification)
                return;

            NotifyObservers(_pendingOldValue, _value);
            _hasPendingNotification = false;
        }
    }
}