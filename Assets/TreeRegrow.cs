using UnityEngine;

public class TreeRegrow : MonoBehaviour {
    [SerializeField] private float growSpeed;

    private Rigidbody rb;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;
    private Vector3 scaleSpeed;
    private bool respawnTree = false;

   private void Start() {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;

        scaleSpeed = new Vector3(growSpeed * startScale.x, growSpeed * startScale.y, growSpeed * startScale.z);
    }

    private void Update() {
        if (transform.position.y <= -10f) {
            RespawnTree();
        }

        if (respawnTree) {
            transform.localScale += scaleSpeed * Time.deltaTime;
            if (transform.localScale.x > startScale.x) {
                respawnTree = false;
            }
        }
    }

    private void RespawnTree() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = new Vector3(0f, 0f, 0f);
        rb.isKinematic = true;
        respawnTree = true;
    }
} 