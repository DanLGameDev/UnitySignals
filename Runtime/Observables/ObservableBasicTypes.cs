namespace ObservableGadgets.Observables
{
    public class ObservableBoolean : ObservableValue<bool>
    {
        public ObservableBoolean(bool initialState) : base(initialState) {}
    }
    
    public class ObservableInteger : ObservableValue<int>
    {
        public ObservableInteger(int initialState) : base(initialState) {}
    }
    
    public class ObservableFloat : ObservableValue<float>
    {
        public ObservableFloat(float initialState) : base(initialState) {}
    }

    public class ObservableString : ObservableValue<string>
    {
        public ObservableString(string initialState) : base(initialState) { }
    }
}