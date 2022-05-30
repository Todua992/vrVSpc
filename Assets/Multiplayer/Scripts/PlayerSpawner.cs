using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour {
    [SerializeField] private GameObject playerPrefabA;
    [SerializeField] private GameObject playerPrefabB;
    [SerializeField] private bool test;

    public override void OnNetworkSpawn() {
        if (IsClient && IsOwner && IsHost) {
            SpawnPlayerServerRpc(NetworkManager.LocalClientId, 0);
        } else if (IsClient && IsOwner) {
            SpawnPlayerServerRpc(NetworkManager.LocalClientId, 1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId, int prefabId) {
        GameObject newPlayer;
        if (prefabId == 0) {
            newPlayer = Instantiate(playerPrefabA);
            if (!test) {
                Destroy(GameObject.Find("MainCamera"));
            }
        } else {
            newPlayer = Instantiate(playerPrefabB);
        }
        NetworkObject netObj = newPlayer.GetComponent<NetworkObject>();
        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);
        Destroy(gameObject);
    }
}
