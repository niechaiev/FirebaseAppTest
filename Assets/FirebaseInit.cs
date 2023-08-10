using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseInit : MonoBehaviour
{
    [SerializeField] private Button loadButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private TMP_InputField inputField;


    private const string PlayerKey = "PLAYER_KEY";

    // Start is called before the first frame update
    private FirebaseDatabase database;
    private DataSnapshot dataSnapshot;
    private DatabaseReference reference;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(_ =>
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

            FirebaseAnalytics.LogEvent("custom2","custom",1);
            Debug.LogError(_.Exception);
            Debug.Log("im debugging");
            database = FirebaseDatabase.DefaultInstance;
            reference = database.GetReference(PlayerKey);
            reference.ValueChanged += ValueChangedHandler;
        });


        loadButton.onClick.AddListener(LoadHandler);
        saveButton.onClick.AddListener(SaveHandler);
    }

    private void ValueChangedHandler(object sender, ValueChangedEventArgs e)
    {
        LoadPlayer();
    }

    private void SaveHandler()
    {
        var playerData = new PlayerData { Name = inputField.text };
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