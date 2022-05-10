using Unity.Netcode;
using UnityEngine;

public class SelfDestruct : NetworkBehaviour {
    [SerializeField] private float timer;
    [SerializeField] private DestroyHead destroyHead;

    private void Start() {
        destroyHead = GameObject.Find("Camera (head)").GetComponent<DestroyHead>();    
    }

    private void Update() {
        if (IsHost) {
            if (timer > 0f) {
                timer -= Time.deltaTime;
            } else {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (IsHost) {
            if (collision.transform.CompareTag("MainCamera")) {
                destroyHead.DestroyPartHost(transform.position);
            }

            Destroy(gameObject);
        }
    }
}