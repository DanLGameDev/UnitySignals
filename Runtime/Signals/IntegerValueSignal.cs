namespace DGP.UnitySignals.Signals
{
    public class IntegerValueSignal : ValueSignal<int>
    {
        public IntegerValueSignal(int value) : base(value) { }
        public IntegerValueSignal() : base() { }
    }
}