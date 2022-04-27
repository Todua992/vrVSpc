using UnityEngine;

public class TreeRegrow : MonoBehaviour
{
    [SerializeField] private float Scaleratio;

    private Rigidbody rb;

    private Vector3 currentScaleSpeed;
    private Vector3 Startposition;
    private Vector3 StartScale;

    private Quaternion StartRotation;
    
    private bool newborn;
    
    void Start()
    {
        currentScaleSpeed = new Vector3 (currentScaleSpeed.x = Scaleratio, currentScaleSpeed.y = Scaleratio, currentScaleSpeed.z = Scaleratio);
        newborn = false;
    rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        StartScale = transform.localScale;
        Startposition = transform.position;
        StartRotation = transform.rotation;

    }

    void FixedUpdate()
    {
        if (transform.position.y <= -10)
        {
            Givebirth();
        }

        if(newborn == true)
        {
            transform.localScale = transform.localScale + currentScaleSpeed * Time.deltaTime;
            if ((StartScale.x - transform.localScale.x) < 0)
            {
                newborn=false;
            }
        }
        
    }



    void Givebirth()
    {
        transform.position = Startposition;
        transform.rotation = StartRotation;
        rb.isKinematic = true;
        transform.localScale = new Vector3(0, 0, 0);
        newborn = true;
    }


}
    