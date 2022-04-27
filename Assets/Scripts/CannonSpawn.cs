using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;

public class CannonSpawn : NetworkBehaviour {
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private GameObject cannonPrefab;
    [SerializeField] private float spawnTime;

    private float timer;

    private void Start() {
        if (IsHost) {
            timer = spawnTime;
        }
    }

    private void Update() {
        if (IsHost) {
            if (timer > 0) {
                timer -= Time.deltaTime;
            } else {
                timer = spawnTime;

                int index = Random.Range(0, spawnPoints.Count);
                GameObject cannon = Instantiate(cannonPrefab, spawnPoints[index].position, spawnPoints[index].rotation);
                cannon.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}