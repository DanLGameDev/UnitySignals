namespace DGP.UnitySignals.ValueSignals
{
    public class DecimalValueSignal : ValueSignal<decimal>
    {
        public DecimalValueSignal(decimal value=default(decimal)) : base(value) { }
    }
}