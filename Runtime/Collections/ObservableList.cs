using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DGP.UnitySignals.Collections
{
    /// <summary>
    /// Observable collection that emits signals when items are added, removed, replaced, or cleared.
    /// </summary>
    /// <remarks>
    /// This collection implements SignalBase with IReadOnlyList interface for API consistency and encapsulation.
    /// 
    /// IMPORTANT NOTIFICATION BEHAVIOR:
    /// Since this is a mutable collection, the oldValue and newValue parameters in SignalValueChanged
    /// will reference the same object (the current list state). For detailed change information,
    /// subscribe to the collection-specific events: ItemAdded, ItemRemoved, ItemReplaced, or Cleared.
    /// 
    /// Usage patterns:
    /// - For "react to any change": Use AddObserver with Action&lt;IReadOnlyList&lt;T&gt;&gt;
    /// - For "track specific changes": Use ItemAdded, ItemRemoved, ItemReplaced, Cleared events
    /// - For computed signals: Works seamlessly - will recalculate on any collection modification
    /// </remarks>
    /// <typeparam name="TValueType">The type of elements in the list</typeparam>
    public class ObservableList<TValueType> : SignalBase<IReadOnlyList<TValueType>>, IList<TValueType>, INotifyCollectionChanged, IReadOnlyList<TValueType>
    {
        /// <summary>
        /// Raised when the collection changes in any way (add, remove, replace, clear).
        /// Provides detailed NotifyCollectionChangedEventArgs for WPF/UI binding scenarios.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        
        /// <summary>
        /// Raised when an item is added to the collection.
        /// </summary>
        /// <param name="item">The item that was added</param>
        /// <param name="index">The index where the item was inserted</param>
        public event Action<TValueType, int> ItemAdded;
        
        /// <summary>
        /// Raised when an item is removed from the collection.
        /// </summary>
        /// <param name="item">The item that was removed</param>
        /// <param name="index">The index where the item was located</param>
        public event Action<TValueType, int> ItemRemoved;
        
        /// <summary>
        /// Raised when an item at a specific index is replaced with a different item.
        /// </summary>
        /// <param name="oldItem">The item that was replaced</param>
        /// <param name="newItem">The new item at that index</param>
        /// <param name="index">The index where the replacement occurred</param>
        public event Action<TValueType, TValueType, int> ItemReplaced;
        
        /// <summary>
        /// Raised when the collection is cleared (all items removed).
        /// </summary>
        public event Action Cleared;
        
        private readonly ObservableCollection<TValueType> _collection;

        public ObservableList()
        {
            _collection = new ObservableCollection<TValueType>();
            _collection.CollectionChanged += OnCollectionChanged;
        }

        public ObservableList(IEnumerable<TValueType> collection)
        {
            _collection = new ObservableCollection<TValueType>(collection);
            _collection.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Capture the current state as both old and new value
            // (they're the same reference since we mutated in place)
            var currentState = (IReadOnlyList<TValueType>)this;
            
            // Handle add operations
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    var item = (TValueType)e.NewItems[i];
                    var index = e.NewStartingIndex + i;
                    ItemAdded?.Invoke(item, index);
                }
            }
            
            // Handle remove operations
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                for (int i = 0; i < e.OldItems.Count; i++)
                {
                    var item = (TValueType)e.OldItems[i];
                    var index = e.OldStartingIndex + i;
                    ItemRemoved?.Invoke(item, index);
                }
            }
            
            // Handle replace operations
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null && e.NewItems != null)
                {
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        var oldItem = (TValueType)e.OldItems[i];
                        var newItem = (TValueType)e.NewItems[i];
                        var index = e.OldStartingIndex + i;
                        ItemReplaced?.Invoke(oldItem, newItem, index);
                    }
                }
            }
            
            // Handle reset (Clear)
            if (e.Action == NotifyCollectionChangedAction.Reset)
                Cleared?.Invoke();
            
            // Notify standard collection changed subscribers
            CollectionChanged?.Invoke(this, e);
            
            // Notify signal observers
            // Note: Both parameters are the same reference (current state after mutation)
            NotifyObservers(currentState, currentState);
        }

        /// <summary>
        /// Gets the current collection as an IReadOnlyList interface.
        /// This is the signal's value and provides read-only access to the collection.
        /// </summary>
        public override IReadOnlyList<TValueType> GetValue() => this;

        /// <summary>
        /// Convenience property for accessing the collection as a read-only list.
        /// Equivalent to calling GetValue().
        /// </summary>
        public IReadOnlyList<TValueType> Value => GetValue();
        
        // IList<T> and IReadOnlyList<T> implementation
        
        public TValueType this[int index]
        {
            get => _collection[index];
            set => _collection[index] = value;
        }

        public int Count => _collection.Count;
        public bool IsReadOnly => false;

        public void Add(TValueType item) => _collection.Add(item);
        public void AddRange(IEnumerable<TValueType> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
    
            foreach (var item in items)
                _collection.Add(item);
        }
        public void ReplaceAll(IEnumerable<TValueType> newItems)
        {
            Clear();
            AddRange(newItems);
        }
        public void Clear() => _collection.Clear();
        public bool Contains(TValueType item) => _collection.Contains(item);
        public void CopyTo(TValueType[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);
        public IEnumerator<TValueType> GetEnumerator() => _collection.GetEnumerator();
        public int IndexOf(TValueType item) => _collection.IndexOf(item);
        public void Insert(int index, TValueType item) => _collection.Insert(index, item);
        public bool Remove(TValueType item) => _collection.Remove(item);
        public void RemoveAt(int index) => _collection.RemoveAt(index);
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected override void Dispose(bool disposing)
        {
            if (disposing && _collection != null)
                _collection.CollectionChanged -= OnCollectionChanged;

            base.Dispose(disposing);
        }
    }
}