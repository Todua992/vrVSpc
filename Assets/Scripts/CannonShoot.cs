using Unity.Netcode;
using UnityEngine;

public class CannonShoot : NetworkBehaviour {
    [SerializeField] private float reloadTime;
    [SerializeField] private float shootSpeed;
    [SerializeField] private Transform cannonBallSpawn;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private ParticleSystem explosionVFX;

    [SerializeField] private NetworkVariable<bool> networkColliding = new NetworkVariable<bool>();
    [SerializeField] private NetworkVariable<bool> networkShooting = new NetworkVariable<bool>();
    [SerializeField] private NetworkVariable<bool> networkTiming = new NetworkVariable<bool>();

    private float timer = 0;
    private bool colliding = false;
    private bool shooting = false;
    private bool timing = false;

    private void Update() {
        CheckColliding();
        CheckShooting();
        CheckTiming();        

        if (timer > 0) {
            timer -= Time.deltaTime;
        } else if (colliding && Input.GetKeyDown(KeyCode.P)) {
            
            shooting = true;
            UpdateShootingServerRpc(true);

            timing = true;
            UpdateTimingServerRpc(true);
        }

        if (shooting != networkShooting.Value) {
            shooting = networkShooting.Value;
        }

        if (shooting == true) {
            shooting = false;
            UpdateShootingServerRpc(false);

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
            colliding = true;
            UpdateCollidingServerRpc(true);
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Player")) {
            colliding = false;
            UpdateCollidingServerRpc(false);
        }
    }

    #endregion

    #region CHECK SERVER CALLS

    private void CheckColliding() {
        if (colliding != networkColliding.Value) {
            colliding = networkColliding.Value;
        }
    }

    private void CheckShooting() {
        if (shooting != networkShooting.Value) {
            shooting = networkShooting.Value;
        }
    }

    private void CheckTiming() {
        if (timing != networkTiming.Value) {
            timing = networkTiming.Value;
        }

        if (timing) {
            timer = reloadTime;

            timing = false;
            UpdateTimingServerRpc(false);
        }
    }

    #endregion

    #region SERVER CALLS

    [ServerRpc]
    public void UpdateCollidingServerRpc(bool colliding) {
        networkColliding.Value = colliding;
    }

    [ServerRpc]
    public void UpdateShootingServerRpc(bool shooting) {
        networkShooting.Value = shooting;
    }

    [ServerRpc]
    public void UpdateTimingServerRpc(bool timing) {
        networkTiming.Value = timing;
    }

    #endregion
}