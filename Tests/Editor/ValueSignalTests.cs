using DGP.UnitySignals.Signals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class ValueSignalTests
    {
        [Test]
        public void TestIntSignalConstruction()
        {
            var signal = new IntegerValueSignal();
            Assert.IsNotNull(signal);
            Assert.AreEqual(0, signal.GetValue());
            
            signal = new IntegerValueSignal(42);
            Assert.IsNotNull(signal);
            Assert.AreEqual(42, signal.GetValue());
        }

        [Test]
        public void TestEventHandling()
        {
            int invoked = 0;
            var signal = new IntegerValueSignal();
            
            signal.SignalValueChanged += (sender, oldValue, newValue) => invoked++;
            signal.SetValue(42);
            
            Assert.AreEqual(1, invoked);
        }
        
        private class TestIntObserver : ISignalObserver<int>
        {
            public int Invoked = 0;
            public void SignalValueChanged(IEmitSignals<int> emitter, int oldValue, int newValue) => Invoked++;
        }
        
        [Test]
        public void TestObserverHandling()
        {
            var signal = new IntegerValueSignal();
            var observer = new TestIntObserver();
            
            signal.AddObserver(observer);
            signal.SetValue(42);
            
            Assert.AreEqual(1, observer.Invoked);
        }
        
        [Test]
        public void TestDelegateObserverHandling()
        {
            int invoked = 0;
            var signal = new IntegerValueSignal();
            
            signal.AddObserver((sender, oldValue, newValue) => invoked++);
            signal.SetValue(42);
            
            Assert.AreEqual(1, invoked);
        }
        
        [Test]
        public void TestActionObserverHandling()
        {
            int invoked = 0;
            var signal = new IntegerValueSignal();
            
            signal.AddObserver((IEmitSignals sender) => invoked++);
            signal.SetValue(42);
            
            Assert.AreEqual(1, invoked);
        }
        
    }
}