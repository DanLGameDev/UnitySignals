using System;

namespace DGP.UnitySignals.Signals
{
    public class StringValueSignal : ValueSignal<string>
    {
        public StringValueSignal(string value="") : base(value) { }
    }
}