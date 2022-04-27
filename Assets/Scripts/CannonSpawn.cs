using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CannonSpawn : MonoBehaviour {
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private GameObject cannonPrefab;
    [SerializeField] private float spawnTime;

    private float timer;

    private void Start() {
        timer = spawnTime;
    }

    private void Update() {
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