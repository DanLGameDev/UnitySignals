using System;

namespace UnitySignals.Signals
{
    public abstract class ClampedNumericSignal<TValueType> : Signal<TValueType> where TValueType : IComparable<TValueType>
    {
        public ClampMode ClampMode { get; }
        public TValueType MinValue { get; }
        public TValueType MaxValue { get; }

        protected ClampedNumericSignal(TValueType initialState, ClampMode clampMode, TValueType minValue, TValueType maxValue) : base(initialState)
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