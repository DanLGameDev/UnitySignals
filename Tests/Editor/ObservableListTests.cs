using System.Collections.Specialized;
using System.Linq;
using DGP.UnitySignals.Collections;
using DGP.UnitySignals.Signals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class ObservableListTests
    {
        [Test]
        public void TestObservableList_CollectionChanged_Events()
        {
            // Arrange
            var list = new ObservableList<int>();
            var addCount = 0;
            var removeCount = 0;
            var replaceCount = 0;
            var resetCount = 0;
            
            list.CollectionChanged += (sender, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        addCount++;
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        removeCount++;
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        replaceCount++;
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        resetCount++;
                        break;
                }
            };
            
            // Act
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list[1] = 4;        // Replace
            list.Remove(3);
            list.Clear();       // Reset
            
            // Assert
            Assert.AreEqual(3, addCount, "Should have 3 add events");
            Assert.AreEqual(1, removeCount, "Should have 1 remove event");
            Assert.AreEqual(1, replaceCount, "Should have 1 replace event");
            Assert.AreEqual(1, resetCount, "Should have 1 reset event");
        }

        [Test]
        public void TestObservableList_SignalObservers()
        {
            // Arrange
            var list = new ObservableList<int>();
            var notificationCount = 0;
            
            list.AddObserver((ObservableList<int> _) => notificationCount++);
            
            // Act
            list.Add(1);
            list.Add(2);
            list[0] = 10;
            list.Remove(2);
            list.Clear();
            
            // Assert
            Assert.AreEqual(5, notificationCount, "Signal observer should be notified for each change");
        }

        [Test]
        public void TestObservableList_ComputedSignal_Sum()
        {
            // Arrange
            var list = new ObservableList<int>();
            var sumSignal = new ComputedSignal<int>(() => list.Value.Sum(x => x));
            
            // Act & Assert - Initial empty
            Assert.AreEqual(0, sumSignal.Value);
            
            list.Add(5);
            Assert.AreEqual(5, sumSignal.Value);
            
            list.Add(10);
            Assert.AreEqual(15, sumSignal.Value);
            
            list.Add(3);
            Assert.AreEqual(18, sumSignal.Value);
            
            list.Remove(5);
            Assert.AreEqual(13, sumSignal.Value);
            
            list.Clear();
            Assert.AreEqual(0, sumSignal.Value);
        }

        [Test]
        public void TestObservableList_ComputedSignal_Count()
        {
            // Arrange
            var list = new ObservableList<string>();
            var countSignal = new ComputedSignal<int>(() => list.Value.Count);
            
            // Act & Assert
            Assert.AreEqual(0, countSignal.Value);
            
            list.Add("hello");
            Assert.AreEqual(1, countSignal.Value);
            
            list.Add("world");
            Assert.AreEqual(2, countSignal.Value);
            
            list.RemoveAt(0);
            Assert.AreEqual(1, countSignal.Value);
            
            list.Clear();
            Assert.AreEqual(0, countSignal.Value);
        }

        [Test]
        public void TestObservableList_ComputedSignal_Average()
        {
            // Arrange
            var list = new ObservableList<double>();
            list.Add(10.0);
            list.Add(20.0);
            
            var averageSignal = new ComputedSignal<double>(() => 
                list.Value.Count > 0 ? list.Value.Average(x => x) : 0.0);
            
            // Act & Assert
            Assert.AreEqual(15.0, averageSignal.Value);
            
            list.Add(30.0);
            Assert.AreEqual(20.0, averageSignal.Value);
            
            list.Remove(10.0);
            Assert.AreEqual(25.0, averageSignal.Value);
        }

        [Test]
        public void TestObservableList_IList_Operations()
        {
            // Arrange
            var list = new ObservableList<string>();
            
            // Act & Assert - Add
            list.Add("first");
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("first", list[0]);
            
            // Insert
            list.Insert(0, "zero");
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("zero", list[0]);
            Assert.AreEqual("first", list[1]);
            
            // Contains
            Assert.IsTrue(list.Contains("zero"));
            Assert.IsFalse(list.Contains("missing"));
            
            // IndexOf
            Assert.AreEqual(0, list.IndexOf("zero"));
            Assert.AreEqual(1, list.IndexOf("first"));
            Assert.AreEqual(-1, list.IndexOf("missing"));
            
            // Remove
            var removed = list.Remove("zero");
            Assert.IsTrue(removed);
            Assert.AreEqual(1, list.Count);
            
            // RemoveAt
            list.RemoveAt(0);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void TestObservableList_Constructor_WithCollection()
        {
            // Arrange
            var initialData = new[] { 1, 2, 3, 4, 5 };
            
            // Act
            var list = new ObservableList<int>(initialData);
            
            // Assert
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(5, list[4]);
        }

        [Test]
        public void TestObservableList_Enumeration()
        {
            // Arrange
            var list = new ObservableList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            
            // Act
            var sum = 0;
            foreach (var item in list)
            {
                sum += item;
            }
            
            // Assert
            Assert.AreEqual(6, sum);
        }

        [Test]
        public void TestObservableList_CopyTo()
        {
            // Arrange
            var list = new ObservableList<int>();
            list.Add(10);
            list.Add(20);
            list.Add(30);
            
            var array = new int[5];
            
            // Act
            list.CopyTo(array, 1);
            
            // Assert
            Assert.AreEqual(0, array[0]);
            Assert.AreEqual(10, array[1]);
            Assert.AreEqual(20, array[2]);
            Assert.AreEqual(30, array[3]);
            Assert.AreEqual(0, array[4]);
        }

        [Test]
        public void TestObservableList_Dispose_UnsubscribesFromEvents()
        {
            // Arrange
            var list = new ObservableList<int>();
            var notificationCount = 0;
            
            list.AddObserver((ObservableList<int> _) => notificationCount++);
            list.Add(1);
            
            Assert.AreEqual(1, notificationCount);
            
            // Act
            list.Dispose();
            
            // Assert
            Assert.IsTrue(list.IsDead);
            
            // Adding after dispose shouldn't notify (though collection still works internally)
            list.Add(2);
            Assert.AreEqual(1, notificationCount, "Should not notify after disposal");
        }

        [Test]
        public void TestObservableList_SignalChanged_Event()
        {
            // Arrange
            var list = new ObservableList<int>();
            var signalChangedCount = 0;
            
            list.SignalChanged += (IEmitSignals _) => signalChangedCount++;
            
            // Act
            list.Add(1);
            list.Add(2);
            list.Remove(1);
            
            // Assert
            Assert.AreEqual(3, signalChangedCount);
        }

        [Test]
        public void TestObservableList_GetValue_ReturnsSelf()
        {
            // Arrange
            var list = new ObservableList<int>();
            list.Add(1);
            list.Add(2);
            
            // Act
            var value = list.GetValue();
            
            // Assert
            Assert.AreSame(list, value, "GetValue should return the list itself");
            Assert.AreEqual(2, value.Count);
        }
    }
}