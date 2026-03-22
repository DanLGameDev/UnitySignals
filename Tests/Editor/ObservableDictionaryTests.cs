using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DGP.UnitySignals.Collections;
using DGP.UnitySignals.Signals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class ObservableDictionaryTests
    {
        [Test]
        public void TestObservableDictionary_CollectionChanged_Events()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            var addCount = 0;
            var removeCount = 0;
            var replaceCount = 0;
            var resetCount = 0;

            dict.CollectionChanged += (sender, e) =>
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
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict.Add("c", 3);
            dict["b"] = 99;     // Replace
            dict.Remove("c");
            dict.Clear();       // Reset

            // Assert
            Assert.AreEqual(3, addCount, "Should have 3 add events");
            Assert.AreEqual(1, removeCount, "Should have 1 remove event");
            Assert.AreEqual(1, replaceCount, "Should have 1 replace event");
            Assert.AreEqual(1, resetCount, "Should have 1 reset event");
        }

        [Test]
        public void TestObservableDictionary_ItemAdded_Event()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            var addedItems = new List<(string key, int value)>();

            dict.ItemAdded += (key, value) => addedItems.Add((key, value));

            // Act
            dict.Add("x", 10);
            dict.Add("y", 20);
            dict["z"] = 30;     // Add via indexer (new key)

            // Assert
            Assert.AreEqual(3, addedItems.Count);
            Assert.AreEqual(("x", 10), addedItems[0]);
            Assert.AreEqual(("y", 20), addedItems[1]);
            Assert.AreEqual(("z", 30), addedItems[2]);
        }

        [Test]
        public void TestObservableDictionary_ItemRemoved_Event()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict.Add("c", 3);

            var removedItems = new List<(string key, int value)>();
            dict.ItemRemoved += (key, value) => removedItems.Add((key, value));

            // Act
            dict.Remove("b");
            dict.Remove("a");

            // Assert
            Assert.AreEqual(2, removedItems.Count);
            Assert.AreEqual(("b", 2), removedItems[0]);
            Assert.AreEqual(("a", 1), removedItems[1]);
        }

        [Test]
        public void TestObservableDictionary_ItemReplaced_Event()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            dict.Add("x", 10);
            dict.Add("y", 20);

            var replacedItems = new List<(string key, int oldValue, int newValue)>();
            dict.ItemReplaced += (key, oldValue, newValue) => replacedItems.Add((key, oldValue, newValue));

            // Act
            dict["x"] = 100;
            dict["y"] = 200;

            // Assert
            Assert.AreEqual(2, replacedItems.Count);
            Assert.AreEqual(("x", 10, 100), replacedItems[0]);
            Assert.AreEqual(("y", 20, 200), replacedItems[1]);
        }

        [Test]
        public void TestObservableDictionary_Cleared_Event()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);

            int clearedCount = 0;
            dict.Cleared += () => clearedCount++;

            // Act
            dict.Clear();

            // Assert
            Assert.AreEqual(1, clearedCount, "Cleared event should fire once");
            Assert.AreEqual(0, dict.Count, "Dictionary should be empty");
        }

        [Test]
        public void TestObservableDictionary_Clear_DoesNotFireItemRemoved()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);

            int itemRemovedCount = 0;
            dict.ItemRemoved += (key, value) => itemRemovedCount++;

            // Act
            dict.Clear();

            // Assert
            Assert.AreEqual(0, itemRemovedCount, "ItemRemoved should not fire during Clear");
        }

        [Test]
        public void TestObservableDictionary_Indexer_AddVsReplace()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            int addedCount = 0;
            int replacedCount = 0;

            dict.ItemAdded += (key, value) => addedCount++;
            dict.ItemReplaced += (key, oldValue, newValue) => replacedCount++;

            // Act
            dict["newKey"] = 1;     // Add (key doesn't exist)
            dict["newKey"] = 2;     // Replace (key already exists)

            // Assert
            Assert.AreEqual(1, addedCount, "Should fire ItemAdded for new key");
            Assert.AreEqual(1, replacedCount, "Should fire ItemReplaced for existing key");
            Assert.AreEqual(2, dict["newKey"]);
        }

        [Test]
        public void TestObservableDictionary_SignalObservers()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            var notificationCount = 0;

            dict.AddObserver((IReadOnlyDictionary<string, int> _) => notificationCount++);

            // Act
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict["a"] = 10;
            dict.Remove("b");
            dict.Clear();

            // Assert
            Assert.AreEqual(5, notificationCount, "Signal observer should be notified for each change");
        }

        [Test]
        public void TestObservableDictionary_ComputedSignal_Sum()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            var sumSignal = new ComputedSignal<int>(() => dict.Value.Values.Sum());

            // Act & Assert - initial empty
            Assert.AreEqual(0, sumSignal.Value);

            dict.Add("a", 5);
            Assert.AreEqual(5, sumSignal.Value);

            dict.Add("b", 10);
            Assert.AreEqual(15, sumSignal.Value);

            dict["a"] = 20;
            Assert.AreEqual(30, sumSignal.Value);

            dict.Remove("b");
            Assert.AreEqual(20, sumSignal.Value);

            dict.Clear();
            Assert.AreEqual(0, sumSignal.Value);
        }

        [Test]
        public void TestObservableDictionary_ComputedSignal_Count()
        {
            // Arrange
            var dict = new ObservableDictionary<string, string>();
            var countSignal = new ComputedSignal<int>(() => dict.Value.Count);

            // Act & Assert
            Assert.AreEqual(0, countSignal.Value);

            dict.Add("k1", "v1");
            Assert.AreEqual(1, countSignal.Value);

            dict.Add("k2", "v2");
            Assert.AreEqual(2, countSignal.Value);

            dict.Remove("k1");
            Assert.AreEqual(1, countSignal.Value);

            dict.Clear();
            Assert.AreEqual(0, countSignal.Value);
        }

        [Test]
        public void TestObservableDictionary_IDictionary_Operations()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();

            // Add
            dict.Add("a", 1);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(1, dict["a"]);

            // ContainsKey
            Assert.IsTrue(dict.ContainsKey("a"));
            Assert.IsFalse(dict.ContainsKey("missing"));

            // TryGetValue
            Assert.IsTrue(dict.TryGetValue("a", out var val));
            Assert.AreEqual(1, val);
            Assert.IsFalse(dict.TryGetValue("missing", out _));

            // Remove by key
            var removed = dict.Remove("a");
            Assert.IsTrue(removed);
            Assert.AreEqual(0, dict.Count);

            // Remove missing key
            var removedMissing = dict.Remove("nope");
            Assert.IsFalse(removedMissing);
        }

        [Test]
        public void TestObservableDictionary_Constructor_WithDictionary()
        {
            // Arrange
            var initial = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };

            // Act
            var dict = new ObservableDictionary<string, int>(initial);

            // Assert
            Assert.AreEqual(3, dict.Count);
            Assert.AreEqual(1, dict["a"]);
            Assert.AreEqual(2, dict["b"]);
            Assert.AreEqual(3, dict["c"]);
        }

        [Test]
        public void TestObservableDictionary_Enumeration()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict.Add("c", 3);

            // Act
            var sum = 0;
            foreach (var kvp in dict)
                sum += kvp.Value;

            // Assert
            Assert.AreEqual(6, sum);
        }

        [Test]
        public void TestObservableDictionary_Keys_And_Values()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            dict.Add("x", 10);
            dict.Add("y", 20);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "x", "y" }, dict.Keys);
            CollectionAssert.AreEquivalent(new[] { 10, 20 }, dict.Values);
        }

        [Test]
        public void TestObservableDictionary_Contains_KeyValuePair()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            dict.Add("a", 1);

            // Assert
            Assert.IsTrue(dict.Contains(new KeyValuePair<string, int>("a", 1)));
            Assert.IsFalse(dict.Contains(new KeyValuePair<string, int>("a", 99)));
            Assert.IsFalse(dict.Contains(new KeyValuePair<string, int>("b", 1)));
        }

        [Test]
        public void TestObservableDictionary_CopyTo()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);

            var array = new KeyValuePair<string, int>[3];

            // Act
            dict.CopyTo(array, 1);

            // Assert
            Assert.AreEqual(default(KeyValuePair<string, int>), array[0]);
            Assert.AreEqual(2, array.Skip(1).Count(kvp => kvp.Key != null));
        }

        [Test]
        public void TestObservableDictionary_Dispose_ClearsObservers()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            var notificationCount = 0;

            dict.AddObserver((IReadOnlyDictionary<string, int> _) => notificationCount++);
            dict.Add("a", 1);

            Assert.AreEqual(1, notificationCount);

            // Act
            dict.Dispose();

            // Assert
            Assert.IsTrue(dict.IsDead);

            dict.Add("b", 2);
            Assert.AreEqual(1, notificationCount, "Should not notify after disposal");
        }

        [Test]
        public void TestObservableDictionary_SignalChanged_Event()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            var signalChangedCount = 0;

            dict.SignalChanged += (IEmitSignals _) => signalChangedCount++;

            // Act
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict.Remove("a");

            // Assert
            Assert.AreEqual(3, signalChangedCount);
        }

        [Test]
        public void TestObservableDictionary_GetValue_ReturnsSelf()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            dict.Add("a", 1);

            // Act
            var value = dict.GetValue();

            // Assert
            Assert.AreSame(dict, value, "GetValue should return the dictionary itself");
            Assert.AreEqual(1, value.Count);
        }

        [Test]
        public void TestObservableDictionary_AllEventsFireTogether()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();

            int itemAddedFired = 0;
            int itemRemovedFired = 0;
            int itemReplacedFired = 0;
            int clearedFired = 0;
            int collectionChangedFired = 0;

            dict.ItemAdded += (key, value) => itemAddedFired++;
            dict.ItemRemoved += (key, value) => itemRemovedFired++;
            dict.ItemReplaced += (key, oldValue, newValue) => itemReplacedFired++;
            dict.Cleared += () => clearedFired++;
            dict.CollectionChanged += (sender, e) => collectionChangedFired++;

            // Act
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict["a"] = 10;
            dict.Remove("b");
            dict.Clear();

            // Assert
            Assert.AreEqual(2, itemAddedFired, "ItemAdded should fire for Add operations");
            Assert.AreEqual(1, itemRemovedFired, "ItemRemoved should fire for Remove operations");
            Assert.AreEqual(1, itemReplacedFired, "ItemReplaced should fire for indexer set on existing key");
            Assert.AreEqual(1, clearedFired, "Cleared should fire for Clear operation");
            Assert.AreEqual(5, collectionChangedFired, "CollectionChanged should fire for all operations");
        }

        [Test]
        public void TestObservableDictionary_Remove_KeyValuePair()
        {
            // Arrange
            var dict = new ObservableDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);

            int removedCount = 0;
            dict.ItemRemoved += (key, value) => removedCount++;

            // Act - Remove via KeyValuePair (ICollection<KVP> interface)
            dict.Remove(new KeyValuePair<string, int>("a", 1));

            // Assert
            Assert.AreEqual(1, removedCount);
            Assert.IsFalse(dict.ContainsKey("a"));
            Assert.AreEqual(1, dict.Count);
        }
    }
}