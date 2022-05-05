using UnityEngine;

public class CannonMovement : MonoBehaviour {
    [SerializeField] private GameObject target;
    private bool track = false;
    private Rigidbody rb;
    private GameObject childObj;
    public float time = 3f;
    private bool timer = true;
    public float speed = 1f;
    public Quaternion targetRotation;
    private void Start() {
        speed = 1f;
        target = GameObject.Find("Camera (head)");
        rb = GetComponent<Rigidbody>();
        childObj = GameObject.Find("Cloud");
        rb.drag = 30;
    }

    private void Update() {

        if (track == true) {
            targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime );
        }

    }

    private void OnTriggerStay(Collider collider) {
        if (timer == true) {
            time -= Time.deltaTime;
            if (time <= 0) {
                childObj.SetActive(false);
                rb.drag = 1;
                track = true;
                timer = false;
            }
        }
    }


   




}