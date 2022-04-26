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

        if (colliding && timer <= 0f && Input.GetKeyDown(KeyCode.P)) {
            
            shooting = true;
            playerShoot.Shooting(true);

            timing = true;
            playerShoot.Timing(true);
        } else if (shooting == true) {
            shooting = false;
            playerShoot.Shooting(false);

            if (IsHost) {
                Shoot();
            }
        }
    }
    
    private void Shoot() {
        //explosionSound.Play();
        explosionVFX.Play();

        GameObject cannonBall = Instantiate(cannonBallPrefab, cannonBallSpawn.position, cannonBallSpawn.rotation);
        cannonBall.GetComponent<NetworkObject>().Spawn();
        cannonBall.GetComponent<Rigidbody>().AddForce(cannonBallSpawn.forward * shootSpeed, ForceMode.Impulse);
    }

    #region COLLISIONS

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

    #endregion

    #region CHECK SERVER CALLS

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

    #endregion

}