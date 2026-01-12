using DGP.UnitySignals.Observers;

namespace DGP.UnitySignals.Editor.Tests
{
    public class TestIntObserver : ISignalObserver<int>
    {
        public int Invoked = 0;
        public void SignalValueChanged(IEmitSignals<int> emitter, int oldValue, int newValue) => Invoked++;
    }
}