using System;

namespace DGP.UnitySignals
{
    public interface IEmitSignals<TValueType> where TValueType : IComparable<TValueType>
    { 
        public delegate void SignalChangedHandler(IEmitSignals<TValueType> sender, TValueType oldValue, TValueType newValue);
        public event SignalChangedHandler OnSignalChanged;

        public void AddObserver(ISignalObserver<TValueType> observer);
        public void RemoveObserver(ISignalObserver<TValueType> observer);
        
        public void AddObserver(Action<TValueType> observer);
        public void RemoveObserver(Action<TValueType> observer);
        
        public TValueType GetValue();
    }
}