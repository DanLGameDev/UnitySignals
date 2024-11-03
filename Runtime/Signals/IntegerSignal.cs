namespace UnitySignals.Signals
{
    public class IntegerSignal : Signal<int>
    {
        public IntegerSignal(int initialState) : base(initialState) {}
    }
}