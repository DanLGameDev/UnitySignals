using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace DGP.UnitySignals.Collections
{
    /// <summary>
    /// Observable dictionary that emits signals when items are added, removed, replaced, or cleared.
    /// </summary>
    /// <remarks>
    /// Since this is a mutable collection, the oldValue and newValue parameters in SignalValueChanged
    /// will reference the same object (the current dictionary state). For detailed change information,
    /// subscribe to the collection-specific events: ItemAdded, ItemRemoved, ItemReplaced, or Cleared.
    /// </remarks>
    /// <typeparam name="TKey">The type of keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary</typeparam>
    public class ObservableDictionary<TKey, TValue> : SignalBase<IReadOnlyDictionary<TKey, TValue>>, IDictionary<TKey, TValue>, INotifyCollectionChanged, IReadOnlyDictionary<TKey, TValue>
    {
        /// <summary>
        /// Raised when the collection changes in any way (add, remove, replace, clear).
        /// Provides detailed NotifyCollectionChangedEventArgs for WPF/UI binding scenarios.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raised when a key-value pair is added to the dictionary.
        /// </summary>
        /// <param name="key">The key that was added</param>
        /// <param name="value">The value that was added</param>
        public event Action<TKey, TValue> ItemAdded;

        /// <summary>
        /// Raised when a key-value pair is removed from the dictionary.
        /// </summary>
        /// <param name="key">The key that was removed</param>
        /// <param name="value">The value that was removed</param>
        public event Action<TKey, TValue> ItemRemoved;

        /// <summary>
        /// Raised when the value for an existing key is replaced.
        /// </summary>
        /// <param name="key">The key whose value changed</param>
        /// <param name="oldValue">The previous value</param>
        /// <param name="newValue">The new value</param>
        public event Action<TKey, TValue, TValue> ItemReplaced;

        /// <summary>
        /// Raised when the dictionary is cleared (all items removed).
        /// </summary>
        public event Action Cleared;

        private readonly Dictionary<TKey, TValue> _dictionary;

        public ObservableDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        /// <summary>
        /// Gets the current dictionary as an IReadOnlyDictionary interface.
        /// This is the signal's value and provides read-only access to the dictionary.
        /// </summary>
        public override IReadOnlyDictionary<TKey, TValue> GetValue() => this;

        /// <summary>
        /// Convenience property for accessing the dictionary as a read-only dictionary.
        /// Equivalent to calling GetValue().
        /// </summary>
        public new IReadOnlyDictionary<TKey, TValue> Value => GetValue();

        // IDictionary<TKey, TValue> implementation

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                if (_dictionary.TryGetValue(key, out var oldValue))
                {
                    _dictionary[key] = value;
                    var pair = new KeyValuePair<TKey, TValue>(key, value);
                    var oldPair = new KeyValuePair<TKey, TValue>(key, oldValue);
                    ItemReplaced?.Invoke(key, oldValue, value);
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, pair, oldPair));
                }
                else
                {
                    _dictionary[key] = value;
                    var pair = new KeyValuePair<TKey, TValue>(key, value);
                    ItemAdded?.Invoke(key, value);
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair));
                }

                NotifyObservers((IReadOnlyDictionary<TKey, TValue>)this, (IReadOnlyDictionary<TKey, TValue>)this);
            }
        }

        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            var pair = new KeyValuePair<TKey, TValue>(key, value);
            ItemAdded?.Invoke(key, value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair));
            NotifyObservers((IReadOnlyDictionary<TKey, TValue>)this, (IReadOnlyDictionary<TKey, TValue>)this);
        }

        public bool Remove(TKey key)
        {
            if (!_dictionary.TryGetValue(key, out var value))
                return false;

            _dictionary.Remove(key);
            var pair = new KeyValuePair<TKey, TValue>(key, value);
            ItemRemoved?.Invoke(key, value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, pair));
            NotifyObservers((IReadOnlyDictionary<TKey, TValue>)this, (IReadOnlyDictionary<TKey, TValue>)this);
            return true;
        }

        public void Clear()
        {
            _dictionary.Clear();
            Cleared?.Invoke();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            NotifyObservers((IReadOnlyDictionary<TKey, TValue>)this, (IReadOnlyDictionary<TKey, TValue>)this);
        }

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        public bool Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}