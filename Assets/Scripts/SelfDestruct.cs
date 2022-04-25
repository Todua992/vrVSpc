using UnityEngine;

public class SelfDestruct : MonoBehaviour {
    [SerializeField] private float timer;

    private void Update() {
        if (timer > 0f) {
            timer -= Time.deltaTime;
        } else {
            Destroy(gameObject);
        }
    }
}