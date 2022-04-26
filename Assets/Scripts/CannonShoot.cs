using Unity.Netcode;
using UnityEngine;

public class CannonShoot : MonoBehaviour {
    [SerializeField] private float reloadTime;
    [SerializeField] private float shootSpeed;
    [SerializeField] private Transform cannonBallSpawn;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private ParticleSystem explosionVFX;

    [SerializeField] private NetworkVariable<bool> networkTimer = new NetworkVariable<bool>();
    [SerializeField] private NetworkVariable<bool> networkShooting = new NetworkVariable<bool>();

    private float timer = 0;
    private bool colliding = false;
    private bool shooting = false;

    private void Update() {
        if (networkTimer.Value) {
            timer = reloadTime;
            UpdateReloadTimeServerRcp(false);
        }
        
        if (timer > 0) {
            timer -= Time.deltaTime;
        } else if (colliding && Input.GetKeyDown(KeyCode.P)) {
            UpdateShootingServerRpc(true);
            UpdateReloadTimeServerRcp(true);
        }

        if (shooting != networkShooting.Value) {
            shooting = networkShooting.Value;
        }

        if (shooting == true) {
            UpdateShootingServerRpc(false);
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

    [ServerRpc]
    public void UpdateReloadTimeServerRcp(bool reloadTimer) {
        networkTimer.Value = reloadTimer;
    }

    [ServerRpc]
    public void UpdateShootingServerRpc(bool shooting) {
        networkShooting.Value = shooting;
    }
}