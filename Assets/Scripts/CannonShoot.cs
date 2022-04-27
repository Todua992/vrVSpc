using Unity.Netcode;
using UnityEngine;

public class CannonShoot : NetworkBehaviour {
    public int index = -1;
    public NetworkVariable<int> networkIndex = new NetworkVariable<int>();

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
        playerShoot = NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerShoot>();
        timer = holdTime;

        if (IsHost) {
            UpdateIndexServerRpc(index);
        }
    }

    private void Update() {
        print(playerShoot.networkIndex.Value);

        if (index != networkIndex.Value) {
            CheckIndexValue();
            return;
        }
        
        if (shoot == true) {
            Shoot();
        }

        if (colliding && Input.GetKey(KeyCode.E)) {
            if (timer > 0f) {
                timer -= Time.deltaTime;
            } else {
                playerShoot.UpdateNetworkValues(true, index);
            }
        } else if (timer != holdTime) {
            timer = holdTime;
        }

        CheckNetworkValues();
    }

    private void Shoot() {
        //explosionSound.Play();
        explosionVFX.Play();

        if (IsHost) {
            GameObject cannonBall = Instantiate(cannonBallPrefab, cannonBallSpawn.position, cannonBallSpawn.rotation);
            cannonBall.GetComponent<NetworkObject>().Spawn();
            cannonBall.GetComponent<Rigidbody>().AddForce(cannonBallSpawn.forward * shootSpeed, ForceMode.Impulse);
            cannonSpawn.RemoveCannon(transform.parent.gameObject);
            Destroy(transform.parent.gameObject);
        }
    }

    private void CheckNetworkValues() {
        print(playerShoot.networkShoot.Value + " : " + playerShoot.networkIndex.Value);

        if (shoot != playerShoot.networkShoot.Value && index == playerShoot.networkIndex.Value) {
            shoot = playerShoot.networkShoot.Value;
        }
    }

    private void CheckIndexValue() {
        if (index != networkIndex.Value) {
            index = networkIndex.Value; 
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

    [ServerRpc]
    public void UpdateIndexServerRpc(int index) {
        networkIndex.Value = index;
    }
}