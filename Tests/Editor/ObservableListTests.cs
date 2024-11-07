using DGP.UnitySignals.Collections;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class ObservableListTests
    {
        [Test]
        public void TestObservableList()
        {
            // Arrange
            var list = new ObservableList<int>();
            var added = 0;
            var removed = 0;
            var changed = 0;
            var cleared = 0;
            list.ItemAdded += (sender, e) => added++;
            list.ItemRemoved += (sender, e) => removed++;
            list.ItemChanged += (sender, e) => changed++;
            list.Cleared += (sender, e) => cleared++;
            
            // Act
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list[1] = 4;
            list.Remove(3);
            list.Clear();
            
            // Assert
            Assert.AreEqual(3, added);
            Assert.AreEqual(1, removed);
            Assert.AreEqual(1, changed);
            Assert.AreEqual(1, cleared);
        }
    }
}