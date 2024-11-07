namespace DGP.UnitySignals.Signals
{
    public class LongValueSignal : ValueSignal<long>
    {
        public LongValueSignal(long value=default(long)) : base(value) { }
    }
}