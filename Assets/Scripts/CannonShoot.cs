using Unity.Netcode;
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
        //explosionSound.Play();
        explosionVFX.Play();

        GameObject cannonBall = Instantiate(cannonBallPrefab, cannonBallSpawn.position, cannonBallSpawn.rotation);
        cannonBall.GetComponent<NetworkObject>().Spawn();
        cannonBall.GetComponent<Rigidbody>().AddForce(cannonBallSpawn.forward * shootSpeed, ForceMode.Impulse);
    }
}