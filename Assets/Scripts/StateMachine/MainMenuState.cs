using StateMachine;
using UnityEngine;

namespace States
{
    public class MainMenuState : State
    {
        public MainMenuState(StateContext stateContext) : base(stateContext)
        {
        }

        public override void EnterState()
        {
            stateContext.MainCanvas.gameObject.SetActive(true);
        }

        public override void LeaveState()
        {
            stateContext.MainCanvas.gameObject.SetActive(false);
        }
    }
}