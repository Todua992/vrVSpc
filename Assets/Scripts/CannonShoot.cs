using Unity.Netcode;
using UnityEngine;

public class CannonShoot : MonoBehaviour {
    [SerializeField] private float shootSpeed;
    [SerializeField] private Transform cannonBallSpawn;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private ParticleSystem explosionVFX;

    private bool colliding = false;

    private void Update() {
        if (colliding && Input.GetKeyDown(KeyCode.P)) {
            Shoot();
        }
    }
    
    private void Shoot() {
        //explosionSound.Play();
        explosionVFX.Play();

        GameObject cannonBall = Instantiate(cannonBallPrefab, cannonBallSpawn.position, cannonBallSpawn.rotation);
        cannonBall.GetComponent<NetworkObject>().Spawn();
        cannonBall.GetComponent<Rigidbody>().AddForce(cannonBallSpawn.forward * shootSpeed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Player")) {
            colliding = true;
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Player")) {
            colliding = false;
        }
    }
}