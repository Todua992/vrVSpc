using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MissileController : NetworkBehaviour {
    [SerializeField] private float holdTime;
    [SerializeField] private float flySpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private int index;
    [SerializeField] private Transform hit;

    private bool shoot;
    private bool colliding;
    private float timer;
    private bool networkShoot;

    private List<PlayerRocket> playerRockets = new();

    private DestroyHead destroyHead;

    private void Update() {
        if (colliding && Input.GetKey(KeyCode.E)) {
            if (timer > 0f) {
                timer -= Time.deltaTime;
            } else {
                shoot = true;
            }
        } else {
            timer = holdTime;        
        }

        if (networkShoot) {
            Shoot();
        }

        if (shoot && !networkShoot) {
            foreach (PlayerRocket selected in playerRockets) {
                selected.UpdateShoot(true, index);
            }
        }

        if (shoot) {
            float horizontal = 0f;
            float vertical = 0f;

            foreach (PlayerRocket selected in playerRockets) {
                if (Input.GetKey(KeyCode.UpArrow)) {
                    vertical = -1f;
                } else if (Input.GetKey(KeyCode.DownArrow)) {
                    vertical = 1f;
                }

                if(Input.GetKey(KeyCode.LeftArrow)) {
                    horizontal = -1f;
                } else if (Input.GetKey(KeyCode.RightArrow)) {
                    horizontal = 1f;
                }

                selected.UpdateInput(horizontal, vertical);
            }
        }

        if (!networkShoot) {
            CheckNetworkValues();
        }
    }

    private void CheckNetworkValues() {
        foreach (PlayerRocket selceted in playerRockets) {
            if (networkShoot != selceted.networkShoot.Value && index == selceted.networkIndex.Value) {
                destroyHead = GameObject.Find("Camera (head)").GetComponent<DestroyHead>();
                networkShoot = selceted.networkShoot.Value;
            }
        }
    }

    private void Shoot() {
        if (IsHost) {
            transform.position += transform.forward * flySpeed * Time.deltaTime;
            float verticalInput = 0f;
            float horizontalInput = 0f;


            foreach (PlayerRocket selected in playerRockets) {
                verticalInput = selected.networkVertical.Value;
                horizontalInput = selected.networkHorizontal.Value;
            }

            transform.Rotate(verticalInput * rotateSpeed * Time.deltaTime, horizontalInput * rotateSpeed * Time.deltaTime, 0, Space.Self);
        }
    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Player")) {
            playerRockets.Add(collider.GetComponent<PlayerRocket>());

            if (collider.GetComponent<NetworkObject>().IsOwner) {
                colliding = true;
            }
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Player") && !networkShoot) {
            playerRockets.Remove(collider.GetComponent<PlayerRocket>());

            if (collider.GetComponent<NetworkObject>().IsOwner) {
                colliding = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (IsHost) {
            if (collision.transform.CompareTag("Head")) {
                destroyHead.DestroyPartHost(hit.position);
                Destroy(gameObject);
            }
        }
    }
}