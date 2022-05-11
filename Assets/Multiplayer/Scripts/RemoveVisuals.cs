using Unity.Netcode;
using UnityEngine;

public class RemoveVisuals : NetworkBehaviour {
    [SerializeField] private bool enable;

    public override void OnNetworkSpawn() {
        GetComponent<MeshRenderer>().enabled = enable;
    }
}
