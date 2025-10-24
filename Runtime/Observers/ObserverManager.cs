using System;
using System.Collections.Generic;

namespace DGP.UnitySignals.Observers
{
    public class ObserverManager<T>
    {
        private readonly List<IEmitSignals.SignalChangedDelegate> _untypedObservers = new();
        private readonly List<ISignalObserver<T>> _objectObservers = new();
        private readonly List<IEmitSignals<T>.SignalChangedHandler> _delegateObservers = new();
        private readonly List<Action<T>> _actionObservers = new();

        // Flags and deferred removal lists
        private bool _isNotifying;
        private List<IEmitSignals.SignalChangedDelegate> _untypedPendingRemovals;
        private List<ISignalObserver<T>> _objectPendingRemovals;
        private List<IEmitSignals<T>.SignalChangedHandler> _delegatePendingRemovals;
        private List<Action<T>> _actionPendingRemovals;

        // Add observer methods
        public void AddObserver(IEmitSignals.SignalChangedDelegate observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (!_untypedObservers.Contains(observer))
                _untypedObservers.Add(observer);
        }

        public void AddObserver(ISignalObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (!_objectObservers.Contains(observer))
                _objectObservers.Add(observer);
        }

        public void AddObserver(IEmitSignals<T>.SignalChangedHandler observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (!_delegateObservers.Contains(observer))
                _delegateObservers.Add(observer);
        }

        public void AddObserver(Action<T> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (!_actionObservers.Contains(observer))
                _actionObservers.Add(observer);
        }

        // Remove observer methods with deferred removal
        public void RemoveObserver(IEmitSignals.SignalChangedDelegate observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            if (_isNotifying) {
                // Defer removal
                _untypedPendingRemovals ??= new List<IEmitSignals.SignalChangedDelegate>();
                if (!_untypedPendingRemovals.Contains(observer))
                    _untypedPendingRemovals.Add(observer);
            } else {
                // Immediate removal
                _untypedObservers.Remove(observer);
            }
        }

        public void RemoveObserver(ISignalObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            if (_isNotifying) {
                // Defer removal
                _objectPendingRemovals ??= new List<ISignalObserver<T>>();
                if (!_objectPendingRemovals.Contains(observer))
                    _objectPendingRemovals.Add(observer);
            } else {
                // Immediate removal
                _objectObservers.Remove(observer);
            }
        }

        public void RemoveObserver(IEmitSignals<T>.SignalChangedHandler observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            if (_isNotifying) {
                // Defer removal
                _delegatePendingRemovals ??= new List<IEmitSignals<T>.SignalChangedHandler>();
                if (!_delegatePendingRemovals.Contains(observer))
                    _delegatePendingRemovals.Add(observer);
            } else {
                // Immediate removal
                _delegateObservers.Remove(observer);
            }
        }

        public void RemoveObserver(Action<T> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            if (_isNotifying) {
                // Defer removal
                _actionPendingRemovals ??= new List<Action<T>>();
                if (!_actionPendingRemovals.Contains(observer))
                    _actionPendingRemovals.Add(observer);
            } else {
                // Immediate removal
                _actionObservers.Remove(observer);
            }
        }

        // Notify observers in natural order - without trying to invoke events
        public void NotifyObservers(IEmitSignals<T> emitter, T oldValue, T newValue)
        {
            if (emitter == null) 
                throw new ArgumentNullException(nameof(emitter));
    
            _isNotifying = true;
    
            // Notify untyped observers
            foreach (var observer in _untypedObservers)
                observer(emitter);
        
            // Notify object observers
            foreach (var observer in _objectObservers)
                observer.SignalValueChanged(emitter, oldValue, newValue);
        
            // Notify delegate observers
            foreach (var observer in _delegateObservers)
                observer(emitter, oldValue, newValue);
        
            // Notify action observers
            foreach (var observer in _actionObservers)
                observer(newValue);
        
            _isNotifying = false;
    
            // Process any deferred removals
            ProcessPendingRemovals();
        }

        private void ProcessPendingRemovals()
        {
            // Process untyped observer removals
            if (_untypedPendingRemovals != null && _untypedPendingRemovals.Count > 0) {
                foreach (var observer in _untypedPendingRemovals)
                    _untypedObservers.Remove(observer);

                _untypedPendingRemovals.Clear();
            }

            // Process object observer removals
            if (_objectPendingRemovals != null && _objectPendingRemovals.Count > 0) {
                foreach (var observer in _objectPendingRemovals)
                    _objectObservers.Remove(observer);

                _objectPendingRemovals.Clear();
            }

            // Process delegate observer removals
            if (_delegatePendingRemovals != null && _delegatePendingRemovals.Count > 0) {
                foreach (var observer in _delegatePendingRemovals)
                    _delegateObservers.Remove(observer);

                _delegatePendingRemovals.Clear();
            }

            // Process action observer removals
            if (_actionPendingRemovals != null && _actionPendingRemovals.Count > 0) {
                foreach (var observer in _actionPendingRemovals)
                    _actionObservers.Remove(observer);

                _actionPendingRemovals.Clear();
            }
        }

        public void ClearObservers()
        {
            // Clear all observers
            _untypedObservers.Clear();
            _objectObservers.Clear();
            _delegateObservers.Clear();
            _actionObservers.Clear();

            // Clear pending removals as well
            _untypedPendingRemovals?.Clear();
            _objectPendingRemovals?.Clear();
            _delegatePendingRemovals?.Clear();
            _actionPendingRemovals?.Clear();
        }
    }
}