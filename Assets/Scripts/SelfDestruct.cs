using UnityEngine;

public class SelfDestruct : MonoBehaviour {
    [SerializeField] private bool onCollisionDestroy = true;

    [SerializeField] private float timer;

    private void OnCollisionEnter(Collision collision) {
        if (onCollisionDestroy == true) {
            Destroy(gameObject);

        }
    }

    public void Update() {
        if (timer > 0f) {
            timer -= Time.deltaTime;

            if (timer <= 0f) {
                Destroy(gameObject);
            }
        }
    }
}