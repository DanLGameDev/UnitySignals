using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DGP.UnitySignals.Signals
{
    public class CalculatedSignal<TSignalType> : SignalBase<TSignalType> where TSignalType : IEquatable<TSignalType>
    {
        private readonly Expression<Func<TSignalType>> _signalExpression;
        private readonly Func<TSignalType> _signalDelegate;
        private TSignalType _currentValue;
        private readonly HashSet<IEmitSignals> _sourceSignals = new();
        
        public override TSignalType GetValue() => _currentValue;
        
        public CalculatedSignal(Expression<Func<TSignalType>> signalExpression)
        {
            _signalExpression = signalExpression;
            _signalDelegate = signalExpression.Compile();
            _currentValue = _signalDelegate();

            FindDependentSignals(signalExpression.Body);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            foreach (var signal in _sourceSignals)
                signal.SignalChanged -= OnSourceSignalChanged;
            
            _sourceSignals.Clear();
        }
        
        private void FindDependentSignals(Expression expression)
        {
            switch (expression) {
                case MemberExpression memberExpression:
                    SubscribeToSignal(memberExpression);
                    break;
                case MethodCallExpression methodCallExpression:
                    if (methodCallExpression.Object is MemberExpression objMember)
                        SubscribeToSignal(objMember);
                    
                    foreach (var argument in methodCallExpression.Arguments)
                        FindDependentSignals(argument);
                    
                    break;
                case BinaryExpression binaryExpression:
                    FindDependentSignals(binaryExpression.Left);
                    FindDependentSignals(binaryExpression.Right);
                    break;
                case UnaryExpression unaryExpression:
                    FindDependentSignals(unaryExpression.Operand);
                    break;
            }
        }

        private void SubscribeToSignal(MemberExpression memberExpression)
        {
            if (memberExpression == null) return;

            var signal = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
            if (signal is IEmitSignals sourceSignal)
            {
                sourceSignal.AddObserver(OnSourceSignalChanged);
                _sourceSignals.Add(sourceSignal);
            }
        }

        private void OnSourceSignalChanged(IEmitSignals sender)
        {
            var oldValue = _currentValue;
            var newValue = _signalDelegate();
    
            if (!newValue.Equals(oldValue))
            {
                _currentValue = newValue;
                NotifyObservers(oldValue, newValue);
            }
        }
        
        
    }
}