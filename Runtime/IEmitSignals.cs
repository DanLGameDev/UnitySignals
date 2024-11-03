using System;

namespace UnitySignals
{
    public interface IEmitSignals<TValueType> where TValueType : IComparable<TValueType>
    { 
        public delegate void SignalChangedHandler(IEmitSignals<TValueType> sender, TValueType oldValue, TValueType newValue);
        public event SignalChangedHandler OnValueChange;

        public void AddObserver(IObserve<TValueType> observer);
        public void RemoveObserver(IObserve<TValueType> observer);
        
        public void AddObserver(Action<TValueType> observer);
        public void RemoveObserver(Action<TValueType> observer);
        
        public TValueType GetValue();
    }
}