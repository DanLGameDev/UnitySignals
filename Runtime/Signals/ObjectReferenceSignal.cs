using System;
using System.Collections.Generic;
using UnityEngine;

namespace DGP.UnitySignals.Signals
{
    public class ObjectReferenceSignal<TObjectType> : IEmitSignals, IDisposable 
        where TObjectType : UnityEngine.Object
    {
        public event IEmitSignals.SignalChangedDelegate SignalChanged;
        
        private TObjectType _value;
        private readonly List<IEmitSignals.SignalChangedDelegate> _observers = new();
        private readonly List<Action<TObjectType>> _actionObservers = new();
        
        public TObjectType GetValue() => _value;
        public static implicit operator TObjectType(ObjectReferenceSignal<TObjectType> signal) => signal.GetValue();
        
        public ObjectReferenceSignal(TObjectType value = null) => _value = value;
        
        public virtual void SetValue(TObjectType value)
        {
            if (_value == value) 
                return;
            
            TObjectType oldValue = _value;
            _value = value;
            NotifyObservers();
        }
        
        public void AddObserver(IEmitSignals.SignalChangedDelegate observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _observers.Add(observer);
        }
        
        public void RemoveObserver(IEmitSignals.SignalChangedDelegate observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _observers.Remove(observer);
        }
        
        public void AddObserver(Action<TObjectType> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _actionObservers.Add(observer);
        }
        
        public void RemoveObserver(Action<TObjectType> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _actionObservers.Remove(observer);
        }
        
        protected void NotifyObservers()
        {
            SignalChanged?.Invoke(this);
            
            foreach (var observer in _observers)
                observer(this);
                
            foreach (var observer in _actionObservers)
                observer(_value);
        }
        
        public void ClearObservers()
        {
            _observers.Clear();
            _actionObservers.Clear();
        }
        
        public void Dispose()
        {
            ClearObservers();
            GC.SuppressFinalize(this);
        }
    }
    
    // Convenience types
    public class GameObjectSignal : ObjectReferenceSignal<GameObject> 
    {
        public GameObjectSignal(GameObject value = null) : base(value) {}
    }
    
    public class TransformSignal : ObjectReferenceSignal<Transform> 
    {
        public TransformSignal(Transform value = null) : base(value) {}
    }
}