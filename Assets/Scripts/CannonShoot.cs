using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CannonShoot : NetworkBehaviour {
    public int index = -1;
    public NetworkVariable<int> networkIndex = new();

    [SerializeField] private float holdTime;
    [SerializeField] private float shootSpeed;
    [SerializeField] private Transform cannonBallSpawn;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private ParticleSystem explosionVFX;
    [SerializeField] private GameObject interactUI;

    private bool colliding = false;
    private bool shoot = false;
    private float timer;
    
    [HideInInspector] public CannonSpawn cannonSpawn;
    private List<PlayerShoot> playerShoots = new();

    private void Start() {
        timer = holdTime;

        interactUI = GameObject.Find("Canvas").transform.Find("Interact").gameObject;

        if (IsHost) {
            UpdateIndexServerRpc(index);
        }
    }

    private void Update() {
        if (index != networkIndex.Value) {
            CheckIndexValue();
            return;
        }

        if (playerShoots.Count == 0) {
            return;
        }
        
        if (shoot == true) {
            Shoot();
        }

        if (colliding && Input.GetKey(KeyCode.E)) {
            if (timer > 0f) {
                timer -= Time.deltaTime;
            } else {
                foreach (PlayerShoot selected in playerShoots) {
                    selected.UpdateNetworkValues(true, index);
                }
            }
        } else if (timer != holdTime) {
            timer = holdTime;
        }

        CheckNetworkValues();
    }

    private void Shoot() {
        explosionVFX.Play();
        foreach (PlayerShoot selected in playerShoots) {
            if (selected.gameObject.GetComponent<NetworkObject>().IsOwner) {
                interactUI.SetActive(false);
            }
        }

        if (IsHost) {
            GameObject cannonBall = Instantiate(cannonBallPrefab, cannonBallSpawn.position, cannonBallSpawn.rotation);
            cannonBall.GetComponent<NetworkObject>().Spawn();
            cannonBall.GetComponent<Rigidbody>().AddForce(cannonBallSpawn.forward * shootSpeed, ForceMode.Impulse);
            cannonSpawn.RemoveCannon(transform.parent.gameObject);
            Destroy(transform.parent.gameObject);
        }
    }

    private void CheckNetworkValues() {
        foreach (PlayerShoot selected in playerShoots) {
            if (shoot != selected.networkShoot.Value && index == selected.networkIndex.Value) {
                shoot = selected.networkShoot.Value;
            }
        }
    }

    private void CheckIndexValue() {
        if (index != networkIndex.Value) {
            index = networkIndex.Value; 
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Player")) {
            if (collider.gameObject.GetComponent<NetworkObject>().IsOwner) {
                interactUI.SetActive(true);
            }

            playerShoots.Add(collider.GetComponent<PlayerShoot>());
            colliding = true;
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Player")) {
            if (collider.gameObject.GetComponent<NetworkObject>().IsOwner) {
                interactUI.SetActive(false);
            }

            playerShoots.Remove(collider.GetComponent<PlayerShoot>());
            colliding = false;
        }
    }

    [ServerRpc]
    public void UpdateIndexServerRpc(int index) => networkIndex.Value = index;
}