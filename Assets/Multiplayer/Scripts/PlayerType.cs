using Unity.Netcode;
using UnityEngine;

public class PlayerType : NetworkBehaviour {
    [SerializeField] private NetworkVariable<int> networkPlayerType = new();
    [SerializeField] private NetworkVariable<bool> networkPlayerTypeSet = new();

    [SerializeField] private GameObject playerPC;
    [SerializeField] private GameObject playerVR;

    private UIManager managerUI;

    public override void OnNetworkSpawn() {
        if (IsClient && IsOwner) {
            managerUI = GameObject.Find("UIManager").GetComponent<UIManager>();

            if (managerUI.playerType == 0) {
                VRPlayer();
            } else if (managerUI.playerType == 1) {
                PCPlayer();
            }
        }
    }

    private void Update() {        
        CheckPlayerType();
    }

    public void VRPlayer () {
        if (IsClient && IsOwner && !networkPlayerTypeSet.Value) {
            UpdatePlayerTypeServerRpc(0, true);
            playerVR.SetActive(true);
        } else if (IsClient && !IsOwner) {
            playerVR.SetActive(true);
        }
    }

    public void PCPlayer () {
        if (IsClient && IsOwner && !networkPlayerTypeSet.Value) {
            UpdatePlayerTypeServerRpc(1, true);
            playerPC.SetActive(true);
        } else if (IsClient && !IsOwner) {
            playerPC.SetActive(true);
        }
    }

    private void CheckPlayerType() {
        if (networkPlayerTypeSet.Value) {
            if (networkPlayerType.Value == 0) {
                VRPlayer();
            } else if (networkPlayerType.Value == 1) {
                PCPlayer();
            }
        }
    }

    [ServerRpc]
    public void UpdatePlayerTypeServerRpc(int playerType, bool playerTypeSet) {
        networkPlayerType.Value = playerType;
        networkPlayerTypeSet.Value = playerTypeSet;

    }
}