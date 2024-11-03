namespace UnitySignals.Signals
{
    public class BooleanSignal : Signal<bool>
    {
        public BooleanSignal(bool initialState) : base(initialState) {}
    }
}