using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Database;
using States;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace StateMachine
{
    public class StateContext : MonoBehaviour
    {
        public Canvas MainCanvas;
        public Canvas PlayerDataCanvas;
        public Canvas SignCanvas;
        [SerializeField] public TMP_Text textField;
        [SerializeField] public Button signInButton;
        [SerializeField] public Button signOutButton;
        [SerializeField] public Button signUpButton;
        [SerializeField] public Button changeButton;
        [SerializeField] public Button loadButton;
        [SerializeField] public Button saveButton;
        [SerializeField] public TMP_InputField inputField;
        public Button goBackButton;
        public Button signButton;
        public TMP_InputField emailField;
        public TMP_InputField passwordField;

        private State currentState;

        private MainMenuState mainMenuState;
        private ChangeDataState changeDataState;
        private SignUpState signUpState;
        private SignInState signInState;

        private const string PlayerKey = "PLAYER_KEY";

        // Start is called before the first frame update
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

        void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(_ =>
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                FirebaseAnalytics.LogEvent("custom2", "custom", 1);
                if (_.Exception != null)
                    Debug.LogError(_.Exception);

                Debug.Log("im debugging");
                database = FirebaseDatabase.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                reference = database.GetReference(PlayerKey);
                reference.ValueChanged += ValueChangedHandler;

                if (auth.CurrentUser.IsValid())
                {
                    textField.text = $"Welcome, {auth.CurrentUser.Email}";
                }
            });

            AddListeners();
            SwitchState(mainMenuState);
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
            Auth.SignOut();
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