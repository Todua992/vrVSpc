using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CannonSpawn : NetworkBehaviour {
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private GameObject cannonPrefab;
    [SerializeField] private float spawnTime;
    [SerializeField] private int maxCannonCount;
    private int index = 0;

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
                int choose = Random.Range(0, spawnPoints.Count);
                GameObject cannon = Instantiate(cannonPrefab, spawnPoints[choose].position, spawnPoints[choose].rotation);
                cannon.GetComponent<NetworkObject>().Spawn();
                cannon.GetComponentInChildren<CannonShoot>().cannonSpawn = this;
                cannon.GetComponentInChildren<CannonShoot>().index = index;
                index++;
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