using Unity.Netcode;
using UnityEngine;

public class PCHealth : NetworkBehaviour {
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform vrHead;

    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private int maxHealth;
    
    private int health;

    private void Start() {
        health = maxHealth;
    }

    private void Update() {
        if (IsOwner) {
            if (transform.position.y < -0.5f) {
                if (health > 0) {
                    transform.position = spawnPositions[Random.Range(0, spawnPositions.Length)].position;
                } else {
                    PCGameOver();
                }
            }
        }
    }

    private void PCGameOver() {
        // Set camera to VR
        mainCamera.SetParent(vrHead);
    }
}