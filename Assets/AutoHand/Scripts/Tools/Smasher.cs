using UnityEngine;
using UnityEngine.Events;

namespace Autohand.Demo
{
    public delegate void SmashEvent(Smasher smasher, Smash smashable);

    [RequireComponent(typeof(Rigidbody))]
    public class Smasher : MonoBehaviour
    {

        [SerializeField] private Material handMaterial;

        Rigidbody rb;
        [Header("Options")]
        public LayerMask smashableLayers;
        [Tooltip("How much to multiply the magnitude on smash")]
        public float forceMulti = 1;
        [Tooltip("Can be left empty - The center of mass point to calculate velocity magnitude - for example: the camera of the hammer is a better point vs the pivot center of the hammer object")]
        public Transform centerOfMassPoint;


        public float Upwordsblast;
        [SerializeField] private float Starttime;
        [SerializeField] private GameObject particle;
        private float Currenttime;
        [Header("Event")]
        public UnityEvent OnSmash;


        public SmashEvent OnSmashEvent;

        public bool blast = false;
        public float MinMag;
        public GameObject exp;
        public float expForce, radius;

        Vector3[] velocityOverTime = new Vector3[3];
        Vector3 lastPos;

        private void Start()
        {
            Currenttime = Starttime;
            rb = GetComponent<Rigidbody>();
            if (smashableLayers == 0)
                smashableLayers = LayerMask.GetMask(Hand.grabbableLayerNameDefault);

            OnSmashEvent += (smasher, smashable) => { OnSmash?.Invoke(); };
        }


        void FixedUpdate()
        {
            for (int i = 1; i < velocityOverTime.Length; i++)
            {
                velocityOverTime[i] = velocityOverTime[i - 1];
            }
            velocityOverTime[0] = lastPos - (centerOfMassPoint ? centerOfMassPoint.position : rb.position);

            lastPos = centerOfMassPoint ? centerOfMassPoint.position : rb.position;

            handMaterial.color = new Color(Map(Currenttime, 0f, Starttime, 0f, 1f), Map(Currenttime, 0f, Starttime, 0.5f, 0f), Map(Currenttime, 0f, Starttime, 1f, 0f));
            // Timer ####
            if (Currenttime > 0)
            {
                Currenttime -= Time.deltaTime;
            }
            if (Currenttime <= 0)
            {
                ParticleSystem first = particle.GetComponent<ParticleSystem>();
                first.Play();

            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            ParticleSystem first = particle.GetComponent<ParticleSystem>();

            Smash smash;
            if (blast == true)
            {
                if (GetMagnitude() > MinMag & Currenttime <= 0)
                {
                    knockBack();
                    Currenttime = Starttime;
                    first.Stop();
                }
            }

            if (collision.transform.CanGetComponent(out smash))
            {
                if (GetMagnitude() >= smash.smashForce)
                {
                    smash.DoSmash();
                    OnSmashEvent?.Invoke(this, smash);


                }
            }
        }


        float GetMagnitude()
        {
            Vector3 velocity = Vector3.zero;
            for (int i = 0; i < velocityOverTime.Length; i++)
            {
                velocity += velocityOverTime[i];
            }

            return (velocity.magnitude / velocityOverTime.Length) * forceMulti * 10;
        }
        void knockBack()
        {

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider nearyby in colliders)
            {
                Rigidbody rb = nearyby.GetComponent<Rigidbody>();
                RagdollController controller = nearyby.GetComponent<RagdollController>();



                if (controller != null)
                {
                    controller.EnableRagdoll();
                    nearyby.GetComponentInChildren<Rigidbody>().AddExplosionForce(expForce, transform.position, radius, Upwordsblast);
                }

                if (rb != null)
                {
                    rb.AddExplosionForce(expForce, transform.position, radius, Upwordsblast);
                }
            }
        }

        private float Map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
    }

}
