using System;
using DGP.UnitySignals.Signals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class ObserverManagerTests
    {
        [Test]
        public void TestRemoveObserverDuringNotification_UntypedDelegate()
        {
            var signal = new IntegerValueSignal(10);
            int invoked1 = 0;
            int invoked2 = 0;
            
            IEmitSignals.SignalChangedDelegate observer1 = null;
            IEmitSignals.SignalChangedDelegate observer2 = (sender) => invoked2++;
            
            observer1 = (sender) => {
                invoked1++;
                signal.RemoveObserver(observer2); // Remove during notification
            };
            
            signal.AddObserver(observer1);
            signal.AddObserver(observer2);
            
            signal.SetValue(20);
            Assert.AreEqual(1, invoked1);
            Assert.AreEqual(1, invoked2, "Observer2 should still be notified this time");
            
            signal.SetValue(30);
            Assert.AreEqual(2, invoked1);
            Assert.AreEqual(1, invoked2, "Observer2 should not be notified after removal");
        }

        [Test]
        public void TestRemoveObserverDuringNotification_ObjectObserver()
        {
            var signal = new IntegerValueSignal(10);
            var observer1 = new TestIntObserver();
            var observer2 = new TestIntObserver();
            
            // Override observer1 to remove observer2 during notification
            bool shouldRemove = true;
            signal.AddObserver((IEmitSignals<int> sender, int old, int newVal) => {
                observer1.Invoked++;
                if (shouldRemove) {
                    signal.RemoveObserver(observer2);
                    shouldRemove = false;
                }
            });
            signal.AddObserver(observer2);
            
            signal.SetValue(20);
            Assert.AreEqual(1, observer2.Invoked, "Observer2 should be notified before removal");
            
            signal.SetValue(30);
            Assert.AreEqual(1, observer2.Invoked, "Observer2 should not be notified after removal");
        }

        [Test]
        public void TestRemoveObserverDuringNotification_DelegateObserver()
        {
            var signal = new IntegerValueSignal(10);
            int invoked1 = 0;
            int invoked2 = 0;
            
            IEmitSignals<int>.SignalChangedHandler observer1 = null;
            IEmitSignals<int>.SignalChangedHandler observer2 = (sender, old, newVal) => invoked2++;
            
            observer1 = (sender, old, newVal) => {
                invoked1++;
                signal.RemoveObserver(observer2);
            };
            
            signal.AddObserver(observer1);
            signal.AddObserver(observer2);
            
            signal.SetValue(20);
            Assert.AreEqual(1, invoked1);
            Assert.AreEqual(1, invoked2, "Should still notify this time");
            
            signal.SetValue(30);
            Assert.AreEqual(2, invoked1);
            Assert.AreEqual(1, invoked2, "Should not notify after removal");
        }

        [Test]
        public void TestRemoveObserverDuringNotification_ActionObserver()
        {
            var signal = new IntegerValueSignal(10);
            int invoked1 = 0;
            int invoked2 = 0;
            
            Action<int> observer1 = null;
            Action<int> observer2 = (value) => invoked2++;
            
            observer1 = (value) => {
                invoked1++;
                signal.RemoveObserver(observer2);
            };
            
            signal.AddObserver(observer1);
            signal.AddObserver(observer2);
            
            signal.SetValue(20);
            Assert.AreEqual(1, invoked1);
            Assert.AreEqual(1, invoked2, "Should still notify this time");
            
            signal.SetValue(30);
            Assert.AreEqual(2, invoked1);
            Assert.AreEqual(1, invoked2, "Should not notify after removal");
        }

        [Test]
        public void TestRemoveSelfDuringNotification()
        {
            var signal = new IntegerValueSignal(10);
            int invoked = 0;
            
            Action<int> observer = null;
            observer = (value) => {
                invoked++;
                signal.RemoveObserver(observer); // Remove self
            };
            
            signal.AddObserver(observer);
            
            signal.SetValue(20);
            Assert.AreEqual(1, invoked);
            
            signal.SetValue(30);
            Assert.AreEqual(1, invoked, "Should not be notified after self-removal");
        }

        [Test]
        public void TestMultipleRemovalsDuringNotification()
        {
            var signal = new IntegerValueSignal(10);
            int invoked1 = 0;
            int invoked2 = 0;
            int invoked3 = 0;
            
            Action<int> observer2 = (value) => invoked2++;
            Action<int> observer3 = (value) => invoked3++;
            
            Action<int> observer1 = (value) => {
                invoked1++;
                signal.RemoveObserver(observer2);
                signal.RemoveObserver(observer3); // Multiple removals
            };
            
            signal.AddObserver(observer1);
            signal.AddObserver(observer2);
            signal.AddObserver(observer3);
            
            signal.SetValue(20);
            Assert.AreEqual(1, invoked1);
            Assert.AreEqual(1, invoked2);
            Assert.AreEqual(1, invoked3);
            
            signal.SetValue(30);
            Assert.AreEqual(2, invoked1);
            Assert.AreEqual(1, invoked2, "Should not notify after removal");
            Assert.AreEqual(1, invoked3, "Should not notify after removal");
        }
    }
}