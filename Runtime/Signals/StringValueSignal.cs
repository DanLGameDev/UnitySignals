namespace DGP.UnitySignals.Signals
{
    public class StringValueSignal : ValueSignal<string>
    {
        public StringValueSignal(string value=default(string)) : base(value) { }
    }
}