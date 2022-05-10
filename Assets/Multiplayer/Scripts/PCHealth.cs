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
    [SerializeField] private bool dead;
    
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
        if (IsOwner && health > 0) {
            if (transform.position.y < -0.5f) {
                transform.position = spawnPositions[Random.Range(0, spawnPositions.Count)].position;
                health--;

                await Task.Delay(1000);
            }
        } else if (!dead) {
            PCGameOver();
        }
    }

    private void PCGameOver() {
        dead = true;
        pcCamera.enabled = false;
        vrCamera.enabled = true;
        face.SetActive(false);
    }
}