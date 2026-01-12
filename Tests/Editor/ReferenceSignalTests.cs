using DGP.UnitySignals.Observers;
using DGP.UnitySignals.Signals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class ReferenceSignalTests
    {
        private interface ITestInterface
        {
            int Value { get; set; }
        }

        private class TestClass : ITestInterface
        {
            public int Value { get; set; }
        }

        [Test]
        public void TestReferenceSignalConstruction()
        {
            var signal = new ReferenceSignal<ITestInterface>();
            Assert.IsNotNull(signal);
            Assert.IsNull(signal.GetValue());

            var obj = new TestClass { Value = 10 };
            signal = new ReferenceSignal<ITestInterface>(obj);
            Assert.IsNotNull(signal);
            Assert.AreEqual(obj, signal.GetValue());
        }

        [Test]
        public void TestReferenceSignalSetValue()
        {
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };

            signal.SetValue(obj);
            Assert.AreEqual(obj, signal.GetValue());
        }

        [Test]
        public void TestReferenceSignalEventHandling()
        {
            int invoked = 0;
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };

            signal.SignalChanged += (sender) => invoked++;
            signal.SetValue(obj);

            Assert.AreEqual(1, invoked);
        }

        [Test]
        public void TestReferenceSignalActionObserver()
        {
            int invoked = 0;
            ITestInterface lastValue = null;
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };

            signal.AddObserver((ITestInterface value) => {
                invoked++;
                lastValue = value;
            });

            signal.SetValue(obj);

            Assert.AreEqual(1, invoked);
            Assert.AreEqual(obj, lastValue);
        }

        [Test]
        public void TestReferenceSignalNoInvokeOnSameReference()
        {
            int invoked = 0;
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };

            signal.AddObserver((ITestInterface value) => invoked++);

            signal.SetValue(obj);
            signal.SetValue(obj); // Same reference - should not invoke again

            Assert.AreEqual(1, invoked);
        }

        [Test]
        public void TestReferenceSignalDifferentObjectsSameValues()
        {
            int invoked = 0;
            var signal = new ReferenceSignal<ITestInterface>();
            var obj1 = new TestClass { Value = 10 };
            var obj2 = new TestClass { Value = 10 }; // Same values, different object

            signal.AddObserver((ITestInterface value) => invoked++);

            signal.SetValue(obj1);
            Assert.AreEqual(1, invoked);

            signal.SetValue(obj2); // Different reference - should invoke
            Assert.AreEqual(2, invoked);
        }

        [Test]
        public void TestReferenceSignalSetToNull()
        {
            int invoked = 0;
            ITestInterface lastValue = null;
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };

            signal.SetValue(obj);
            signal.AddObserver((ITestInterface value) => {
                invoked++;
                lastValue = value;
            });

            signal.SetValue(null);

            Assert.AreEqual(1, invoked);
            Assert.IsNull(lastValue);
        }

        [Test]
        public void TestReferenceSignalImplicitConversion()
        {
            var obj = new TestClass { Value = 10 };
            var signal = new ReferenceSignal<TestClass>(obj); // Use concrete type

            TestClass implicitValue = signal; // Now it works
            Assert.AreEqual(obj, implicitValue);
        }

        [Test]
        public void TestReferenceSignalRemoveObserver()
        {
            int invoked = 0;
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };

            void Observer(ITestInterface value) => invoked++;

            signal.AddObserver(Observer);
            signal.SetValue(obj);
            Assert.AreEqual(1, invoked);

            signal.RemoveObserver(Observer);
            signal.SetValue(null);
            Assert.AreEqual(1, invoked); // Should still be 1
        }

        [Test]
        public void TestReferenceSignalClearObservers()
        {
            int invoked = 0;
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };

            signal.AddObserver((ITestInterface value) => invoked++);
            signal.AddObserver((IEmitSignals sender) => invoked++);

            signal.SetValue(obj);
            Assert.AreEqual(2, invoked);

            signal.ClearObservers();
            signal.SetValue(null);
            Assert.AreEqual(2, invoked); // Should still be 2
        }

        [Test]
        public void TestReferenceSignalDispose()
        {
            int invoked = 0;
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };

            signal.AddObserver((ITestInterface value) => invoked++);
            signal.SetValue(obj);
            Assert.AreEqual(1, invoked);

            signal.Dispose();
            signal.SetValue(null);
            Assert.AreEqual(1, invoked); // Should still be 1
        }

        [Test]
        public void TestReferenceSignalWithInterface()
        {
            // This is the key test - ensuring interfaces work!
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 42 };

            signal.SetValue(obj);

            Assert.AreEqual(42, signal.GetValue().Value);
        }

        [Test]
        public void TestReferenceSignalValueChangedEvent()
        {
            int invoked = 0;
            ITestInterface capturedOld = null;
            ITestInterface capturedNew = null;

            var signal = new ReferenceSignal<ITestInterface>();
            var obj1 = new TestClass { Value = 10 };
            var obj2 = new TestClass { Value = 20 };

            signal.SignalValueChanged += (sender, oldValue, newValue) => {
                invoked++;
                capturedOld = oldValue;
                capturedNew = newValue;
            };

            signal.SetValue(obj1);

            Assert.AreEqual(1, invoked);
            Assert.IsNull(capturedOld);
            Assert.AreEqual(obj1, capturedNew);

            signal.SetValue(obj2);

            Assert.AreEqual(2, invoked);
            Assert.AreEqual(obj1, capturedOld);
            Assert.AreEqual(obj2, capturedNew);
        }

        [Test]
        public void TestReferenceSignalDelegateObserver()
        {
            int invoked = 0;
            ITestInterface capturedOld = null;
            ITestInterface capturedNew = null;

            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };

            signal.AddObserver((IEmitSignals<ITestInterface> sender, ITestInterface oldValue, ITestInterface newValue) => {
                invoked++;
                capturedOld = oldValue;
                capturedNew = newValue;
            });

            signal.SetValue(obj);

            Assert.AreEqual(1, invoked);
            Assert.IsNull(capturedOld);
            Assert.AreEqual(obj, capturedNew);
        }

        [Test]
        public void TestReferenceSignalObjectObserver()
        {
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };
            var observer = new TestReferenceObserver();

            signal.AddObserver(observer);
            signal.SetValue(obj);

            Assert.AreEqual(1, observer.Invoked);
            Assert.IsNull(observer.CapturedOld);
            Assert.AreEqual(obj, observer.CapturedNew);
        }

        [Test]
        public void TestReferenceSignalRemoveDelegateObserver()
        {
            int invoked = 0;
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };

            void Observer(IEmitSignals<ITestInterface> sender, ITestInterface oldValue, ITestInterface newValue) => invoked++;

            signal.AddObserver(Observer);
            signal.SetValue(obj);
            Assert.AreEqual(1, invoked);

            signal.RemoveObserver(Observer);
            signal.SetValue(null);
            Assert.AreEqual(1, invoked); // Should still be 1
        }

        [Test]
        public void TestReferenceSignalRemoveObjectObserver()
        {
            var signal = new ReferenceSignal<ITestInterface>();
            var obj = new TestClass { Value = 10 };
            var observer = new TestReferenceObserver();

            signal.AddObserver(observer);
            signal.SetValue(obj);
            Assert.AreEqual(1, observer.Invoked);

            signal.RemoveObserver(observer);
            signal.SetValue(null);
            Assert.AreEqual(1, observer.Invoked); // Should still be 1
        }

        [Test]
        public void TestReferenceSignalValueProperty()
        {
            var obj1 = new TestClass { Value = 10 };
            var obj2 = new TestClass { Value = 20 };
            var signal = new ReferenceSignal<ITestInterface>(obj1);
    
            Assert.AreEqual(obj1, signal.Value);

            int invoked = 0;
            signal.AddObserver((ITestInterface value) => invoked++);

            signal.Value = obj2;
            Assert.AreEqual(obj2, signal.Value);
            Assert.AreEqual(1, invoked);
        }
        
        [Test]
        public void TestReferenceSignalSetValueSilently_SameReference_ReturnsFalse()
        {
            var obj = new TestClass { Value = 10 };
            var signal = new ReferenceSignal<ITestInterface>(obj);
    
            bool changed = signal.SetValueSilently(obj); // Same reference
    
            Assert.IsFalse(changed);
        }

        [Test]
        public void TestReferenceSignalSetValueSilently_DifferentReference_ReturnsTrue()
        {
            var obj1 = new TestClass { Value = 10 };
            var obj2 = new TestClass { Value = 20 };
            var signal = new ReferenceSignal<ITestInterface>(obj1);
    
            bool changed = signal.SetValueSilently(obj2); // Different reference
    
            Assert.IsTrue(changed);
        }
        

        private class TestReferenceObserver : ISignalObserver<ITestInterface>
        {
            public int Invoked = 0;
            public ITestInterface CapturedOld = null;
            public ITestInterface CapturedNew = null;

            public void SignalValueChanged(IEmitSignals<ITestInterface> emitter, ITestInterface oldValue, ITestInterface newValue)
            {
                Invoked++;
                CapturedOld = oldValue;
                CapturedNew = newValue;
            }
        }
    }
}