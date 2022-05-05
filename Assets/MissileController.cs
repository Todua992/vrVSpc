using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour {
    [SerializeField] private float flySpeed;
    [SerializeField] private float rotateSpeed;
    private bool launch = false;

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Space)) {
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