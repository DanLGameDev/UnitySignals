namespace DGP.UnitySignals.Signals
{
    public class ByteValueSignal : ValueSignal<byte>
    {
        public ByteValueSignal(byte value = default(byte)) : base(value) { }
    }
}