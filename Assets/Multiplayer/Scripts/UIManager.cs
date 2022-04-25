using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {
    [Header("Button")]
    [SerializeField] private Button startServerButton;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    [SerializeField] private Button executePhysicsButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI playersInGameText;

    [Header("InputField")]
    [SerializeField] private TMP_InputField joinCodeInput;

    private bool hasServerStarted;

    private void Awake() => Cursor.visible = true;

    void Update() {
        playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
    }

    void Start() {
        startServerButton?.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartServer()) {
                Logger.Instance.LogInfo("Server started...");
            } else {
                Logger.Instance.LogInfo("Unable to start server...");
            }     
        });

        startHostButton?.onClick.AddListener(async () => {
            if (RelayManager.Instance.IsRelayEnabled) {
                await RelayManager.Instance.SetupRelay();
            }
                
            if (NetworkManager.Singleton.StartHost()) {
                Logger.Instance.LogInfo("Host started...");
            } else {
                Logger.Instance.LogInfo("Unable to start host...");
            }     
        });

        startClientButton?.onClick.AddListener(async () => {
            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text)) {

                await RelayManager.Instance.JoinRelay(joinCodeInput.text);
            }   

            if (NetworkManager.Singleton.StartClient()) {
                Logger.Instance.LogInfo("Client started...");
            } else {
                Logger.Instance.LogInfo("Unable to start client...");
            }   
        });

        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            Logger.Instance.LogInfo($"{id} just connected...");
        };

        NetworkManager.Singleton.OnServerStarted += () => {
            hasServerStarted = true;
        };

        executePhysicsButton.onClick.AddListener(() => {
            if (!hasServerStarted) {
                Logger.Instance.LogWarning("Server has not started...");
                return;
            }

            SpawnerControl.Instance.SpawnObjects();
        });
    }
}