namespace UnitySignals.Signals
{
    public class StringSignal : Signal<string>
    {
        public StringSignal(string initialState) : base(initialState) { }
    }
}