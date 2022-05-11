using Unity.Netcode;
using UnityEngine;

public class DestroyHead : NetworkBehaviour {
    public NetworkVariable<Vector3> networkHit = new();
    public NetworkVariable<int> networkIndex = new();

    [SerializeField] private int maxHealth;
    private int health;
    private int oldIndex;

    private void Start() {
        if (IsHost) {
            health = maxHealth;
        }
    }

    private void Update() {
        if (IsClient) {
            if (networkIndex.Value != oldIndex) {
                oldIndex = networkIndex.Value;
                DestoryPartClient(networkHit.Value);
            }
        }
    }

    public void DestoryPartClient(Vector3 hit) {
        Collider[] colliders = Physics.OverlapSphere(hit, 0.05f);

        foreach (Collider collider in colliders) {
            if (collider.name == "CubeCell") {
                collider.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    public void DestroyPartHost(Vector3 hit) {
        UpdateDestroyPartServerRpc(hit);
    }

    [ServerRpc]
    public void UpdateDestroyPartServerRpc(Vector3 hit) {
        networkHit.Value = hit;
        networkIndex.Value++;
    }
}
