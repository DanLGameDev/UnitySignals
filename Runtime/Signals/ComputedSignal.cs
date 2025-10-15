using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DGP.UnitySignals.Signals
{
    public class ComputedSignal<TSignalType> : SignalBase<TSignalType>
    {
        private readonly Expression<Func<TSignalType>> _signalExpression;
        private readonly Func<TSignalType> _signalDelegate;
        private TSignalType _currentValue;
        private readonly HashSet<IEmitSignals> _sourceSignals = new();
        
        public override TSignalType GetValue() => _currentValue;
        
        public TSignalType Value => _currentValue;

        public ComputedSignal(Expression<Func<TSignalType>> signalExpression)
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
    
            if (!EqualityComparer<TSignalType>.Default.Equals(newValue, oldValue))
            {
                _currentValue = newValue;
                NotifyObservers(oldValue, newValue);
            }
        }
    }
}