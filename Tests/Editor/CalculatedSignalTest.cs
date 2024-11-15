using DGP.UnitySignals.Signals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class CalculatedSignalTest
    {
        [Test]
        public void TestCalculatedSignal()
        {
            IntegerValueSignal signal = new IntegerValueSignal(1);
            CalculatedSignal<int> calculatedSignal = new CalculatedSignal<int>(() => signal.GetValue() * 2);
            
            Assert.AreEqual(2, calculatedSignal.GetValue());
            
            signal.SetValue(2);
            Assert.AreEqual(4, calculatedSignal.GetValue());
        }
        
        [Test]
        public void TestNestedCalculatedSignals()
        {
            IntegerValueSignal signal = new IntegerValueSignal(1);
            CalculatedSignal<int> calculatedSignal = new CalculatedSignal<int>(() => signal.GetValue() * 2);
            CalculatedSignal<int> calculatedSignal2 = new CalculatedSignal<int>(() => calculatedSignal.GetValue() * 2);
            
            Assert.AreEqual(4, calculatedSignal2.GetValue());
            
            signal.SetValue(2);
            Assert.AreEqual(8, calculatedSignal2.GetValue());
        }

        [Test]
        public void TestCalculatedThresholdValue()
        {
            FloatValueSignal oxygenRemaining = new(1.0f);
            float threshold = 0.25f;
            CalculatedSignal<bool> oxygenLow = new(() => oxygenRemaining.GetValue() < threshold);
            
            Assert.IsFalse(oxygenLow.GetValue());
            
            oxygenRemaining.SetValue(0.2f);
            Assert.IsTrue(oxygenLow.GetValue());
        }
        
    }
}