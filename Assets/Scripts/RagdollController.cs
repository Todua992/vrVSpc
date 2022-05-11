using Unity.Netcode;
using System.Threading.Tasks;
using UnityEngine;

public class RagdollController : NetworkBehaviour
{
    private Rigidbody[] rb;
    private Animator animator;
    public bool isRagdool = false;
    [SerializeField] private int RegDollTime;
    [SerializeField] private Transform hips;
    [SerializeField] private PlayerControlAuthorative controller;



    void Start() {
        rb = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        DisableRagdoll();
    }



    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.layer == 14 || collision.gameObject.tag == " Stone") {
            EnableRagdoll();
        }
    }

    public async void EnableRagdoll()
    {
        if (!isRagdool) {
            isRagdool = true;

            foreach (Rigidbody selected in rb)
            {
                selected.isKinematic = false;
                selected.detectCollisions = true;
            }

            animator.enabled = false;
            controller.enabled = false;


            await Task.Delay(RegDollTime);

            DisableRagdoll();
        }
    }

    private void DisableRagdoll()
    {
        
        isRagdool = false;

        foreach (Rigidbody selected in rb)
        {
            selected.isKinematic = true;
            selected.detectCollisions = false;

        }

        if (IsOwner) {
            transform.position = hips.position;
        }      

        
        animator.enabled = true;
        controller.enabled = true;
    }
}