using System;
using DGP.UnitySignals.Observers;

namespace DGP.UnitySignals
{
    public abstract class SignalBase<TValueType> : IEmitSignals<TValueType>, IDisposable
    {
        public event IEmitSignals.SignalChangedDelegate SignalChanged;
        public event IEmitSignals.SignalDirtiedDelegate SignalDirtied;
        public event IEmitSignals<TValueType>.SignalChangedHandler SignalValueChanged;
        
        private readonly ObserverManager<TValueType> _observerManager = new();
        
        public event IEmitSignals.SignalDiedDelegate SignalDied;
        public bool IsDead { get; private set; }

        public static implicit operator TValueType(SignalBase<TValueType> signal) => signal.GetValue();
        
        // IEmitSignals implementation
        public void AddObserver(IEmitSignals.SignalChangedDelegate observer) => _observerManager.AddObserver(observer);
        public void RemoveObserver(IEmitSignals.SignalChangedDelegate observer) => _observerManager.RemoveObserver(observer);

        // IEmitSignals<TValueType> implementation
        public void AddObserver(ISignalObserver<TValueType> observer, bool triggerImmediately = false)
        {
            _observerManager.AddObserver(observer);
            
            if (triggerImmediately)
                observer.SignalValueChanged(this, GetValue(), GetValue());
        }

        public void RemoveObserver(ISignalObserver<TValueType> observer) => _observerManager.RemoveObserver(observer);

        public void AddObserver(IEmitSignals<TValueType>.SignalChangedHandler observer, bool triggerImmediately = false)
        {
            _observerManager.AddObserver(observer);
            
            if (triggerImmediately)
                observer(this, GetValue(), GetValue());
        }

        public void RemoveObserver(IEmitSignals<TValueType>.SignalChangedHandler observer) => _observerManager.RemoveObserver(observer);

        public void AddObserver(Action<TValueType> observer, bool triggerImmediately = false)
        {
            _observerManager.AddObserver(observer);
            
            if (triggerImmediately)
                observer(GetValue());
        }

        public void RemoveObserver(Action<TValueType> observer) => _observerManager.RemoveObserver(observer);

        public abstract TValueType GetValue();
        public virtual TValueType Value => GetValue();
        
        protected void MarkAsDead()
        {
            if (IsDead) 
                return;
            
            IsDead = true;
            SignalDied?.Invoke(this);
        }
        
        protected void SignalAsDirty() => SignalDirtied?.Invoke(this);

        /// <summary>
        /// Notifies all observers that the signal's value has changed
        /// </summary>
        protected void NotifyObservers(TValueType oldValue, TValueType newValue)
        {
            SignalAsDirty();
            SignalChanged?.Invoke(this);
            SignalValueChanged?.Invoke(this, oldValue, newValue);
            
            _observerManager.NotifyObservers(this, oldValue, newValue);
        }
        
        public void ClearObservers() => _observerManager.ClearObservers();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) 
                return;
            
            MarkAsDead();
            ClearObservers();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}