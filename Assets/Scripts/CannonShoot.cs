using UnityEngine;

public class CannonShoot : MonoBehaviour {
    [SerializeField] private float shootSpeed;
    [SerializeField] private Transform cannonBallSpawn;
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

        Rigidbody cannonBall = Instantiate(cannonBallPrefab, cannonBallSpawn.position, cannonBallSpawn.rotation).GetComponent<Rigidbody>();
        cannonBall.AddForce(cannonBallSpawn.forward * shootSpeed, ForceMode.Impulse);
    }
}