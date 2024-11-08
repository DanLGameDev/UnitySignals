using DGP.UnitySignals.Signals;
using DGP.UnitySignals.ValueSignals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class DelegateSignalTest
    {
        [Test]
        public void TestDelegateSignal()
        {
            IntegerValueSignal signal = new IntegerValueSignal(1);
            DelegateSignal<int> delegateSignal = new DelegateSignal<int>(() => signal.GetValue() * 2);
            
            Assert.AreEqual(2, delegateSignal.GetValue());
            
            signal.SetValue(2);
            Assert.AreEqual(4, delegateSignal.GetValue());
        }
        
        [Test]
        public void TestNestedDelegateSignals()
        {
            IntegerValueSignal signal = new IntegerValueSignal(1);
            DelegateSignal<int> delegateSignal = new DelegateSignal<int>(() => signal.GetValue() * 2);
            DelegateSignal<int> delegateSignal2 = new DelegateSignal<int>(() => delegateSignal.GetValue() * 2);
            
            Assert.AreEqual(4, delegateSignal2.GetValue());
            
            signal.SetValue(2);
            Assert.AreEqual(8, delegateSignal2.GetValue());
        }

        [Test]
        public void TestThresholdValue()
        {
            FloatValueSignal oxygenRemaining = new(1.0f);
            float threshold = 0.25f;
            DelegateSignal<bool> oxygenLow = new(() => oxygenRemaining.GetValue() < threshold);
            
            Assert.IsFalse(oxygenLow.GetValue());
            
            oxygenRemaining.SetValue(0.2f);
            Assert.IsTrue(oxygenLow.GetValue());
        }
        
    }
}