using States;

namespace StateMachine
{
    public class ChangeDataState : State
    {
        public ChangeDataState(StateContext stateContext) : base(stateContext)
        {
        }

        public override void EnterState()
        {
            stateContext.PlayerDataCanvas.gameObject.SetActive(true);

        }

        public override void LeaveState()
        {
            stateContext.PlayerDataCanvas.gameObject.SetActive(false);
        }
    }
}