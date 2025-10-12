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
        
        [Test]
        public void TestValueSignalNoNotificationOnSameValue()
        {
            int invoked = 0;
            var signal = new IntegerValueSignal(42);
    
            signal.AddObserver((sender, oldValue, newValue) => invoked++);
    
            signal.SetValue(42); // Same value
            Assert.AreEqual(0, invoked); // Should NOT invoke
    
            signal.SetValue(99); // Different value
            Assert.AreEqual(1, invoked); // Should invoke
        }
        
        [Test]
        public void TestValueProperty()
        {
            var signal = new IntegerValueSignal(10);
            Assert.AreEqual(10, signal.Value);
    
            int invoked = 0;
            signal.AddObserver((sender, oldValue, newValue) => invoked++);
    
            signal.Value = 20; // Using property instead of SetValue
            Assert.AreEqual(20, signal.Value);
            Assert.AreEqual(1, invoked);
        }
        
    }
}