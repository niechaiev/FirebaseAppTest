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
            if (stateContext.Auth.CurrentUser != null)
            {
                stateContext.TextField.text = $"{WelcomeMessage} {stateContext.Auth.CurrentUser.Email}";
                return;
            }

            stateContext.TextField.text = $"{WelcomeMessage} {GuestUser}";
        }

        public override void LeaveState()
        {
            stateContext.MainCanvas.gameObject.SetActive(false);
        }
    }
}