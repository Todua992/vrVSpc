using UnityEngine;

public class CannonShoot : MonoBehaviour {
    [SerializeField] private float shootSpeed;
    [SerializeField] private Transform spawn;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private ParticleSystem explosionVFX;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            Shoot();
        }
    }

    private void Shoot() {
        explosionSound.Play();
        explosionVFX.Play();

        Rigidbody cannonBall = Instantiate(cannonBallPrefab, spawn.position, spawn.rotation).GetComponent<Rigidbody>();
        cannonBall.AddForce(spawn.forward * shootSpeed, ForceMode.Impulse);
    }
}