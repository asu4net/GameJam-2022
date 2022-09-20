namespace asu4net.StateSystem
{
    public interface IState
    {
        public string id { get; set; }
        public StateManager stateManager { get; set; }
        public void EnterState(IState previousState);
        public void UpdateState();
        public void ExitState();
    }
}
