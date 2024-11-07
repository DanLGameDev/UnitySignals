using System;
using System.Collections;
using System.Collections.Generic;

namespace DGP.UnitySignals.Collections
{
    public class ObservableList<TValueType> : IObservableCollection<TValueType>, IList<TValueType>, ICollection<TValueType>, IEnumerable<TValueType>, IEnumerable
    {
        public event IObservableCollection<TValueType>.ListChangedHandler ItemAdded;
        public event IObservableCollection<TValueType>.ListChangedHandler ItemRemoved;
        public event IObservableCollection<TValueType>.ListItemChangedHandler ItemChanged;
        public event EventHandler Cleared;

        private List<TValueType> _list = new List<TValueType>();

        public IEnumerator<TValueType> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TValueType item)
        {
            _list.Add(item);
            ItemAdded?.Invoke(this, new ListChangedEventArgs<TValueType> { Item = item, Index = _list.Count - 1 });
        }

        public void Clear()
        {
            _list.Clear();
            Cleared?.Invoke(this, EventArgs.Empty);
        }

        public bool Contains(TValueType item) => _list.Contains(item);
        public void CopyTo(TValueType[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public bool Remove(TValueType item)
        {
            var index = _list.IndexOf(item);
            if (index == -1) return false;
            _list.RemoveAt(index);
            ItemRemoved?.Invoke(this, new ListChangedEventArgs<TValueType> { Item = item, Index = index });
            return true;
        }

        public int Count => _list.Count;
        public bool IsReadOnly => false;
        public int IndexOf(TValueType item) => _list.IndexOf(item);

        public void Insert(int index, TValueType item)
        {
            _list.Insert(index, item);
            ItemAdded?.Invoke(this, new ListChangedEventArgs<TValueType> { Item = item, Index = index });
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            _list.RemoveAt(index);
            ItemRemoved?.Invoke(this, new ListChangedEventArgs<TValueType> { Item = item, Index = index });
        }

        public TValueType this[int index]
        {
            get => _list[index];
            set {
                var oldValue = _list[index];
                _list[index] = value;
                ItemChanged?.Invoke(this, new ListItemChangedEventArgs<TValueType> { Index = index, OldItem = oldValue, NewItem = value });
            }
        }
    }
}