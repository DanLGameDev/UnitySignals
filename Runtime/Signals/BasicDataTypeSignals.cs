namespace DGP.UnitySignals.Signals
{
    public class BooleanValueSignal : ValueSignal<bool> 
    {
        public BooleanValueSignal(bool value = false) : base(value) {}
    }
    
    public class FloatValueSignal : ValueSignal<float> 
    {
        public FloatValueSignal(float value = 0) : base(value) {}
    }
    
    public class IntegerValueSignal : ValueSignal<int> 
    {
        public IntegerValueSignal(int value = 0) : base(value) {}
    }
    
    public class StringValueSignal : ValueSignal<string> 
    {
        public StringValueSignal(string value = "") : base(value) {}
    }
    
    public class ByteValueSignal : ValueSignal<byte> 
    {
        public ByteValueSignal(byte value = 0) : base(value) {}
    }
    
    public class DecimalValueSignal : ValueSignal<decimal> 
    {
        public DecimalValueSignal(decimal value = 0) : base(value) {}
    }
    
    public class DoubleValueSignal : ValueSignal<double> 
    {
        public DoubleValueSignal(double value = 0) : base(value) {}
    }
    
    public class LongValueSignal : ValueSignal<long> 
    {
        public LongValueSignal(long value = 0) : base(value) {}
    }
    
    public class CharValueSignal : ValueSignal<char> 
    {
        public CharValueSignal(char value = '\0') : base(value) {}
    }
}