using Unity.Netcode;
using UnityEngine;

public class PlayerType : NetworkBehaviour {
    [SerializeField] private NetworkVariable<int> networkPlayerType = new();
    [SerializeField] private NetworkVariable<bool> networkPlayerTypeSet = new();

    [SerializeField] private GameObject playerPC;
    [SerializeField] private GameObject playerVR;
 
    private void Update() {
        CheckPlayerType();
    }

    public void VRPlayer () {
        if (IsClient && IsOwner && !networkPlayerTypeSet.Value) {
            UpdatePlayerTypeServerRpc(0, true);
            playerPC.SetActive(false);
            playerVR.SetActive(true);
        } else if (IsClient && !IsOwner) {
            playerPC.SetActive(false);
            playerVR.SetActive(true);
        }
    }

    public void PCPlayer () {
        if (IsClient && IsOwner && !networkPlayerTypeSet.Value) {
            UpdatePlayerTypeServerRpc(1, true);
            playerPC.SetActive(true);
            playerVR.SetActive(false);
        } else if (IsClient && !IsOwner) {
            playerPC.SetActive(true);
            playerVR.SetActive(false);
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