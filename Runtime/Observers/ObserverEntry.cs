using System;
using System.Collections.Generic;

namespace DGP.UnitySignals.Observers
{
    internal class ObserverEntry<TObserver> where TObserver : class
    {
        public TObserver Observer { get; }
        public WeakReference<TObserver> WeakObserver { get; }
        public bool IsWeak { get; }
    
        public ObserverEntry(TObserver observer, bool weak = false)
        {
            if (weak)
            {
                WeakObserver = new WeakReference<TObserver>(observer);
                IsWeak = true;
            }
            else
            {
                Observer = observer;
                IsWeak = false;
            }
        }
    
        public bool TryGetObserver(out TObserver observer)
        {
            if (!IsWeak)
            {
                observer = Observer;
                return true;
            }
        
            return WeakObserver.TryGetTarget(out observer);
        }
    
        public bool Equals(TObserver other)
        {
            if (!IsWeak)
                return EqualityComparer<TObserver>.Default.Equals(Observer, other);
        
            if (WeakObserver.TryGetTarget(out var target))
                return EqualityComparer<TObserver>.Default.Equals(target, other);
            
            return false;
        }
    }
}