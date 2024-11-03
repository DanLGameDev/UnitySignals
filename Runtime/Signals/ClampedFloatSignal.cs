namespace UnitySignals.Signals
{
    public class ClampedFloatSignal : ClampedNumericSignal<float>
    {
        public ClampedFloatSignal(float initialState, ClampMode clampMode, float min, float max) : base(initialState, clampMode, min, max) {}
    }
}