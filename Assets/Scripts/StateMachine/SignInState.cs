using System.Collections;
using States;
using TMPro;
using UnityEngine;

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
            stateContext.SignButton.GetComponentInChildren<TMP_Text>().text = "Sign In";
            stateContext.SignButton.onClick.AddListener(stateContext.SignIn);
            
        }

        public override void LeaveState()
        {
            stateContext.SignCanvas.gameObject.SetActive(false);
            stateContext.SignButton.onClick.RemoveListener(stateContext.SignIn);
        }

        public IEnumerator SignInAsync()
        {
            var registerTask = stateContext.Auth
                .SignInWithEmailAndPasswordAsync(
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
    }
}