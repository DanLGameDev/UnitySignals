using DGP.UnitySignals.Signals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class SignalTransactionTests
    {
        [Test]
        public void TestTransactionCommitsOnDispose()
        {
            int invoked = 0;
            var signal = new IntegerValueSignal(10);
            
            signal.AddObserver((sender, oldValue, newValue) => invoked++);
            
            using (var transaction = new SignalTransaction())
            {
                transaction.Set(signal, 20);
                Assert.AreEqual(0, invoked); // Should not have notified yet
                Assert.AreEqual(20, signal.Value); // But value should be updated
            }
            
            Assert.AreEqual(1, invoked); // Should notify after dispose
        }
        
        [Test]
        public void TestTransactionExplicitCommit()
        {
            int invoked = 0;
            var signal = new IntegerValueSignal(10);
            
            signal.AddObserver((sender, oldValue, newValue) => invoked++);
            
            using (var transaction = new SignalTransaction())
            {
                transaction.Set(signal, 20);
                Assert.AreEqual(0, invoked);
                
                transaction.Commit();
                Assert.AreEqual(1, invoked); // Should notify after explicit commit
            }
            
            Assert.AreEqual(1, invoked); // Should still be 1 (no double commit)
        }
        
        [Test]
        public void TestTransactionMultipleSignals()
        {
            int invoked1 = 0;
            int invoked2 = 0;
            int invoked3 = 0;
            
            var signal1 = new IntegerValueSignal(10);
            var signal2 = new FloatValueSignal(5.0f);
            var signal3 = new StringValueSignal("hello");
            
            signal1.AddObserver((sender, oldValue, newValue) => invoked1++);
            signal2.AddObserver((sender, oldValue, newValue) => invoked2++);
            signal3.AddObserver((sender, oldValue, newValue) => invoked3++);
            
            using (var transaction = new SignalTransaction())
            {
                transaction.Set(signal1, 20)
                           .Set(signal2, 10.0f)
                           .Set(signal3, "world");
                
                Assert.AreEqual(0, invoked1);
                Assert.AreEqual(0, invoked2);
                Assert.AreEqual(0, invoked3);
            }
            
            Assert.AreEqual(1, invoked1);
            Assert.AreEqual(1, invoked2);
            Assert.AreEqual(1, invoked3);
        }
        
        [Test]
        public void TestTransactionWithUnchangedValue()
        {
            int invoked = 0;
            var signal = new IntegerValueSignal(10);
            
            signal.AddObserver((sender, oldValue, newValue) => invoked++);
            
            using (var transaction = new SignalTransaction())
            {
                transaction.Set(signal, 10); // Same value
            }
            
            Assert.AreEqual(0, invoked); // Should not notify for unchanged value
        }
        
        [Test]
        public void TestTransactionMultipleSetsOnSameSignal()
        {
            int invoked = 0;
            int capturedOld = 0;
            int capturedNew = 0;
            
            var signal = new IntegerValueSignal(10);
            
            signal.AddObserver((sender, oldValue, newValue) => {
                invoked++;
                capturedOld = oldValue;
                capturedNew = newValue;
            });
            
            using (var transaction = new SignalTransaction())
            {
                transaction.Set(signal, 20);
                transaction.Set(signal, 30);
                transaction.Set(signal, 40);
                
                Assert.AreEqual(40, signal.Value); // Should have latest value
            }
            
            Assert.AreEqual(1, invoked); // Should only notify once
            Assert.AreEqual(10, capturedOld); // Should have original old value
            Assert.AreEqual(40, capturedNew); // Should have final new value
        }
        
        [Test]
        public void TestTransactionReferenceSignal()
        {
            int invoked = 0;
            var obj1 = new object();
            var obj2 = new object();
            
            var signal = new ReferenceSignal<object>(obj1);
            
            signal.AddObserver((sender, oldValue, newValue) => invoked++);
            
            using (var transaction = new SignalTransaction())
            {
                transaction.Set(signal, obj2);
                Assert.AreEqual(0, invoked);
                Assert.AreEqual(obj2, signal.Value);
            }
            
            Assert.AreEqual(1, invoked);
        }
        
        [Test]
        public void TestTransactionCannotModifyAfterCommit()
        {
            var signal = new IntegerValueSignal(10);
            var transaction = new SignalTransaction();
            
            transaction.Set(signal, 20);
            transaction.Commit();
            
            Assert.Throws<System.InvalidOperationException>(() => {
                transaction.Set(signal, 30);
            });
        }
        
        [Test]
        public void TestTransactionCannotModifyAfterDispose()
        {
            var signal = new IntegerValueSignal(10);
            var transaction = new SignalTransaction();
            
            transaction.Set(signal, 20);
            transaction.Dispose();
            
            Assert.Throws<System.ObjectDisposedException>(() => {
                transaction.Set(signal, 30);
            });
        }
        
        [Test]
        public void TestTransactionMixedChangedAndUnchanged()
        {
            int invoked1 = 0;
            int invoked2 = 0;
            int invoked3 = 0;
            
            var signal1 = new IntegerValueSignal(10);
            var signal2 = new IntegerValueSignal(20);
            var signal3 = new IntegerValueSignal(30);
            
            signal1.AddObserver((sender, oldValue, newValue) => invoked1++);
            signal2.AddObserver((sender, oldValue, newValue) => invoked2++);
            signal3.AddObserver((sender, oldValue, newValue) => invoked3++);
            
            using (var transaction = new SignalTransaction())
            {
                transaction.Set(signal1, 100); // Changed
                transaction.Set(signal2, 20);  // Unchanged
                transaction.Set(signal3, 300); // Changed
            }
            
            Assert.AreEqual(1, invoked1); // Changed - should notify
            Assert.AreEqual(0, invoked2); // Unchanged - should not notify
            Assert.AreEqual(1, invoked3); // Changed - should notify
        }
    }
}