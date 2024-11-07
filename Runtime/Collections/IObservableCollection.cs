using System;

namespace DGP.UnitySignals.Collections
{
    public struct ListChangedEventArgs<TValueType>
    {
        public int Index;
        public TValueType Item;
    }
    public struct ListItemChangedEventArgs<TValueType>
    {
        public int Index;
        public TValueType OldItem;
        public TValueType NewItem;
    }
    public interface IObservableCollection<TValueType>
    {
        public delegate void ListChangedHandler(IObservableCollection<TValueType> sender, ListChangedEventArgs<TValueType> e);
        public delegate void ListItemChangedHandler(IObservableCollection<TValueType> sender, ListItemChangedEventArgs<TValueType> e);

        public event ListChangedHandler ItemAdded;
        public event ListChangedHandler ItemRemoved;
        public event ListItemChangedHandler ItemChanged;
        public event EventHandler Cleared;
    }
}