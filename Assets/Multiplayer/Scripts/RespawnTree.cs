using Unity.Netcode;
using UnityEngine;
using Autohand;
using System.Collections.Generic;

public class RespawnTree : NetworkBehaviour {
    [SerializeField] private NetworkVariable<bool> networkIsKinematic = new();
    [SerializeField] private float growSpeed;
    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject treeJoint;

    private List<MeshRenderer> meshRenderers = new();
    private List<MeshCollider> treeCollider = new();
    private List<MeshCollider> meshColliders = new();
    private Rigidbody rb;
    public Grabbable grabbable;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;
    private Vector3 scaleSpeed;
    private bool respawn = false;
    private bool isStarted = false;

   private void Start() {
        foreach (MeshCollider meshCollider in tree.GetComponentsInChildren<MeshCollider>()) {
            treeCollider.Add(meshCollider);
            meshCollider.enabled = false;
        }

        foreach (MeshRenderer meshRenderer in tree.GetComponentsInChildren<Renderer>()) {
            meshRenderers.Add(meshRenderer);
            meshRenderer.enabled = false;
        }

        foreach (MeshCollider meshCollider in GetComponentsInChildren<MeshCollider>()) {
            meshColliders.Add(meshCollider);
        }

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
            RespawnObject();
            grabbable.enabled = false;
        }

        if (respawn) {
            transform.localScale += scaleSpeed * Time.deltaTime;
            if (transform.localScale.x > startScale.x) {
                respawn = false;
                grabbable.enabled = true;
            }
        }

        if (isStarted && !IsHost) {
            if (rb.isKinematic != networkIsKinematic.Value) {
                ChangeKinematic();
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("PC-Player") && collision.collider.gameObject.layer == LayerMask.NameToLayer("Platform")){

            rb.isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("PC-Player") && collision.collider.gameObject.layer != LayerMask.NameToLayer("Platform")) {

            rb.isKinematic = false;
        }
    }

    public override void OnNetworkSpawn() {
        isStarted = true;

        if (!IsHost) {
            ChangeKinematic();
        }
    }

    private void RespawnObject() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = new Vector3(0f, 0f, 0f);
        rb.isKinematic = true;
        foreach (MeshRenderer meshRenderer in meshRenderers) {
            meshRenderer.enabled = false;
        }

        foreach (MeshCollider meshCollider in treeCollider) {
            meshCollider.enabled = false;
        }

        treeJoint.SetActive(true);
        if (IsHost) {
            UpdateIsKinematicServerRpc(true);
        }
        respawn = true;
    }

    private float Map(float s, float a1, float a2, float b1, float b2) {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    private void ChangeKinematic() {
        foreach (MeshCollider meshCollider in meshColliders) {
            meshCollider.convex = !networkIsKinematic.Value;
        }

        foreach (MeshCollider meshCollider in treeCollider) {
            meshCollider.enabled = !networkIsKinematic.Value;
        }

        foreach (MeshRenderer meshRenderer in meshRenderers) {
            meshRenderer.enabled = !networkIsKinematic.Value;
        }

        treeJoint.SetActive(networkIsKinematic.Value);

        rb.isKinematic = networkIsKinematic.Value;
    }

    public void IsKinematic () {
        rb.isKinematic = false;

        treeJoint.SetActive(false);

        foreach (MeshCollider meshCollider in treeCollider) {
            meshCollider.enabled = true;
        }

        foreach (MeshRenderer meshRenderer in meshRenderers) {
            meshRenderer.enabled = true;
        }

        if (IsHost) {
            UpdateIsKinematicServerRpc(false);
        }
    }

    [ServerRpc]
    public void UpdateIsKinematicServerRpc(bool isKinematic) {
        networkIsKinematic.Value = isKinematic;
    }
} 