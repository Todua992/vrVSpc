using UnityEngine;
using Autohand;

public class Stones : MonoBehaviour {
    [SerializeField] private float growSpeed;

    private Rigidbody rb;

    [HideInInspector] public Grabbable grabbable;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;
    private Vector3 scaleSpeed;
    private bool respawn = false;

    private void Start() {
        grabbable = GetComponent<Grabbable>();
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;

        scaleSpeed = new Vector3(growSpeed * startScale.x, growSpeed * startScale.y, growSpeed * startScale.z);
        rb.mass = Map(startScale.x * startScale.y * startScale.z, 0.125f, 3.375f, 2.5f, 10f);
    }

    private void Update() {
        if (transform.position.y <= -10f) {
            RespawnTree();
            grabbable.enabled = false;
        }

        if (respawn) {
            transform.localScale += scaleSpeed * Time.deltaTime;
            if (transform.localScale.x > startScale.x) {
                respawn = false;
                grabbable.enabled = true;

            }
        }
    }

    private void RespawnTree() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = new Vector3(0f, 0f, 0f);
        rb.isKinematic = true;
        respawn = true;
    }

    private float Map(float s, float a1, float a2, float b1, float b2) {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}