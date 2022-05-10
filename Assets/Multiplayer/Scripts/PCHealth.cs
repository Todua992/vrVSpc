using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PCHealth : NetworkBehaviour {
    [SerializeField] private Camera pcCamera;
    [SerializeField] private Camera vrCamera;
    [SerializeField] private GameObject face;

    [SerializeField] private List<Transform> spawnPositions = new();
    [SerializeField] private int maxHealth;
    
    private int health;

    private void Start() {
        if (IsOwner) {
            pcCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            vrCamera = GameObject.Find("Camera (head)").GetComponent<Camera>();
            face = GameObject.Find("Face");
            
            foreach (Transform t in GameObject.Find("CannonSpawnpoints").GetComponentsInChildren<Transform>()) {
                spawnPositions.Add(t);
            }

            spawnPositions.RemoveAt(0);
        }

        health = maxHealth;
    }

    private async void Update() {
        if (IsOwner) {
            if (transform.position.y < -0.5f) {
                if (health > 0) {
                    transform.position = spawnPositions[Random.Range(0, spawnPositions.Count)].position;
                    health--;

                    await Task.Delay(50);
                } else {
                    PCGameOver();
                }
            }
        }
    }

    private void PCGameOver() {
        pcCamera.enabled = false;
        vrCamera.enabled = true;
        face.SetActive(false);
    }
}