using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using States;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;

namespace StateMachine
{
    public class StateContext : MonoBehaviour
    {
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private Canvas playerDataCanvas;
        [SerializeField] private Canvas signCanvas;
        [SerializeField] private TMP_Text textField;
        [SerializeField] private Button signInButton;
        [SerializeField] private Button signOutButton;
        [SerializeField] private Button signUpButton;
        [SerializeField] private Button changeButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button goBackButton;
        [SerializeField] private Button signButton;
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passwordField;
        public Canvas MainCanvas => mainCanvas;
        public Canvas PlayerDataCanvas => playerDataCanvas;
        public Canvas SignCanvas => signCanvas;
        public TMP_Text TextField => textField;
        public Button SignInButton => signInButton;
        public Button SignOutButton => signOutButton;
        public Button SignUpButton => signUpButton;
        public Button ChangeButton => changeButton;
        public Button LoadButton => loadButton;
        public Button SaveButton => saveButton;
        public TMP_InputField InputField => inputField;
        public Button GoBackButton => goBackButton;
        public Button SignButton => signButton;
        public TMP_InputField EmailField => emailField;
        public TMP_InputField PasswordField => passwordField;
        public State CurrentState => currentState;
        public FirebaseDatabase Database => database;
        public DataSnapshot DataSnapshot => dataSnapshot;
        public DatabaseReference Reference => reference;

        private State currentState;
        private MainMenuState mainMenuState;
        private ChangeDataState changeDataState;
        private SignUpState signUpState;
        private SignInState signInState;
        public MainMenuState MainMenuState => mainMenuState;
        public ChangeDataState ChangeDataState => changeDataState;
        public SignUpState SignUpState => signUpState;
        public SignInState SignInState => signInState;

        private const string PlayerKey = "PLAYER_KEY";

        private FirebaseDatabase database;
        private DataSnapshot dataSnapshot;
        private DatabaseReference reference;
        private FirebaseAuth auth;
        public FirebaseAuth Auth => auth;


        private void Awake()
        {
            mainMenuState = new MainMenuState(this);
            changeDataState = new ChangeDataState(this);
            signUpState = new SignUpState(this);
            signInState = new SignInState(this);
        }

        public void SwitchState(State newGameState)
        {
            if (currentState != null)
                currentState.LeaveState();
            currentState = newGameState;
            currentState.EnterState();
        }

        public void SignUp()
        {
            StartCoroutine(signUpState.SignUpAsync());
        }

        public void SignIn()
        {
            StartCoroutine(signInState.SignInAsync());
        }

        void Start()
        {
            StartCoroutine(InitializeFirebase());
        }

        IEnumerator InitializeFirebase()
        {
            var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
            yield return new WaitUntil(() => dependencyTask.IsCompleted);
            
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

            if (dependencyTask.Exception != null)
                Debug.LogError(dependencyTask.Exception);

            CacheFirebaseInstances();

            yield return SignInAnonymously();
     
            AddListeners();
            SwitchState(mainMenuState);
        }

        private IEnumerator SignInAnonymously()
        {
            if (auth.CurrentUser == null)
            {
                var authTask = auth.SignInAnonymouslyAsync();
                yield return new WaitUntil(() => authTask.IsCompleted);

                if (authTask.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                }

                if (authTask.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + authTask.Exception);
                }
            }
            mainMenuState.UpdateWelcomeMessage();
        }
        private void CacheFirebaseInstances()
        {
            database = FirebaseDatabase.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            reference = database.GetReference(PlayerKey);
            reference.ValueChanged += ValueChangedHandler;
        }

        private void AddListeners()
        {
            loadButton.onClick.AddListener(LoadHandler);
            saveButton.onClick.AddListener(SaveHandler);
            changeButton.onClick.AddListener(() => SwitchState(changeDataState));
            goBackButton.onClick.AddListener(() => SwitchState(mainMenuState));
            signUpButton.onClick.AddListener(() => SwitchState(signUpState));
            signInButton.onClick.AddListener(() => SwitchState(signInState));
            signOutButton.onClick.AddListener(SignOut);
        }

        private void SignOut()
        {
            auth.SignOut();
            StartCoroutine(SignInAnonymously());


        }


        private void ValueChangedHandler(object sender, ValueChangedEventArgs e)
        {
            LoadPlayer();
        }

        private void SaveHandler()
        {
            var playerData = new PlayerData { Name = inputField.text };
            Debug.Log("saving1");
            SavePlayer(playerData);
        }

        public void LoadHandler()
        {
            LoadPlayer();
        }

        public void SavePlayer(PlayerData player)
        {
            database.GetReference(PlayerKey).SetRawJsonValueAsync(JsonUtility.ToJson(player));
        }

        public async Task<PlayerData?> LoadPlayer()
        {
            if (!await SaveExists()) return null;
            var playerData = JsonUtility.FromJson<PlayerData>(dataSnapshot.GetRawJsonValue());
            inputField.text = playerData.Name;
            return playerData;
        }

        public async Task<bool> SaveExists()
        {
            dataSnapshot = await database.GetReference(PlayerKey).GetValueAsync();
            return dataSnapshot.Exists;
        }
    }
}