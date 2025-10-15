using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DGP.UnitySignals.Signals
{
    public class PropertyNotifySignal<TValueType> : SignalBase<TValueType> where TValueType : INotifyPropertyChanged
    {
        private TValueType _value;
        
        public TValueType Value
        {
            get => _value;
            set => SetValue(value);
        }
        
        public override TValueType GetValue() => _value;

        public PropertyNotifySignal(TValueType value = default(TValueType))
        {
            SetValue(value);
        }

        public virtual void SetValue(TValueType value)
        {
            if (ReferenceEquals(_value, value))
                return;
            
            if (_value != null)
                _value.PropertyChanged -= OnPropertyChanged;
            
            TValueType oldValue = _value;
            _value = value;
            
            if (_value != null)
                _value.PropertyChanged += OnPropertyChanged;
            
            NotifyObservers(oldValue, _value);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
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