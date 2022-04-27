using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHud : NetworkBehaviour {
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private NetworkVariable<NetworkString> networkPlayerName = new();
    [SerializeField] private NetworkVariable<bool> networkPlayerNameSet = new();

    private TMP_InputField playerNameInputField;
    private string playerName;

    private void Awake() => playerNameInputField = GameObject.Find("PlayerNameInputField").GetComponent<TMP_InputField>();

    public void Update() => CheckPlayerName();

    private void CheckPlayerName() {
        if (networkPlayerNameSet.Value) {
            playerName = networkPlayerName.Value;

            UpdatePlayerName();
        }
    }

    private void UpdatePlayerName() => playerNameText.text = playerName;

    public override void OnNetworkSpawn() {
        if (IsClient && IsOwner) {
            if (!string.IsNullOrEmpty(playerNameInputField.text)) {
                UpdatePlayerNameServerRpc(playerNameInputField.text, true);
            } else {
                UpdatePlayerNameServerRpc($"Player {OwnerClientId}", true);
            } 
        } else {
            playerCanvas.SetActive(true);
        }
    }

    [ServerRpc]
    public void UpdatePlayerNameServerRpc(string playerName, bool playerNameSet) {
        networkPlayerName.Value = playerName;
        networkPlayerNameSet.Value = playerNameSet;
    }
}