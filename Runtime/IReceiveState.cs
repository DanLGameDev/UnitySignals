namespace ObservableGadgets
{
    public interface IReceiveState<TState>
    {
        public void SetState(TState newState);
    }
}