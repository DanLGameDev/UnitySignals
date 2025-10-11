using System;
using System.ComponentModel;

namespace DGP.UnitySignals.Signals
{
    public class PropertyNotifySignal<TValueType> : SignalBase<TValueType> 
        where TValueType : INotifyPropertyChanged, IEquatable<TValueType>
    {
        private TValueType _value;
        
        public override TValueType GetValue() => _value;

        public PropertyNotifySignal(TValueType value = default(TValueType))
        {
            SetValue(value);
        }

        public virtual void SetValue(TValueType value)
        {
            // Unsubscribe from old value
            if (_value != null)
                _value.PropertyChanged -= OnPropertyChanged;
            
            TValueType oldValue = _value;
            _value = value;
            
            // Subscribe to new value
            if (_value != null)
                _value.PropertyChanged += OnPropertyChanged;
            
            NotifyObservers(oldValue, _value);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Re-notify observers that the object's internal state changed
            NotifyObservers(_value, _value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _value != null)
                _value.PropertyChanged -= OnPropertyChanged;
            
            base.Dispose(disposing);
        }
    }
}