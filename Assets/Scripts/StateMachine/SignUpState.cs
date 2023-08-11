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
            stateContext.SignButton.GetComponentInChildren<TMP_Text>().text = "Sign Up";
            stateContext.SignButton.onClick.AddListener(stateContext.SignUp);
        }


        public IEnumerator SignUpAsync()
        {
            var registerTask = stateContext.Auth
                .CreateUserWithEmailAndPasswordAsync(
                    stateContext.EmailField.text,
                    stateContext.PasswordField.text);
            
            yield return new WaitUntil(() => registerTask.IsCompleted);
            if (registerTask.Exception != null)
            {
                Debug.LogError(registerTask.Exception);
                yield return null;
            }
            stateContext.SwitchState(stateContext.MainMenuState);
        }

        public override void LeaveState()
        {
            stateContext.SignCanvas.gameObject.SetActive(false);
            stateContext.SignButton.onClick.RemoveListener(stateContext.SignUp);
        }
    }
}