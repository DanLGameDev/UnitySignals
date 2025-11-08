using System.Collections.Generic;

namespace DGP.UnitySignals.Signals
{
    public class ValueSignal<TValueType> : SignalBase<TValueType>, ISettableSignal<TValueType>, ITransactableSignal<TValueType>
    {
        private TValueType _value;
        private TValueType _pendingOldValue;
        private bool _hasPendingNotification;

        public TValueType Value
        {
            get => _value;
            set => SetValue(value);
        }
        
        public ValueSignal(TValueType value = default(TValueType)) => _value = value;
        
        public override TValueType GetValue() => _value;
        
        public virtual void SetValue(TValueType value)
        {
            if (EqualityComparer<TValueType>.Default.Equals(_value, value))
                return;
            
            TValueType oldValue = _value;
            _value = value;

            NotifyObservers(oldValue, _value);
        }

        public bool SetValueSilently(TValueType newValue)
        {
            if (EqualityComparer<TValueType>.Default.Equals(_value, newValue))
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