using Unity.Netcode;
using UnityEngine;

public class SelfDestruct : NetworkBehaviour {
    [SerializeField] private float timer;

    private void Update() {
        if (IsHost) {
            if (timer > 0f) {
                timer -= Time.deltaTime;
            } else {
                Destroy(gameObject);
            }
        }
    }
}