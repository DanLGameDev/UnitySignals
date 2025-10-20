using System;
using System.Collections.Generic;

namespace DGP.UnitySignals
{
    public abstract class SignalBase<TValueType> : IEmitSignals<TValueType>, IDisposable
    {
        public event IEmitSignals.SignalChangedDelegate SignalChanged;
        public event IEmitSignals<TValueType>.SignalChangedHandler SignalValueChanged;
        
        private readonly List<IEmitSignals.SignalChangedDelegate> _untypedObservers = new();
        private readonly List<ISignalObserver<TValueType>> _objectObservers = new();
        private readonly List<IEmitSignals<TValueType>.SignalChangedHandler> _delegateObservers = new();
        private readonly List<Action<TValueType>> _actionObservers = new();
        
        public static implicit operator TValueType(SignalBase<TValueType> signal) => signal.GetValue();
        
        // IEmitSignals
        public void AddObserver(IEmitSignals.SignalChangedDelegate observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");
            _untypedObservers.Add(observer);
        }

        public void RemoveObserver(IEmitSignals.SignalChangedDelegate observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");
            _untypedObservers.Remove(observer);
        }

        // IEmitSignals<TValueType>
        public void AddObserver(ISignalObserver<TValueType> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");
            _objectObservers.Add(observer);
        }

        public void RemoveObserver(ISignalObserver<TValueType> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");
            _objectObservers.Remove(observer);
        }

        public void AddObserver(IEmitSignals<TValueType>.SignalChangedHandler observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");
            _delegateObservers.Add(observer);
        }

        public void RemoveObserver(IEmitSignals<TValueType>.SignalChangedHandler observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");
            _delegateObservers.Remove(observer);
        }

        public void AddObserver(Action<TValueType> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");
            _actionObservers.Add(observer);
        }

        public void RemoveObserver(Action<TValueType> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");
            _actionObservers.Remove(observer);
        }

        public abstract TValueType GetValue();
        
        protected void NotifyObservers(TValueType oldValue, TValueType newValue)
		{
    		// Handle the events first (these aren't affected by the iteration issue)
    		SignalChanged?.Invoke(this);
    		SignalValueChanged?.Invoke(this, oldValue, newValue);
    
    		// Iterate in reverse for all observer collections
		    for (int i = _untypedObservers.Count - 1; i >= 0; i--)
		        _untypedObservers[i](this);
    
    		for (int i = _objectObservers.Count - 1; i >= 0; i--)
        		_objectObservers[i].SignalValueChanged(this, newValue, oldValue);
    
		    for (int i = _delegateObservers.Count - 1; i >= 0; i--)
        		_delegateObservers[i](this, oldValue, newValue);
   
   		 	for (int i = _actionObservers.Count - 1; i >= 0; i--)
        		_actionObservers[i](newValue);
		}
        
        public void ClearObservers()
        {
            _untypedObservers.Clear();
            _objectObservers.Clear();
            _delegateObservers.Clear();
            _actionObservers.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                ClearObservers();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}