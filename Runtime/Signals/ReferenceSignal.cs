using System;
using DGP.UnitySignals.Observers;
using UnityEngine;

namespace DGP.UnitySignals.Signals
{
    public class ReferenceSignal<TObjectType> : IEmitSignals<TObjectType>, IDisposable
        where TObjectType : class
    {
        public event IEmitSignals.SignalChangedDelegate SignalChanged;
        public event IEmitSignals.SignalDirtiedDelegate SignalDirtied;
        public event IEmitSignals<TObjectType>.SignalChangedHandler SignalValueChanged;
        public event IEmitSignals.SignalDiedDelegate SignalDied;

        private TObjectType _value;
        private readonly ObserverManager<TObjectType> _observerManager = new();
        private bool _isDead = false;
        private bool _isDirty = false;

        public bool IsDead => _isDead;

        public TObjectType GetValue() => _value;
        public static implicit operator TObjectType(ReferenceSignal<TObjectType> signal) => signal?.GetValue();

        public ReferenceSignal(TObjectType value = null) => _value = value;

        public virtual void SetValue(TObjectType value)
        {
            if (ReferenceEquals(_value, value))
                return;

            TObjectType oldValue = _value;
            _value = value;
            
            // Mark as dirty
            MarkDirty();
            
            // Notify observers
            NotifyObservers(oldValue, value);
        }

        // IEmitSignals implementation
        public void AddObserver(IEmitSignals.SignalChangedDelegate observer)
        {
            _observerManager.AddObserver(observer);
        }

        public void RemoveObserver(IEmitSignals.SignalChangedDelegate observer)
        {
            _observerManager.RemoveObserver(observer);
        }

        // IEmitSignals<TObjectType> implementation
        public void AddObserver(ISignalObserver<TObjectType> observer)
        {
            _observerManager.AddObserver(observer);
        }

        public void RemoveObserver(ISignalObserver<TObjectType> observer)
        {
            _observerManager.RemoveObserver(observer);
        }

        public void AddObserver(IEmitSignals<TObjectType>.SignalChangedHandler observer)
        {
            _observerManager.AddObserver(observer);
        }

        public void RemoveObserver(IEmitSignals<TObjectType>.SignalChangedHandler observer)
        {
            _observerManager.RemoveObserver(observer);
        }

        public void AddObserver(Action<TObjectType> observer)
        {
            _observerManager.AddObserver(observer);
        }

        public void RemoveObserver(Action<TObjectType> observer)
        {
            _observerManager.RemoveObserver(observer);
        }

        protected void NotifyObservers(TObjectType oldValue, TObjectType newValue)
        {
            // Raise events directly
            SignalChanged?.Invoke(this);
            SignalValueChanged?.Invoke(this, oldValue, newValue);
            
            // Use observer manager for other observers
            _observerManager.NotifyObservers(this, oldValue, newValue);
            
            // Clear dirty flag after notification
            _isDirty = false;
        }
        
        /// <summary>
        /// Marks the signal as dirty, indicating that its value has changed
        /// </summary>
        protected void MarkDirty()
        {
            _isDirty = true;
        }
        
        /// <summary>
        /// Checks if the signal is currently marked as dirty
        /// </summary>
        public bool IsDirty()
        {
            return _isDirty;
        }
        
        /// <summary>
        /// Marks the signal as dead and notifies observers
        /// </summary>
        protected void MarkAsDead()
        {
            if (!_isDead)
            {
                _isDead = true;
                SignalDied?.Invoke(this);
                Debug.Log($"ReferenceSignal has been marked as dead");
            }
        }

        public void ClearObservers()
        {
            _observerManager.ClearObservers();
        }

        public void Dispose()
        {
            if (!_isDead)
            {
                MarkAsDead();
                ClearObservers();
            }
            GC.SuppressFinalize(this);
        }
    }
}