using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerShoot : NetworkBehaviour {
    public NetworkVariable<bool> networkColliding = new NetworkVariable<bool>();
    public NetworkVariable<bool> networkShooting = new NetworkVariable<bool>();
    public NetworkVariable<bool> networkTiming = new NetworkVariable<bool>();

    #region CLIENT CALLS

    public void Colliding(bool colliding) {
        if (IsClient && IsOwner) {
            UpdateCollidingServerRpc(colliding);
        }
    }

    public void Shooting(bool shooting) {
        if (IsClient && IsOwner) {
            UpdateShootingServerRpc(shooting);
        }
    }

    public void Timing(bool timing) {
        if (IsClient && IsOwner) {
            UpdateTimingServerRpc(timing);
        }
    }

    #endregion

    #region SERVER CALLS

    [ServerRpc]
    public void UpdateCollidingServerRpc(bool colliding) {
        networkColliding.Value = colliding;
    }

    [ServerRpc]
    public void UpdateShootingServerRpc(bool shooting) {
        networkShooting.Value = shooting;
    }

    [ServerRpc]
    public void UpdateTimingServerRpc(bool timing) {
        networkTiming.Value = timing;
    }

    #endregion
}