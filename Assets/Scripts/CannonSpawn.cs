using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CannonSpawn : NetworkBehaviour {
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private GameObject cannonPrefab;
    [SerializeField] private float spawnTime;
    [SerializeField] private int maxCannonCount;

    private List<GameObject> cannons = new();
    private float timer;

    private void Start() {
        if (IsHost) {
            timer = spawnTime;
        }
    }

    private void Update() {
        if (IsHost && cannons.Count < maxCannonCount) {
            if (timer > 0f) {
                timer -= Time.deltaTime;
            } else {
                timer = spawnTime;
                int index = Random.Range(0, spawnPoints.Count);
                GameObject cannon = Instantiate(cannonPrefab, spawnPoints[index].position, spawnPoints[index].rotation);
                cannon.GetComponent<NetworkObject>().Spawn();
                cannon.GetComponent<CannonShoot>().cannonSpawn = this;
                cannons.Add(cannon);
            }
        }
    }

    public void RemoveCannon(GameObject cannon) {
        if (cannons.Contains(cannon)) {
            cannons.Remove(cannon);
        }
    }
}