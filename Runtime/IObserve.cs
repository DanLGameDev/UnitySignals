namespace ObservableGadgets
{
    public interface IObserve<TValueType>
    {
        public void ValueChanged(IEmitValues<TValueType> sender, TValueType oldValue, TValueType newValue);
    }
}