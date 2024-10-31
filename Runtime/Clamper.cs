using System;

namespace ObservableGadgets
{
    public static class Clamper
    {
        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <returns>The clamped value</returns>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }
        
        /// <summary>
        /// Wraps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <returns>The wrapped value</returns>
        public static T Wrap<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) > 0 && value.CompareTo(max) < 0) return value;
            
            T wrappedValue = (dynamic)value - (dynamic)min;
            wrappedValue = (dynamic)wrappedValue % ((dynamic)max - (dynamic)min) + (dynamic)min;
            
            return wrappedValue;
        }
        
    }
}