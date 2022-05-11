using Unity.Netcode;
using UnityEngine;
using TMPro;

public class DestroyHead : NetworkBehaviour {
    public NetworkVariable<Vector3> networkHit = new();
    public NetworkVariable<int> networkIndex = new();

    private TMP_Text healthText;
    private GameObject vrPlayer;

    [SerializeField] private int maxHealth;
    private int health;
    private int oldIndex;

    private void Start() {
        if (IsHost) {
            health = maxHealth;

            healthText = GameObject.Find("HealthTextVR").GetComponent<TMP_Text>();
            vrPlayer = GameObject.Find("NetworkPlayerVR(Clone)");
        }
    }

    private void Update() {
        if (IsClient) {
            if (networkIndex.Value != oldIndex) {
                oldIndex = networkIndex.Value;
                DestoryPartClient(networkHit.Value);
            }
        }

        if (IsOwner) {
            if (health <= 0f) {
                Destroy(vrPlayer);
            }
        }

        if (IsOwner) {
            healthText.text = "Health: " + health + "/" + maxHealth;
        }
    }

    public void DestoryPartClient(Vector3 hit) {
        Collider[] colliders = Physics.OverlapSphere(hit, 0.025f);

        foreach (Collider collider in colliders) {
            if (collider.name == "CubeCell") {
                collider.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    public void DestroyPartHost(Vector3 hit) {
        UpdateDestroyPartServerRpc(hit);
        health--;
    }

    [ServerRpc]
    public void UpdateDestroyPartServerRpc(Vector3 hit) {
        networkHit.Value = hit;
        networkIndex.Value++;
    }
}
