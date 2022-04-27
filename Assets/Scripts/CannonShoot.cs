using Unity.Netcode;
using UnityEngine;

public class CannonShoot : NetworkBehaviour {
    [SerializeField] private float reloadTime;
    [SerializeField] private float shootSpeed;
    [SerializeField] private Transform cannonBallSpawn;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private ParticleSystem explosionVFX;
    
    private float timer = 0;
    private bool colliding = false;
    private bool shooting = false;
    private bool timing = false;

    private PlayerShoot playerShoot;

    private void Update() {
        if (timer > 0) {
            timer -= Time.deltaTime;
        }

        if (playerShoot == null) {
            return;
        }
        
        CheckColliding();
        CheckShooting();
        CheckTiming();        

        if (colliding && Input.GetKeyDown(KeyCode.P)) {
            playerShoot.Shooting(true);
            playerShoot.Timing(true);
        }

        if (shooting != playerShoot.networkShooting.Value) {
            shooting = playerShoot.networkShooting.Value;
        }
    }
    
    private void Shoot() {
        print("Shooting");

        //explosionSound.Play();
        explosionVFX.Play();

        GameObject cannonBall = Instantiate(cannonBallPrefab, cannonBallSpawn.position, cannonBallSpawn.rotation);
        cannonBall.GetComponent<NetworkObject>().Spawn();
        cannonBall.GetComponent<Rigidbody>().AddForce(cannonBallSpawn.forward * shootSpeed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Player")) {
            playerShoot = collider.GetComponent<PlayerShoot>();
            
            colliding = true;
            playerShoot.Colliding(true);
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Player")) {
            colliding = false;
            playerShoot.Colliding(false);

            playerShoot = null;
        }
    }

    private void CheckColliding() {
        if (colliding != playerShoot.networkColliding.Value) {
            colliding = playerShoot.networkColliding.Value;
        }
    }

    private void CheckShooting() {
        if (shooting != playerShoot.networkShooting.Value) {
            shooting = playerShoot.networkShooting.Value;
        }
    }

    private void CheckTiming() {
        if (timing != playerShoot.networkTiming.Value) {
            timing = playerShoot.networkTiming.Value;
        }

        if (timing) {
            timer = reloadTime;

            timing = false;
            playerShoot.Timing(false);
        }
    }
}