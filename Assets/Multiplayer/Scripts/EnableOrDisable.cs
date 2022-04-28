using Unity.Netcode;
using UnityEngine;

public class EnableOrDisable : NetworkBehaviour {
    [SerializeField] private bool enable;

    public override void OnNetworkSpawn() {
        if (!IsHost) {
            gameObject.SetActive(enable);
        }
    }
}