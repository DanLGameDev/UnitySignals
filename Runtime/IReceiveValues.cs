namespace ObservableGadgets
{
    public interface IReceiveValues<TValueType>
    {
        public void SetValue(TValueType newValue);
    }
}