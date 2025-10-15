using DGP.UnitySignals.Signals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class ComputedSignalTest
    {
        [Test]
        public void TestCalculatedSignal()
        {
            IntegerValueSignal signal = new IntegerValueSignal(1);
            ComputedSignal<int> computedSignal = new ComputedSignal<int>(() => signal.GetValue() * 2);
            
            Assert.AreEqual(2, computedSignal.GetValue());
            
            signal.SetValue(2);
            Assert.AreEqual(4, computedSignal.GetValue());
        }
        
        [Test]
        public void TestNestedCalculatedSignals()
        {
            IntegerValueSignal signal = new IntegerValueSignal(1);
            ComputedSignal<int> computedSignal = new ComputedSignal<int>(() => signal.GetValue() * 2);
            ComputedSignal<int> computedSignal2 = new ComputedSignal<int>(() => computedSignal.GetValue() * 2);
            
            Assert.AreEqual(4, computedSignal2.GetValue());
            
            signal.SetValue(2);
            Assert.AreEqual(8, computedSignal2.GetValue());
        }

        [Test]
        public void TestCalculatedThresholdValue()
        {
            FloatValueSignal oxygenRemaining = new(1.0f);
            float threshold = 0.25f;
            ComputedSignal<bool> oxygenLow = new(() => oxygenRemaining.GetValue() < threshold);
            
            Assert.IsFalse(oxygenLow.GetValue());
            
            oxygenRemaining.SetValue(0.2f);
            Assert.IsTrue(oxygenLow.GetValue());
        }
        
    }
}