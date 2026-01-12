using DGP.UnitySignals.Signals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class SignalTests
    {
        [Test]
        public void TestSignalMarkAsDeadIsIdempotent()
        {
            var signal = new IntegerValueSignal(42);
            int diedEventCount = 0;
    
            signal.SignalDied += (sender) => diedEventCount++;
    
            signal.Dispose();
            Assert.AreEqual(1, diedEventCount);
            Assert.IsTrue(signal.IsDead);
    
            // Try to dispose again (which calls MarkAsDead internally)
            signal.Dispose();
            Assert.AreEqual(1, diedEventCount, "SignalDied should only fire once");
            Assert.IsTrue(signal.IsDead);
        }
    }
}