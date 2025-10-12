using System;
using System.ComponentModel;
using DGP.UnitySignals.Signals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class PropertyNotifySignalTests
    {
        private class TestNotifyObject : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private int _value;
            public int Value
            {
                get => _value;
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    }
                }
            }

            private string _name;
            public string Name
            {
                get => _name;
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                    }
                }
            }

            public bool Equals(TestNotifyObject other)
            {
                if (other == null) return false;
                return _value == other._value && _name == other._name;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as TestNotifyObject);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_value, _name);
            }
        }

        [Test]
        public void TestPropertyNotifySignalConstruction()
        {
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj);
            
            Assert.IsNotNull(signal);
            Assert.AreEqual(obj, signal.GetValue());
        }

        [Test]
        public void TestPropertyNotifySignalSetValue()
        {
            var signal = new PropertyNotifySignal<TestNotifyObject>();
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            
            signal.SetValue(obj);
            Assert.AreEqual(obj, signal.GetValue());
        }

        [Test]
        public void TestPropertyChangedTriggersObservers()
        {
            int invoked = 0;
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj);
            
            signal.AddObserver((TestNotifyObject newValue) => invoked++);
            
            // Change a property on the object
            obj.Value = 20;
            
            Assert.AreEqual(1, invoked);
        }

        [Test]
        public void TestMultiplePropertyChangesTriggersMultipleTimes()
        {
            int invoked = 0;
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj);
            
            signal.AddObserver((TestNotifyObject newValue) => invoked++);
            
            obj.Value = 20;
            obj.Name = "Changed";
            obj.Value = 30;
            
            Assert.AreEqual(3, invoked);
        }

        [Test]
        public void TestSetValueUnsubscribesFromOldObject()
        {
            int invoked = 0;
            var obj1 = new TestNotifyObject { Value = 10, Name = "First" };
            var obj2 = new TestNotifyObject { Value = 20, Name = "Second" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj1);
            
            signal.AddObserver((TestNotifyObject newValue) => invoked++);
            
            // Switch to new object
            signal.SetValue(obj2);
            Assert.AreEqual(1, invoked); // From SetValue
            
            // Change old object - should NOT trigger
            obj1.Value = 99;
            Assert.AreEqual(1, invoked); // Still 1
            
            // Change new object - should trigger
            obj2.Value = 25;
            Assert.AreEqual(2, invoked);
        }

        [Test]
        public void TestSetValueToNullUnsubscribes()
        {
            int invoked = 0;
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj);
            
            signal.AddObserver((TestNotifyObject newValue) => invoked++);
            
            signal.SetValue(null);
            Assert.AreEqual(1, invoked); // From SetValue
            
            // Change old object - should NOT trigger
            obj.Value = 99;
            Assert.AreEqual(1, invoked); // Still 1
        }

        [Test]
        public void TestDisposeUnsubscribesFromObject()
        {
            int invoked = 0;
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj);
            
            signal.AddObserver((TestNotifyObject newValue) => invoked++);
            
            signal.Dispose();
            
            // Change object - should NOT trigger
            obj.Value = 99;
            Assert.AreEqual(0, invoked);
        }

        [Test]
        public void TestSignalValueChangedEvent()
        {
            int eventInvoked = 0;
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj);
            
            signal.SignalValueChanged += (sender, oldValue, newValue) => eventInvoked++;
            
            obj.Value = 20;
            
            Assert.AreEqual(1, eventInvoked);
        }

        [Test]
        public void TestDelegateObserver()
        {
            int invoked = 0;
            TestNotifyObject capturedOld = null;
            TestNotifyObject capturedNew = null;
            
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj);
            
            signal.AddObserver((IEmitSignals<TestNotifyObject> sender, TestNotifyObject oldValue, TestNotifyObject newValue) => {
                invoked++;
                capturedOld = oldValue;
                capturedNew = newValue;
            });
            
            obj.Value = 20;
            
            Assert.AreEqual(1, invoked);
            Assert.AreEqual(obj, capturedOld); // Same object reference
            Assert.AreEqual(obj, capturedNew); // Same object reference
        }

        [Test]
        public void TestClearObservers()
        {
            int invoked = 0;
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj);
            
            signal.AddObserver((TestNotifyObject newValue) => invoked++);
            
            obj.Value = 20;
            Assert.AreEqual(1, invoked);
            
            signal.ClearObservers();
            
            obj.Value = 30;
            Assert.AreEqual(1, invoked); // Still 1, observer was cleared
        }

        [Test]
        public void TestPropertyChangeWithNoObservers()
        {
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj);
            
            // Should not throw
            Assert.DoesNotThrow(() => obj.Value = 20);
        }
        
        [Test]
        public void TestPropertyNotifySignalNoNotificationOnSameReference()
        {
            int invoked = 0;
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj);
    
            signal.AddObserver((TestNotifyObject newValue) => invoked++);
    
            signal.SetValue(obj); // Same object reference
            Assert.AreEqual(0, invoked); // Should NOT invoke
    
            var obj2 = new TestNotifyObject { Value = 10, Name = "Test" };
            signal.SetValue(obj2); // Different reference
            Assert.AreEqual(1, invoked); // Should invoke
        }
        
        [Test]
        public void TestValueProperty()
        {
            var obj = new TestNotifyObject { Value = 10, Name = "Test" };
            var signal = new PropertyNotifySignal<TestNotifyObject>(obj);
            Assert.AreEqual(obj, signal.Value);
    
            int invoked = 0;
            signal.AddObserver((TestNotifyObject newValue) => invoked++);
    
            var obj2 = new TestNotifyObject { Value = 20, Name = "Test2" };
            signal.Value = obj2; // Using property instead of SetValue
            Assert.AreEqual(obj2, signal.Value);
            Assert.AreEqual(1, invoked);
        }
        
    }
}