using Unity.Netcode;
using UnityEngine;

public class MissileController : NetworkBehaviour {
    [SerializeField] private float holdTime;
    [SerializeField] private float flySpeed;
    [SerializeField] private float rotateSpeed;

    private bool shoot;
    private bool colliding;
    private float timer;


    private void Update() {
        if (colliding && Input.GetKey(KeyCode.E)) {
            if (timer > 0f) {
                timer -= Time.deltaTime;
            } else {
                shoot = true;
            }
        } else {
            timer = holdTime;        }

        if (shoot) {
            transform.position += transform.forward * flySpeed * Time.deltaTime;
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            transform.Rotate(verticalInput * rotateSpeed * Time.deltaTime, horizontalInput * rotateSpeed * Time.deltaTime, 0, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Player")) {
            if (collider.GetComponent<NetworkObject>().IsOwner) {
                colliding = true;
            }
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Player")) {
            if (collider.GetComponent<NetworkObject>().IsOwner) {
                colliding = false;
            }
        }
    }
}