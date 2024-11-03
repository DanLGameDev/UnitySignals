namespace UnitySignals.Signals
{
    public class ClampedIntegerSignal : ClampedNumericSignal<int>
    {
        public ClampedIntegerSignal(int initialState, ClampMode clampMode, int min, int max) : base(initialState, clampMode, min, max) {}
    }
}