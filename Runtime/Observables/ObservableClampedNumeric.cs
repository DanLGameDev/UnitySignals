using System;

namespace ObservableGadgets.Observables
{
    public abstract class ObservableClampedNumeric<TValueType> : ObservableValue<TValueType> where TValueType : IComparable<TValueType>, IEquatable<TValueType>
    {
        public ClampMode ClampMode { get; }
        public TValueType MinValue { get; }
        public TValueType MaxValue { get; }

        protected ObservableClampedNumeric(TValueType initialState, ClampMode clampMode, TValueType minValue, TValueType maxValue) : base(initialState)
        {
            ClampMode = clampMode;
            MinValue = minValue;
            MaxValue = maxValue;
        }
        
        protected override bool SetValueInternal(TValueType newValue)
        {
            TValueType clampedValue = ApplyClamping(newValue);
            return base.SetValueInternal(clampedValue);
        }
        
        private TValueType ApplyClamping(TValueType value)
        {
            switch (ClampMode)
            {
                case ClampMode.Clamp:
                    return Clamper.Clamp<TValueType>(value, MinValue, MaxValue);
                case ClampMode.Wrap:
                    return Clamper.Wrap<TValueType>(value, MinValue, MaxValue);
                case ClampMode.None:
                    return value;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}