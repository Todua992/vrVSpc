using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {
    [HideInInspector] public string playerName;
    [HideInInspector] public int playerType;

    [Header("Button")]
    [SerializeField] private Button startServerButton;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI playersInGameText;

    [Header("InputField")]
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;

    private void Awake() => Cursor.visible = true;

    void Update() {
        playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
    }

    void Start() {
        startServerButton?.onClick.AddListener(() => {
            if (NetworkManager.Singleton.StartServer()) {
                RemoveNetworkUI();
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
                playerType = 0;
                RemoveNetworkUI();
                Logger.Instance.LogInfo("Host started...");
            } else {
                Logger.Instance.LogInfo("Unable to start host...");
            }     
        });

        startClientButton?.onClick.AddListener(async () => {
            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInputField.text)) {

                await RelayManager.Instance.JoinRelay(joinCodeInputField.text);
            }   

            if (NetworkManager.Singleton.StartClient()) {
                playerType = 1;
                RemoveNetworkUI();
                Logger.Instance.LogInfo("Client started...");
            } else {
                Logger.Instance.LogInfo("Unable to start client...");
            }   
        });

        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            Logger.Instance.LogInfo($"{id} just connected...");
        };

        NetworkManager.Singleton.OnServerStarted += () => { };
    }

    private void RemoveNetworkUI() {
        startServerButton.gameObject.SetActive(false);
        startHostButton.gameObject.SetActive(false);
        startClientButton.gameObject.SetActive(false);
        playerNameInputField.gameObject.SetActive(false);
        joinCodeInputField.gameObject.SetActive(false);
    }

    public void UpdatePlayerName() {
        playerName = playerNameInputField.text;
    }
}