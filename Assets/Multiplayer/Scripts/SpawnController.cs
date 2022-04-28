using Unity.Netcode;
using UnityEngine;

public class SpawnController : NetworkBehaviour {
    [SerializeField] private GameObject prefabVR;

    public override void OnNetworkSpawn() {
        if (IsHost) {
            CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId, 0);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreatePlayerServerRpc(ulong clientId, int prefabId) {
        GameObject player = Instantiate(prefabVR);
            
        NetworkObject netObj = player.GetComponent<NetworkObject>();
        player.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);
    }
}
