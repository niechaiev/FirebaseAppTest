using StateMachine;
using UnityEngine;

namespace States
{
    public class MainMenuState : State
    {
        private const string WelcomeMessage = "Welcome, ";
        private const string GuestUser = "Guest";

        public MainMenuState(StateContext stateContext) : base(stateContext)
        {
        }

        public override void EnterState()
        {
            stateContext.MainCanvas.gameObject.SetActive(true);
            UpdateWelcomeMessage();
        }

        public void UpdateWelcomeMessage()
        {
            var textField = stateContext.TextField;
            textField.text = WelcomeMessage;

            if (stateContext.Auth.CurrentUser.IsAnonymous)
                textField.text += GuestUser;
            else
                textField.text += stateContext.Auth.CurrentUser.Email;
        }

        public override void LeaveState()
        {
            stateContext.MainCanvas.gameObject.SetActive(false);
        }
    }
}