using System;
using UnityEngine;

namespace MissileControll { 
public class MissileController : MonoBehaviour {
    [Header("REFERENCES")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private GameObject particle;
    [Header("AircraftControll")]
    [SerializeField] private float FlySpeed = 5;

    private bool launch = false;


    public float YawAmount = 120;
    public float PitchAmout = 120;


    private float Yaw;
    private float Pitch;
    private Quaternion Worldrotation;



    void Start() {
        Worldrotation = transform.rotation;
    }

    private void FixedUpdate() {



    }

        public void  MissileYaw() {
            float horizontalInput = Input.GetAxis("Horizontal");
            Yaw += horizontalInput * YawAmount * Time.deltaTime;
        }

        public void MissilePitch() {
            float verticalInput = Input.GetAxis("Vertical");

            //Axis

            Pitch += verticalInput * PitchAmout * Time.deltaTime;
        }

        public void Motion() {
            Worldrotation = Quaternion.Euler(Vector3.up * Yaw + Vector3.forward * Pitch);
        }


        public void MooveForward() {

            if (Input.GetKey("space")) {
                launch = true;
            }

            if (launch == true) {
                //Move Forward
                transform.position += transform.forward * FlySpeed * Time.deltaTime;

            }
        }



            private void OnCollisionEnter(Collision collision) {
        ParticleSystem first = particle.GetComponent<ParticleSystem>();
        first.Play();
        Destroy(gameObject);
    }
}
}
