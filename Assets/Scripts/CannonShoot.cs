using Unity.Netcode;
using UnityEngine;

public class CannonShoot : NetworkBehaviour {
    [SerializeField] private float holdTime;
    [SerializeField] private float shootSpeed;
    [SerializeField] private Transform cannonBallSpawn;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private ParticleSystem explosionVFX;

    private bool colliding = false;
    private bool shoot = false;
    private float timer;
    
    [HideInInspector] public CannonSpawn cannonSpawn;
    private PlayerShoot playerShoot;

    private void Start() {       
        playerShoot = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.GetComponent<PlayerShoot>();
        timer = holdTime;
    }

    private void Update() {
        if (shoot == true) {
            Shoot();
        }

        if (colliding && Input.GetKeyDown(KeyCode.E)) {
            if (timer > 0f) {
                timer -= Time.deltaTime;
            } else {
                playerShoot.UpdateNetowrkValues(true);
            }
        } else if (timer != holdTime) {
            timer = holdTime;
        }

        CheckNetworkValues();
    }

    private void Shoot() {
        if (IsHost) {
            GameObject cannonBall = Instantiate(cannonBallPrefab, cannonBallSpawn.position, cannonBallSpawn.rotation);
            cannonBall.GetComponent<NetworkObject>().Spawn();
            cannonBall.GetComponent<Rigidbody>().AddForce(cannonBallSpawn.forward * shootSpeed, ForceMode.Impulse);
        }

        //explosionSound.Play();
        explosionVFX.Play();
        cannonSpawn.RemoveCannon(gameObject);
        Destroy(gameObject);
    }

    private void CheckNetworkValues() {
        if (shoot != playerShoot.networkShoot.Value) {
            shoot = playerShoot.networkShoot.Value;
        }
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