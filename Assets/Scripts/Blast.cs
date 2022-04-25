using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blast : MonoBehaviour
{
    
    public GameObject exp;
    public float expForce, radius;
    private void OnCollisionEnter(Collision other)
    {
        GameObject _exp = Instantiate(exp, transform.position, transform.rotation);
        knockBack();
        Destroy(_exp,3);
        
    }   
       

    void knockBack()
    {
        Rigidbody vrb = GetComponent<Rigidbody>();
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider nearyby in colliders)
            {

                Rigidbody rb = nearyby.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(expForce, transform.position, radius);
                }
            }
       
        
    }
}
