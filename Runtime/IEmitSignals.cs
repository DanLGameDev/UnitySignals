using System;

namespace DGP.UnitySignals
{
    public interface IEmitSignals<TValueType> where TValueType : IEquatable<TValueType>
    { 
        public delegate void SignalChangedHandler(IEmitSignals<TValueType> sender, TValueType oldValue, TValueType newValue);
        public event SignalChangedHandler OnSignalChanged;

        public void AddObserver(ISignalObserver<TValueType> observer);
        public void RemoveObserver(ISignalObserver<TValueType> observer);
        
        public void AddObserver(IEmitSignals<TValueType>.SignalChangedHandler observer);
        public void RemoveObserver(IEmitSignals<TValueType>.SignalChangedHandler observer);
        
        public void AddObserver(Action<TValueType> observer);
        public void RemoveObserver(Action<TValueType> observer);
        
        public void ClearObservers();
        
        public TValueType GetValue();
    }
}