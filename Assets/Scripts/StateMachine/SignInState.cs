using States;
using TMPro;

namespace StateMachine
{
    public class SignInState : State
    {
        public SignInState(StateContext stateContext) : base(stateContext)
        {
        }

        public override void EnterState()
        {
            stateContext.SignCanvas.gameObject.SetActive(true);
            stateContext.signButton.GetComponentInChildren<TMP_Text>().text = "Sign In";
        }

        public override void LeaveState()
        {
            stateContext.SignCanvas.gameObject.SetActive(false);
        }
    }
}