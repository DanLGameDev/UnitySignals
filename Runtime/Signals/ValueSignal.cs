using System;
using UnityEngine;

namespace DGP.UnitySignals.Signals
{
    public class ValueSignal<TValueType> : SignalBase<TValueType> where TValueType : IEquatable<TValueType>
    {
        private TValueType _value;
        
        public override TValueType GetValue() => _value;

        public ValueSignal(TValueType value = default(TValueType)) => _value = value;

        public virtual void SetValue(TValueType value)
        {
            TValueType oldValue = _value;
            _value = value;
            NotifyObservers(oldValue, _value);
        }
    }
    
    public class BooleanValueSignal : ValueSignal<bool> 
    {
        public BooleanValueSignal(bool value = default(bool)) : base(value) {}
    }
    public class FloatValueSignal : ValueSignal<float> 
    {
        public FloatValueSignal(float value = default(float)) : base(value) {}
    }
    public class IntegerValueSignal : ValueSignal<int> 
    {
        public IntegerValueSignal(int value = default(int)) : base(value) {}
    }
    public class StringValueSignal : ValueSignal<string> 
    {
        public StringValueSignal(string value = "") : base(value) {}
    }
    public class ByteValueSignal : ValueSignal<byte> 
    {
        public ByteValueSignal(byte value = default(byte)) : base(value) {}
    }
    public class DecimalValueSignal : ValueSignal<decimal> 
    {
        public DecimalValueSignal(decimal value = default(decimal)) : base(value) {}
    }
    public class DoubleValueSignal : ValueSignal<double> 
    {
        public DoubleValueSignal(double value = default(double)) : base(value) {}
    }
    public class LongValueSignal : ValueSignal<long> 
    {
        public LongValueSignal(long value = default(long)) : base(value) {}
    }
    public class CharValueSignal : ValueSignal<char> 
    {
        public CharValueSignal(char value = default(char)) : base(value) {}
    }
    public class Vector2ValueSignal : ValueSignal<Vector2> 
    {
        public Vector2ValueSignal(Vector2 value = default(Vector2)) : base(value) {}
    }
    public class Vector3ValueSignal : ValueSignal<Vector3> 
    {
        public Vector3ValueSignal(Vector3 value = default(Vector3)) : base(value) {}
    }
}