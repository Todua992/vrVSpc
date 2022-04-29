using Unity.Netcode;
using UnityEngine;

public class RockSpawn : NetworkBehaviour {
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private GameObject Prefab;
    [SerializeField] private int Maxrocks;
    [SerializeField] private float TimeTillSpawn;
    public int Currentrocks;
    private float CurrentTime;

    private void Update() {
        if (IsHost) {
            if (Currentrocks < Maxrocks) {
                CurrentTime -= Time.deltaTime;
                if (CurrentTime <= 0f) {
                    Currentrocks++;
                    Spawn();
                    CurrentTime = TimeTillSpawn;
                }
            }
        }
    }

    private void Spawn() {
        GameObject rock = Instantiate(Prefab, SpawnPoint.position, SpawnPoint.rotation);
        rock.GetComponent<NetworkObject>().Spawn();
    }
}