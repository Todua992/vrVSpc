using System.Threading.Tasks;
using UnityEngine;

public class RagdollController : MonoBehaviour {
    private Rigidbody[] rb;
    private Animator animator;
    private bool isRagdool = false;

    [SerializeField] private Transform hips;

    

    void Start() {
        rb = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        DisableRagdoll();
    }

    public async void EnableRagdoll() {
        if (!isRagdool) {
            isRagdool = true;

            foreach (Rigidbody selected in rb)
            {
                selected.isKinematic = false;
                selected.detectCollisions = true;
            }

            animator.enabled = false;

            await Task.Delay(2000);

            DisableRagdoll();
        }
    }

    private void DisableRagdoll() {
        isRagdool = false;

        foreach (Rigidbody selected in rb) {
            selected.isKinematic = true;
            selected.detectCollisions = false;
            
        }

        transform.position = hips.position;
        animator.enabled = true;
    }
}