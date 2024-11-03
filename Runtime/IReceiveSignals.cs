using System;

namespace UnitySignals
{
    public interface IReceiveSignals<TValueType> where TValueType : IComparable<TValueType>
    {
        public void SetValue(TValueType newValue);
    }
}