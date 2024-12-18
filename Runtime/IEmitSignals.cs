using System;

namespace DGP.UnitySignals
{
    public interface IEmitSignals
    {
        public delegate void SignalChangedDelegate(IEmitSignals sender);
        public event SignalChangedDelegate SignalChanged;
        
        public void AddObserver(SignalChangedDelegate observer);
        public void RemoveObserver(SignalChangedDelegate observer);
    }
    
    public interface IEmitSignals<TValueType> : IEmitSignals where TValueType : IEquatable<TValueType>
    { 
        public delegate void SignalChangedHandler(IEmitSignals<TValueType> sender, TValueType oldValue, TValueType newValue);
        public event SignalChangedHandler SignalValueChanged;

        public void AddObserver(ISignalObserver<TValueType> observer);
        public void RemoveObserver(ISignalObserver<TValueType> observer);
        
        public void AddObserver(SignalChangedHandler observer);
        public void RemoveObserver(SignalChangedHandler observer);
        
        public void AddObserver(Action<TValueType> observer);
        public void RemoveObserver(Action<TValueType> observer);
        
        public void ClearObservers();
        
        public TValueType GetValue();
    }
}