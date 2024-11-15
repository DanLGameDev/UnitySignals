using DGP.UnitySignals.Signals;
using NUnit.Framework;
using UnityEngine;

namespace DGP.UnitySignals.Editor.Tests
{
    public class BasicDataTypeTests
    {
        [Test]
        public void TestBooleanValueSignal()
        {
            int invokeCount = 0;
            
            BooleanValueSignal signal = new BooleanValueSignal();
            Assert.IsFalse(signal.GetValue());
            
            signal.AddObserver((sender, oldValue, newValue) => invokeCount++);
            
            signal.SetValue(true);
            
            Assert.IsTrue(signal.GetValue());
            Assert.AreEqual(1, invokeCount);
        }
        
        [Test]
        public void TestByteValueSignal()
        {
            int invokeCount = 0;
            
            ByteValueSignal signal = new ByteValueSignal();
            Assert.AreEqual(0, signal.GetValue());
            
            signal.AddObserver((sender, oldValue, newValue) => invokeCount++);

            signal.SetValue(1);
            
            Assert.AreEqual(1, signal.GetValue());
            Assert.AreEqual(1, invokeCount);
        }
        
        [Test]
        public void TestCharValueSignal()
        {
            int invokeCount = 0;
            
            CharValueSignal signal = new CharValueSignal();
            Assert.AreEqual('\0', signal.GetValue());
            
            signal.AddObserver((sender, oldValue, newValue) => invokeCount++);

            signal.SetValue('a');
            
            Assert.AreEqual('a', signal.GetValue());
            Assert.AreEqual(1, invokeCount);
        }
        
        [Test]
        public void TestDecimalValueSignal()
        {
            int invokeCount = 0;
            
            DecimalValueSignal signal = new DecimalValueSignal();
            Assert.AreEqual(0m, signal.GetValue());
            
            signal.AddObserver((sender, oldValue, newValue) => invokeCount++);

            signal.SetValue(1m);
            
            Assert.AreEqual(1m, signal.GetValue());
            Assert.AreEqual(1, invokeCount);
        }
        
        [Test]
        public void TestDoubleValueSignal()
        {
            int invokeCount = 0;
            
            DoubleValueSignal signal = new DoubleValueSignal();
            Assert.AreEqual(0d, signal.GetValue());
            
            signal.AddObserver((sender, oldValue, newValue) => invokeCount++);

            signal.SetValue(1d);
            
            Assert.AreEqual(1d, signal.GetValue());
            Assert.AreEqual(1, invokeCount);
        }
        
        [Test]
        public void TestFloatValueSignal()
        {
            int invokeCount = 0;
            
            FloatValueSignal signal = new FloatValueSignal();
            Assert.AreEqual(0f, signal.GetValue());
            
            signal.AddObserver((sender, oldValue, newValue) => invokeCount++);

            signal.SetValue(1f);
            
            Assert.AreEqual(1f, signal.GetValue());
            Assert.AreEqual(1, invokeCount);
        }
        
        [Test]
        public void TestIntValueSignal()
        {
            int invokeCount = 0;
            
            IntegerValueSignal signal = new IntegerValueSignal();
            Assert.AreEqual(0, signal.GetValue());
            
            signal.AddObserver((sender, oldValue, newValue) => invokeCount++);

            signal.SetValue(1);
            
            Assert.AreEqual(1, signal.GetValue());
            Assert.AreEqual(1, invokeCount);
        }
        
        [Test]
        public void TestLongValueSignal()
        {
            int invokeCount = 0;
            
            LongValueSignal signal = new LongValueSignal();
            Assert.AreEqual(0L, signal.GetValue());
            
            signal.AddObserver((sender, oldValue, newValue) => invokeCount++);

            signal.SetValue(1L);
            
            Assert.AreEqual(1L, signal.GetValue());
            Assert.AreEqual(1, invokeCount);
        }
        
        [Test]
        public void TestStringValueSignal()
        {
            int invokeCount = 0;
            
            StringValueSignal signal = new StringValueSignal();
            Assert.AreEqual(string.Empty, signal.GetValue());
            
            signal.AddObserver((sender, oldValue, newValue) => invokeCount++);

            signal.SetValue("Hello, World!");
            
            Assert.AreEqual("Hello, World!", signal.GetValue());
            Assert.AreEqual(1, invokeCount);
        }
        
        [Test]
        public void TestVector2ValueSignal()
        {
            int invokeCount = 0;
            
            Vector2ValueSignal signal = new Vector2ValueSignal();
            Assert.AreEqual(Vector2.zero, signal.GetValue());
            
            signal.AddObserver((sender, oldValue, newValue) => invokeCount++);

            signal.SetValue(Vector2.one);
            
            Assert.AreEqual(Vector2.one, signal.GetValue());
            Assert.AreEqual(1, invokeCount);
        }
        
        [Test]
        public void TestVector3ValueSignal()
        {
            int invokeCount = 0;
            
            Vector3ValueSignal signal = new Vector3ValueSignal();
            Assert.AreEqual(Vector3.zero, signal.GetValue());
            
            signal.AddObserver((sender, oldValue, newValue) => invokeCount++);

            signal.SetValue(Vector3.one);
            
            Assert.AreEqual(Vector3.one, signal.GetValue());
            Assert.AreEqual(1, invokeCount);
        }
        
    }
}