namespace ObservableGadgets.Observables
{
    public class ObservableClampedInteger : ObservableInteger
    {
        private ClampMode _clampMode;

        public ObservableClampedInteger(int initialState, ClampMode clampMode) : base(initialState)
        {
            _clampMode = clampMode;
        }
    }
}