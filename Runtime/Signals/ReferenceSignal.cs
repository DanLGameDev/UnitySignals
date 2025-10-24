namespace DGP.UnitySignals.Signals
{
    public class ReferenceSignal<TObjectType> : SignalBase<TObjectType>
        where TObjectType : class
    {
        private TObjectType _value;

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
    }
}