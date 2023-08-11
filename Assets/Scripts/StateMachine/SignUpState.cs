using System.Collections;
using States;
using TMPro;
using UnityEngine;

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
            stateContext.signButton.onClick.AddListener(stateContext.SignUp);
        }


        public IEnumerator SignUpAsync()
        {
            var registerTask = stateContext.Auth
                .CreateUserWithEmailAndPasswordAsync(
                    stateContext.emailField.text,
                    stateContext.passwordField.text);
            
            yield return new WaitUntil(() => registerTask.IsCompleted);

            Debug.Log(registerTask.Exception);
        }

        public override void LeaveState()
        {
            stateContext.SignCanvas.gameObject.SetActive(false);
            stateContext.signButton.onClick.RemoveListener(stateContext.SignUp);
        }
    }
}