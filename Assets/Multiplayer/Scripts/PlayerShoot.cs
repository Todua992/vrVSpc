using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerShoot : NetworkBehaviour {
    public NetworkVariable<bool> networkShoot = new NetworkVariable<bool>();

    public void UpdateNetowrkValues(bool shoot) {
        if (IsClient && IsOwner) {
            UpdateNetworkValuesServerRpc(shoot);
        }
    }

    [ServerRpc]
    public void UpdateNetworkValuesServerRpc(bool shoot) {
        networkShoot.Value = shoot;
    }
}