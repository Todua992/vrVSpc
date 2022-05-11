using UnityEngine;

public class CannonMovement : MonoBehaviour {
    [SerializeField] private GameObject cloud;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;

    private GameObject target;
    private Quaternion targetRotation;
    private bool track;

    private void Start() {
        target = GameObject.Find("Camera (head)");
    }

    private void Update() {
        if (track) {
            Vector3 targetPosition = target.transform.position + new Vector3(0f, 0.15f, 0f);

            targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime );
        }
    }

    private void OnTriggerEnter(Collider collider) {
        cloud.SetActive(false);
        rb.drag = 1f;
        track = true;
    }
}