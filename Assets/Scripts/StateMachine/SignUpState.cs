using States;
using TMPro;

namespace StateMachine
{
    public class SignUpState : State
    {
        public SignUpState(StateContext stateContext) : base(stateContext)
        {
        }

        public override void EnterState()
        {
            stateContext.SignCanvas.gameObject.SetActive(true);
            stateContext.signButton.GetComponentInChildren<TMP_Text>().text = "Sign Up";
        }

        public override void LeaveState()
        {
            stateContext.SignCanvas.gameObject.SetActive(false);
        }
    }
}