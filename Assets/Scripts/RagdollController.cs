using System.Threading.Tasks;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Rigidbody[] rb;
    private Animator animator;
    private bool isRagdool = false;
    [SerializeField] private int RegDollTime;
    [SerializeField] private Transform hips;



    void Start() {
        rb = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        DisableRagdoll();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            EnableRagdoll();
        }
    }

    public async void EnableRagdoll()
    {
        if (!isRagdool)
        {
            isRagdool = true;

            foreach (Rigidbody selected in rb)
            {
                selected.isKinematic = false;
                selected.detectCollisions = true;
            }

            animator.enabled = false;

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
    }
}