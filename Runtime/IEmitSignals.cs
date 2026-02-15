using System;
using DGP.UnitySignals.Observers;

namespace DGP.UnitySignals
{
    public interface IEmitSignals
    {
        public delegate void SignalChangedDelegate(IEmitSignals sender);
        public event SignalChangedDelegate SignalChanged;
        
        public delegate void SignalDirtiedDelegate(IEmitSignals sender);
        public event SignalDirtiedDelegate SignalDirtied;
        
        public delegate void SignalDiedDelegate(IEmitSignals sender);
        public event SignalDiedDelegate SignalDied;
        public bool IsDead { get; }
        
        public void AddObserver(SignalChangedDelegate observer);
        public void RemoveObserver(SignalChangedDelegate observer);
    }
    
    public interface IEmitSignals<TValueType> : IEmitSignals
    { 
        public delegate void SignalChangedHandler(IEmitSignals<TValueType> sender, TValueType oldValue, TValueType newValue);
        public event SignalChangedHandler SignalValueChanged;

        public void AddObserver(ISignalObserver<TValueType> observer, bool triggerImmediately = false);
        public void RemoveObserver(ISignalObserver<TValueType> observer);
        
        public void AddObserver(SignalChangedHandler observer, bool triggerImmediately = false);
        public void RemoveObserver(SignalChangedHandler observer);
        
        public void AddObserver(Action<TValueType> observer, bool triggerImmediately = false);
        public void RemoveObserver(Action<TValueType> observer);
        
        public void ClearObservers();
        
        public TValueType GetValue();
    }
}