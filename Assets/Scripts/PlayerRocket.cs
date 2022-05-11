using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerRocket : NetworkBehaviour {
    public NetworkVariable<float> networkHorizontal = new();
    public NetworkVariable<float> networkVertical = new();
    public NetworkVariable<bool> networkShoot = new();
    public NetworkVariable<int> networkIndex = new();

    private void Start() {
        if (IsClient && IsOwner) {
            UpdateShoot(false, -1);
        }
    }

    public void UpdateShoot(bool shoot, int index) {
        if (IsClient && IsOwner) {
            UpdateShootServerRpc(shoot, index); 
        }
    }

    [ServerRpc]
    public void UpdateShootServerRpc(bool shoot, int index) {
        networkShoot.Value = shoot;
        networkIndex.Value = index;
    }

    public void UpdateInput(float horizontal, float vertical) {
        UpdateInputServerRpc(horizontal, vertical);
    }

    [ServerRpc]
    public void UpdateInputServerRpc(float horizontal, float vertical) {
        networkHorizontal.Value = horizontal;
        networkVertical.Value = vertical;
    }
}
