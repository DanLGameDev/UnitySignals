using DGP.UnitySignals.Signals;
using NUnit.Framework;
using UnityEngine;

namespace DGP.UnitySignals.Editor.Tests
{
    public class ObjectReferenceSignalTests
    {
        [Test]
        public void TestGameObjectSignalConstruction()
        {
            var signal = new GameObjectSignal();
            Assert.IsNotNull(signal);
            Assert.IsNull(signal.GetValue());
            
            var go = new GameObject("Test");
            signal = new GameObjectSignal(go);
            Assert.IsNotNull(signal);
            Assert.AreEqual(go, signal.GetValue());
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TestGameObjectSignalSetValue()
        {
            var signal = new GameObjectSignal();
            var go = new GameObject("Test");
            
            signal.SetValue(go);
            Assert.AreEqual(go, signal.GetValue());
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TestGameObjectSignalEventHandling()
        {
            int invoked = 0;
            var signal = new GameObjectSignal();
            var go = new GameObject("Test");
            
            signal.SignalChanged += (sender) => invoked++;
            signal.SetValue(go);
            
            Assert.AreEqual(1, invoked);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TestGameObjectSignalActionObserver()
        {
            int invoked = 0;
            GameObject lastValue = null;
            var signal = new GameObjectSignal();
            var go = new GameObject("Test");
            
            signal.AddObserver((GameObject value) => {
                invoked++;
                lastValue = value;
            });
            
            signal.SetValue(go);
            
            Assert.AreEqual(1, invoked);
            Assert.AreEqual(go, lastValue);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TestGameObjectSignalNoInvokeOnSameValue()
        {
            int invoked = 0;
            var signal = new GameObjectSignal();
            var go = new GameObject("Test");
            
            signal.AddObserver((GameObject value) => invoked++);
            
            signal.SetValue(go);
            signal.SetValue(go); // Should not invoke again
            
            Assert.AreEqual(1, invoked);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TestGameObjectSignalSetToNull()
        {
            int invoked = 0;
            GameObject lastValue = null;
            var signal = new GameObjectSignal();
            var go = new GameObject("Test");
            
            signal.SetValue(go);
            signal.AddObserver((GameObject value) => {
                invoked++;
                lastValue = value;
            });
            
            signal.SetValue(null);
            
            Assert.AreEqual(1, invoked);
            Assert.IsNull(lastValue);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TestTransformSignal()
        {
            int invoked = 0;
            var signal = new TransformSignal();
            var go = new GameObject("Test");
            var transform = go.transform;
            
            signal.AddObserver((Transform value) => invoked++);
            signal.SetValue(transform);
            
            Assert.AreEqual(1, invoked);
            Assert.AreEqual(transform, signal.GetValue());
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TestObjectReferenceSignalImplicitConversion()
        {
            var go = new GameObject("Test");
            var signal = new GameObjectSignal(go);
            
            GameObject implicitValue = signal;
            Assert.AreEqual(go, implicitValue);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TestObjectReferenceSignalRemoveObserver()
        {
            int invoked = 0;
            var signal = new GameObjectSignal();
            var go = new GameObject("Test");
            
            void Observer(GameObject value) => invoked++;
            
            signal.AddObserver(Observer);
            signal.SetValue(go);
            Assert.AreEqual(1, invoked);
            
            signal.RemoveObserver(Observer);
            signal.SetValue(null);
            Assert.AreEqual(1, invoked); // Should still be 1
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TestObjectReferenceSignalClearObservers()
        {
            int invoked = 0;
            var signal = new GameObjectSignal();
            var go = new GameObject("Test");
            
            signal.AddObserver((GameObject value) => invoked++);
            signal.AddObserver((IEmitSignals sender) => invoked++);
            
            signal.SetValue(go);
            Assert.AreEqual(2, invoked);
            
            signal.ClearObservers();
            signal.SetValue(null);
            Assert.AreEqual(2, invoked); // Should still be 2
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void TestObjectReferenceSignalDispose()
        {
            int invoked = 0;
            var signal = new GameObjectSignal();
            var go = new GameObject("Test");
            
            signal.AddObserver((GameObject value) => invoked++);
            signal.SetValue(go);
            Assert.AreEqual(1, invoked);
            
            signal.Dispose();
            signal.SetValue(null);
            Assert.AreEqual(1, invoked); // Should still be 1
            
            Object.DestroyImmediate(go);
        }
    }
}