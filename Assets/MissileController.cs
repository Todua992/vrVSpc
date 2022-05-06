using Unity.Netcode;
using UnityEngine;

public class MissileController : NetworkBehaviour {
    [SerializeField] private float flySpeed;
    [SerializeField] private float rotateSpeed;
    private bool launch = false;

    private int mounted = 0;


    private void Update() {
        if (mounted == 2) {
            launch = true;
        }

        if (launch) {
            transform.position += transform.forward * flySpeed * Time.deltaTime;
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            transform.Rotate(verticalInput * rotateSpeed * Time.deltaTime, horizontalInput * rotateSpeed * Time.deltaTime, 0, Space.Self);
        }
    }
}