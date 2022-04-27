using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHud : NetworkBehaviour {
    [SerializeField] private TMP_InputField userNameText;
    [SerializeField] private NetworkVariable<NetworkString> playerNetworkName = new NetworkVariable<NetworkString>();

    private bool overlaySet = false;

    private void Awake() {
        userNameText = GameObject.Find("UserNameText").GetComponent<TMP_InputField>();
    }

    public override void OnNetworkSpawn() {
        if (IsServer) {
            if (string.IsNullOrWhiteSpace(userNameText.text)) {
                playerNetworkName.Value = $"Player {OwnerClientId}";
            } else {
                playerNetworkName.Value = $"{userNameText.text}";
            }
        }
    }

    public void SetOverlay() {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerOverlay.text = $"{playerNetworkName.Value}";
    }

    public void Update() {
        if (!overlaySet && !string.IsNullOrEmpty(playerNetworkName.Value)) {
            SetOverlay();
            overlaySet = true;

            if (IsClient && IsOwner) {
                gameObject.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
            }
        }
    }
}