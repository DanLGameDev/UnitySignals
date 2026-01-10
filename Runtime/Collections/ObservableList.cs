using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DGP.UnitySignals.Collections
{
    public class ObservableList<TValueType> : SignalBase<ObservableList<TValueType>>, IList<TValueType>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event Action<TValueType, int> ItemAdded;
        public event Action<TValueType, int> ItemRemoved;
        
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
            
            // Handle replace operations (fire both removed and added)
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        var item = (TValueType)e.OldItems[i];
                        var index = e.OldStartingIndex + i;
                        ItemRemoved?.Invoke(item, index);
                    }
                }
                
                if (e.NewItems != null)
                {
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var item = (TValueType)e.NewItems[i];
                        var index = e.NewStartingIndex + i;
                        ItemAdded?.Invoke(item, index);
                    }
                }
            }
            
            // Handle reset (Clear) - fire removed for all items
            if (e.Action == NotifyCollectionChangedAction.Reset && e.OldItems != null)
            {
                for (int i = 0; i < e.OldItems.Count; i++)
                {
                    var item = (TValueType)e.OldItems[i];
                    ItemRemoved?.Invoke(item, i);
                }
            }
            
            CollectionChanged?.Invoke(this, e);
            NotifyObservers(this, this);
        }

        public override ObservableList<TValueType> GetValue() => this;
        public ObservableList<TValueType> Value => GetValue();

        // IList<T> implementation - delegates to internal ObservableCollection
        public TValueType this[int index]
        {
            get => _collection[index];
            set => _collection[index] = value;
        }

        public int Count => _collection.Count;
        public bool IsReadOnly => false;

        public void Add(TValueType item) => _collection.Add(item);
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
            {
                _collection.CollectionChanged -= OnCollectionChanged;
            }

            base.Dispose(disposing);
        }
    }
}