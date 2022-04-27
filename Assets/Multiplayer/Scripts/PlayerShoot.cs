using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerShoot : NetworkBehaviour {
    public NetworkVariable<bool> networkShoot = new NetworkVariable<bool>();
    public NetworkVariable<int> networkIndex = new NetworkVariable<int>();

    public void UpdateNetworkValues(bool shoot, int index) {
        if (IsClient && IsOwner) {
            UpdateNetworkValuesServerRpc(shoot, index);
        }
    }

    [ServerRpc]
    public void UpdateNetworkValuesServerRpc(bool shoot, int index) {
        networkShoot.Value = shoot;
        networkIndex.Value = index;
    }
}