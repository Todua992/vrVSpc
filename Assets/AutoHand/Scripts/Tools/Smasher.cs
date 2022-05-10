using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using Unity.Netcode;



namespace Autohand.Demo {
    public delegate void SmashEvent(Smasher smasher, Smash smashable);
    
    [RequireComponent(typeof(Rigidbody))]
    public class Smasher : NetworkBehaviour {
        public NetworkVariable<bool> networkDestory = new();
        public NetworkVariable<float> networkTimer = new();
        public NetworkVariable<Vector3> networkForce = new();

        private Vector3 oldForce;

        [Header("Test")]

        public HandBase handbase;

        public XRHandControllerLink link;
        public CommonButton button;

        [SerializeField] private Material handMaterial;
        [HideInInspector] public RockSpawn rock;
        Rigidbody rb;
        [Header("Options")]
        public LayerMask smashableLayers;
        [Tooltip("How much to multiply the magnitude on smash")]
        public float forceMulti = 1;
        [Tooltip("Can be left empty - The center of mass point to calculate velocity magnitude - for example: the camera of the hammer is a better point vs the pivot center of the hammer object")]
        public Transform centerOfMassPoint;


        private InputDevice targetDevice;
       
        
        [SerializeField] private float Starttime;
        
        
        private float Currenttime;

        [Header("Force and Velocity")]
        public float MinMag;

        [Header("Fracture")]
        [SerializeField] bool CanExplode = false;
        [SerializeField] private GameObject fractured;

 

        [Header("Blast")]
        public bool blast = false;
        public float Upwordsblast;
        [SerializeField] private GameObject particle;
        
        
        public GameObject exp;
        public float expForce, radius;

        [Header("Event")]
        public UnityEvent OnSmash;
        public SmashEvent OnSmashEvent;

        Vector3[] velocityOverTime = new Vector3[3];
        Vector3 lastPos;

        private void Start() {

            handbase = GetComponent<HandBase>();

            rock = FindObjectOfType<RockSpawn>();

            Currenttime = Starttime;
            rb = GetComponent<Rigidbody>();
            if (smashableLayers == 0)
                smashableLayers = LayerMask.GetMask(Hand.grabbableLayerNameDefault);

            OnSmashEvent += (smasher, smashable) => { OnSmash?.Invoke(); };
        }


        void FixedUpdate() {


            targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue);

            if (gripValue > 0f) {
                Debug.Log(gripValue);
            }

            for (int i = 1; i < velocityOverTime.Length; i++) {
                velocityOverTime[i] = velocityOverTime[i - 1];
            }
            velocityOverTime[0] = lastPos - (centerOfMassPoint ? centerOfMassPoint.position : rb.position);

            lastPos = centerOfMassPoint ? centerOfMassPoint.position : rb.position;

            handMaterial.color = new Color(Map(Currenttime, 0f, Starttime, 0f, 1f), Map(Currenttime, 0f, Starttime, 0.5f, 0f), Map(Currenttime, 0f, Starttime, 1f, 0f));


            // Timer ####
            if (IsHost) {
                if (Currenttime > 0) {
                    Currenttime -= Time.deltaTime;
                    UpdateTimerServerRpc(Currenttime);
                }
            } if (!IsHost) {
                Currenttime = networkTimer.Value;
            }

            if (blast == true) {
                if (Currenttime <= 0) {
                    ParticleSystem first = particle.GetComponent<ParticleSystem>();
                    first.Play();

                }
            }

            if (networkDestory.Value) {
                destroy();
            }

            if (!IsHost) {
                if (networkForce.Value != oldForce) {
                    oldForce = networkForce.Value;

                    knockBack(networkForce.Value);
                }
            }
        }


        private void OnCollisionStay(Collision collision) {
            //Blast Function
            if (blast == true) {
                ParticleSystem first = particle.GetComponent<ParticleSystem>();
                if (handbase.holdingObj == null && IsHost) {
                    if (GetMagnitude() > MinMag & Currenttime <= 0 && link.ButtonPressed(button)) {
                        knockBack(transform.position);
                        Currenttime = Starttime;
                        first.Stop();
                        UpdateForceServerRpc(transform.position);
                    }
                    else {
                    Vector3 velocity = Vector3.zero;
                    }
                }


                //For smashing Items
                Smash smash;
                if (collision.transform.CanGetComponent(out smash)) {

                    if (GetMagnitude() >= smash.smashForce) {
                        smash.DoSmash();
                        OnSmashEvent?.Invoke(this, smash);
                    }
                }
            }

            if (IsHost) {
                if (CanExplode == true && collision.gameObject.layer != 17 && collision.gameObject.layer != 30) {
                    if (GetMagnitude() > MinMag) {
                        destroy();
                        UpdateDestoryServerRpc();
                    }
                }
            }
        }
        float GetMagnitude() {
            Vector3 velocity = Vector3.zero;
            for (int i = 0; i < velocityOverTime.Length; i++) {
                velocity += velocityOverTime[i];
            }

            return (velocity.magnitude / velocityOverTime.Length) * forceMulti * 10;
        }

       void knockBack(Vector3 force) {
            Collider[] colliders = Physics.OverlapSphere(force, radius);
            foreach (Collider nearyby in colliders) {
                Rigidbody rb = nearyby.GetComponent<Rigidbody>();
                RagdollController controller = nearyby.GetComponent<RagdollController>();

                if (controller != null) {
                    controller.EnableRagdoll();

                    Debug.Log(nearyby.transform.Find("mixamorig:Hips"));

                    nearyby.transform.Find("mixamorig:Hips").GetComponent<Rigidbody>().AddExplosionForce(expForce * 3f, force, radius, Upwordsblastf, ForceMode.Impulse);
                }

                if (rb != null) {
                    rb.AddExplosionForce(expForce, force, radius, Upwordsblast);
                }
            }
        }

        void CopyVelocity(Rigidbody from, Rigidbody to) {
            to.velocity = from.velocity;
        }

        void destroy() {
            rock.Currentrocks--;
            GameObject frac = Instantiate(fractured, transform.position, transform.rotation);
            frac.GetComponent<NetworkObject>().Spawn();
            foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>()){
                CopyVelocity(this.rb, rb);
            }
            
            Destroy(gameObject);
        }

        private float Map(float s, float a1, float a2, float b1, float b2) {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        [ServerRpc]
        public void UpdateForceServerRpc(Vector3 force) {
            networkForce.Value = force;
        }

        [ServerRpc]
        public void UpdateTimerServerRpc(float currentTime) {
            networkTimer.Value = currentTime;
        }

        [ServerRpc]
        public void UpdateDestoryServerRpc() {
            networkDestory.Value = true;
        }
    }

}
