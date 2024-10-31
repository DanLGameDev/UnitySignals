namespace ObservableGadgets.Observables
{
    public class ObservableString : ObservableValue<string>
    {
        public ObservableString(string initialState) : base(initialState) { }
    }
}