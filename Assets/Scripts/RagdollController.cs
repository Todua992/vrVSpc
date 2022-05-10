using System.Threading.Tasks;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Rigidbody[] rb;
    private Animator animator;
    private bool isRagdool = false;
    [SerializeField] private int RegDollTime;
    [SerializeField] private Transform hips;
    [SerializeField] private PlayerControlAuthorative controller;



    void Start() {
        rb = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        DisableRagdoll();
    }

    public async void EnableRagdoll(Collider collider, float expForce, Vector3 force, float radius, float Upwordsblast)
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

            await Task.Delay(10);

            collider.GetComponentInChildren<Rigidbody>().AddExplosionForce(expForce, force, radius, Upwordsblast);


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

        transform.position = hips.position;
        animator.enabled = true;
        controller.enabled = true;
    }
}