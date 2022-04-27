using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHud : NetworkBehaviour {
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private NetworkVariable<NetworkString> networkPlayerName = new();
    [SerializeField] private NetworkVariable<bool> networkPlayerNameSet = new();

    private UIManager UIManager;
    private string playerName;

    private void Awake() => UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

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
            if (!string.IsNullOrEmpty(UIManager.playerName)) {
                UpdatePlayerNameServerRpc(UIManager.playerName, true);
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