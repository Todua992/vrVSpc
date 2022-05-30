using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PCHealth : NetworkBehaviour {
    [SerializeField] private Camera pcCamera;
    [SerializeField] private Camera vrCamera;
    [SerializeField] private GameObject face;

    [SerializeField] private List<Transform> spawnPositions = new();
    [SerializeField] private int maxHealth;
    [SerializeField] private bool dead;
    [SerializeField] private bool below;

    [SerializeField] private bool test;

    private TMP_Text healthText;
    
    private int health;

    private void Start() {
        if (IsOwner) {
            pcCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            
            if (!test) {
                vrCamera = GameObject.Find("Camera (head)").GetComponent<Camera>();
                face = GameObject.Find("Face");
            } 

            spawnPositions.Add(GameObject.Find("PlayerSpawn").GetComponent<Transform>());

            health = maxHealth;

            healthText = GameObject.Find("HealthTextPC").GetComponent<TMP_Text>();
        }
    }

    private void Update() {
        if (IsOwner && health > 0) {
            healthText.text = "Health: " + health + "/" + maxHealth;

            if (transform.position.y < -0.5f && !below) {
                health--;

                if (health > 0) {
                    transform.position = spawnPositions[Random.Range(0, spawnPositions.Count)].position;
                }
                
                below = true;
            } else if (transform.position.y > -0.5f && below) {
                below = false;
            } else if (below) {
                transform.position = spawnPositions[Random.Range(0, spawnPositions.Count)].position;
            }
        } else if (!dead && IsOwner) {
            healthText.text = "Health: " + health + "/" + maxHealth;
            if (!test) {
                PCGameOver();
            }
        }
    }

    private void PCGameOver() {
        dead = true;
        pcCamera.enabled = false;
        vrCamera.enabled = true;
        face.SetActive(false);
        transform.position = new Vector3(100, 100, 100);
        GetComponent<PlayerControlAuthorative>().enabled = false;
    }
}