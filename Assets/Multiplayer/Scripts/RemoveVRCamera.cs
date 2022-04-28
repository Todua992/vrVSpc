using Unity.Netcode;
using UnityEngine;

public class RemoveVRCamera : NetworkBehaviour {
    public override void OnNetworkSpawn() {
        if (!IsHost) {
            gameObject.SetActive(false);
        }
    }
}