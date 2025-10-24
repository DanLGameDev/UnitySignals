using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace DGP.UnitySignals.Signals
{
    public class ComputedSignal<TSignalType> : SignalBase<TSignalType>
    {
        private readonly Expression<Func<TSignalType>> _signalExpression;
        private readonly Func<TSignalType> _signalDelegate;
        private TSignalType _currentValue;
        private readonly HashSet<IEmitSignals> _sourceSignals = new();
        private bool _isCalculating = false;

        private bool _isDirty = false;
        
        public override TSignalType GetValue()
        {
            if (HasDeadDependencies()) {
                Debug.LogWarning("ComputedSignal accessing value with dead dependencies");
                
                if (!IsDead)
                    MarkAsDead();
            }
            
            return _isDirty ? _signalDelegate.Invoke() : _currentValue;
        }
        
        public TSignalType Value => GetValue();

        public ComputedSignal(Expression<Func<TSignalType>> signalExpression)
        {
            _signalExpression = signalExpression;
            _signalDelegate = signalExpression.Compile();
            
            FindDependentSignals(signalExpression.Body);
            
            foreach (var signal in _sourceSignals)
            {
                signal.SignalDirtied += OnSourceSignalDirtied;
                signal.SignalChanged += OnSourceSignalChanged;
                signal.SignalDied += OnDependencyDied;
            }
            
            _currentValue = _signalDelegate();
        }
        
        private void OnDependencyDied(IEmitSignals sender)
        {
            MarkAsDead();
            Debug.LogWarning($"ComputedSignal marked as dead because dependency {sender} was disposed");
        }
        
        private bool HasDeadDependencies()
        {
            foreach (var signal in _sourceSignals)
            {
                if (signal.IsDead)
                    return true;
            }
    
            return false;
        }

        // Add a public method to check health explicitly
        public bool HasValidDependencies() => !HasDeadDependencies();

        /// <summary>
        /// Recalculates the value of this computed signal
        /// </summary>
        /// <param name="notifyObservers">Whether to notify observers if the value changes</param>
        /// <param name="newCurrentValue">The newly calculated value</param>
        /// <returns>True if the value changed, false otherwise</returns>
        private bool RecalculateValue(bool notifyObservers, out TSignalType newCurrentValue)
        {
            newCurrentValue = _currentValue;
            
            if (_isCalculating)
                return false;
                
            _isCalculating = true;
            
            try {
                var oldValue = _currentValue;
                var newValue = _signalDelegate();
                
                bool hasChanged = !EqualityComparer<TSignalType>.Default.Equals(newValue, oldValue);
                
                if (hasChanged) {
                    newCurrentValue = newValue;
                    
                    if (notifyObservers)
                        NotifyObservers(oldValue, newValue);
                    
                    return true;
                }
                
                return false;
            } finally {
                _isCalculating = false;
            }
        }

        private void OnSourceSignalDirtied(IEmitSignals sender)
        {
            _isDirty = true;
            SignalAsDirty();
        }

        private void OnSourceSignalChanged(IEmitSignals sender)
        {
            if (_isCalculating)
                return;
            
            RecalculateValue(notifyObservers: true, out var newValue);
            _currentValue = newValue;
            _isDirty = false;
        }
        
        private void FindDependentSignals(Expression expression, int depth = 0)
        {
            if (depth > 32)
                throw new Exception("Expression is too complex");
            
            switch (expression) {
                case MemberExpression memberExpression:
                    SubscribeToSignal(memberExpression);
                    // Recursively visit the object the member is accessed on
                    if (memberExpression.Expression != null)
                        FindDependentSignals(memberExpression.Expression, depth + 1);
                    break;
                case MethodCallExpression methodCallExpression:
                    if (methodCallExpression.Object is MemberExpression objMember)
                        SubscribeToSignal(objMember);
                    
                    if (methodCallExpression.Object != null)
                        FindDependentSignals(methodCallExpression.Object, depth + 1);
                    
                    foreach (var argument in methodCallExpression.Arguments)
                        FindDependentSignals(argument, depth+1);
                    
                    break;
                case BinaryExpression binaryExpression:
                    FindDependentSignals(binaryExpression.Left, depth+1);
                    FindDependentSignals(binaryExpression.Right, depth+1);
                    break;
                case UnaryExpression unaryExpression:
                    FindDependentSignals(unaryExpression.Operand, depth+1);
                    break;
                case ConditionalExpression conditionalExpression:
                    FindDependentSignals(conditionalExpression.Test, depth+1);
                    FindDependentSignals(conditionalExpression.IfTrue, depth+1);
                    FindDependentSignals(conditionalExpression.IfFalse, depth+1);
                    break;
            }
        }

        private void SubscribeToSignal(MemberExpression memberExpression)
        {
            if (memberExpression == null) 
                return;

            try {
                var signal = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                
                if (signal is IEmitSignals sourceSignal) {
                    if (ReferenceEquals(sourceSignal, (IEmitSignals)this))
                    {
                        Debug.LogWarning("ComputedSignal detected self-reference in expression. Ignoring to prevent infinite recursion.");
                        return;
                    }

                    _sourceSignals.Add(sourceSignal);
                }
            } catch (Exception ex) {
                Debug.LogWarning($"Error subscribing to signal: {ex.Message}");
            }
        }
        
        // IDisposable implementation
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                foreach (var signal in _sourceSignals) {
                    signal.SignalChanged -= OnSourceSignalChanged;
                    signal.SignalDied -= OnDependencyDied;
                }
        
                _sourceSignals.Clear();
            }
    
            base.Dispose(disposing);
        }
    }
}