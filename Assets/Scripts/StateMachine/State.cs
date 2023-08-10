using StateMachine;

namespace States
{
    public abstract class State
    {
        protected StateContext stateContext;

        protected State(StateContext stateContext)
        {
            this.stateContext = stateContext;
        }

        public abstract void EnterState();
        public abstract void LeaveState();
        
    }
}