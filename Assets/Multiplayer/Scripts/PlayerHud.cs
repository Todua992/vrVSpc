using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHud : NetworkBehaviour {
    [SerializeField] private TMP_InputField userNameText;
    [SerializeField] private NetworkVariable<NetworkString> playerNetworkName = new NetworkVariable<NetworkString>();
    [SerializeField] private NetworkVariable<NetworkString> networkPlayerName = new NetworkVariable<NetworkString>();

    private string playerName;
    private bool overlaySet = false;

    private void Awake() {
        userNameText = GameObject.Find("UserNameText").GetComponent<TMP_InputField>();
    }

    public override void OnNetworkSpawn() {
        UpdatePlayerNameServerRpc(userNameText.text);
    }

    public void SetOverlay() {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerOverlay.text = $"{playerNetworkName.Value}";
    }

    public void Update() {
        CheckPlayerName();

        if (!overlaySet && !string.IsNullOrEmpty(playerNetworkName.Value)) {
            SetOverlay();
            overlaySet = true;

            /*
            if (IsClient && IsOwner) {
                gameObject.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
            }
            */
        }

        if (playerName == "") {
            return;
        }

        if (IsServer) {
            if (string.IsNullOrWhiteSpace(userNameText.text)) {
                playerNetworkName.Value = $"Player {OwnerClientId}";
            } else {
                playerNetworkName.Value = $"{playerName}";
            }
        }
    }

    private void CheckPlayerName() {
        if (playerName != networkPlayerName.Value) {
            playerName = networkPlayerName.Value;
        }
    }

    [ServerRpc]
    public void UpdatePlayerNameServerRpc(string playerName) {
        networkPlayerName.Value = playerName;
    }
}