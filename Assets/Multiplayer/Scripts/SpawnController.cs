using Unity.Netcode;
using UnityEngine;

public class SpawnController : NetworkBehaviour {
    [SerializeField] private GameObject prefabVR;
    [SerializeField] private GameObject prefabPC;

    public override void OnNetworkSpawn() {
        if (IsHost) {
            CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId, 0);
        } else {
            CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId, 1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreatePlayerServerRpc(ulong clientId, int prefabId) {
        GameObject player;
        
        if (prefabId == 0) {
            player = Instantiate(prefabVR);
        } else {
            player = Instantiate(prefabPC);
        }
            
        NetworkObject netObj = player.GetComponent<NetworkObject>();
        player.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);
    }
}
