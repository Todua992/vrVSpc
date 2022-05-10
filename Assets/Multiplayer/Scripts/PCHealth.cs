using Unity.Netcode;
using UnityEngine;

public class PCHealth : NetworkBehaviour {
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform vrHead;
    [SerializeField] private PlayerCameraFollow cameraFollow;

    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private int maxHealth;
    
    private int health;

    private void Start() {
        if (IsOwner) {
            mainCamera = GameObject.Find("MainCamera").transform;
            vrHead = GameObject.Find("Camera (head)").transform;
        }

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
        cameraFollow.enabled = false;
        mainCamera.position = new Vector3(0f, 0f, 0f);
        mainCamera.SetParent(vrHead);
    }
}