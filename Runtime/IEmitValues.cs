using System;

namespace ObservableGadgets
{
    public interface IEmitValues<TValueType>
    { 
        public delegate void ValueChangeHandler(IEmitValues<TValueType> sender, TValueType oldValue, TValueType newValue);
        public event ValueChangeHandler OnValueChange;

        public void AddObserver(IObserve<TValueType> observer);
        public void RemoveObserver(IObserve<TValueType> observer);
        
        public void AddObserver(Action<TValueType> observer);
        public void RemoveObserver(Action<TValueType> observer);
        
        public TValueType GetValue();
    }
}