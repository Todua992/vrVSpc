using Unity.Netcode;
using UnityEngine;
using Autohand;

public class TreeRegrow : NetworkBehaviour {
    [SerializeField] private NetworkVariable<bool> networkIsKinematic = new();
    [SerializeField] private NetworkVariable<bool> networkMesh = new();
    
    [SerializeField] private float growSpeed;

    private MeshCollider meshCollider;
    private Rigidbody rb;
    public Grabbable grabbable;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;
    private Vector3 scaleSpeed;
    private bool respawnTree = false;

   private void Start() {
        meshCollider = GetComponent<MeshCollider>();
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<Grabbable>();

        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;

        scaleSpeed = new Vector3(growSpeed * startScale.x, growSpeed * startScale.y, growSpeed * startScale.z);
        rb.mass = Map(startScale.x * startScale.y * startScale.z, 0.125f, 3.375f, 10f, 40f);
    }

    private void Update() {
        if (transform.position.y <= -10f) {
            RespawnTree();
            grabbable.enabled = false;
        }

        if (respawnTree) {
            transform.localScale += scaleSpeed * Time.deltaTime;
            if (transform.localScale.x > startScale.x) {
                respawnTree = false;
                grabbable.enabled = true;

            }
        }

        if (!IsHost) {
            if (rb.isKinematic != networkIsKinematic.Value) {
                rb.isKinematic = networkIsKinematic.Value;
                meshCollider.convex = !networkIsKinematic.Value;
            }
        }

    }

    private void RespawnTree() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = new Vector3(0f, 0f, 0f);
        meshCollider.convex = false;
        rb.isKinematic = true;
        UpdateIsKinematicServerRpc(true);
        respawnTree = true;
    }

    public void IsKinematic () {
        rb.isKinematic = false;
        meshCollider.convex = true;
        UpdateIsKinematicServerRpc(false);
    }

    private float Map(float s, float a1, float a2, float b1, float b2) {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    [ServerRpc]
    public void UpdateIsKinematicServerRpc(bool isKinematic) {
        networkIsKinematic.Value = isKinematic;
    }
} 