using Unity.Netcode;
using UnityEngine;

public class InteractUI : NetworkBehaviour {
    [SerializeField] private GameObject interactUI;

    private GameObject interactObject;
    private bool isActive;
    private bool oldIsActive;
    
    private void Start() {
        Debug.Log(GameObject.Find("Canvas"));

        interactUI = GameObject.Find("Canvas").transform.Find("Interact").gameObject;

        

        interactObject = null;
    }

    private void Update() {
        if (IsOwner) {
            if (interactObject == null) {
                isActive = false;
            }

            if (isActive != oldIsActive) {
                interactUI.SetActive(isActive);
                oldIsActive = isActive;
            }
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Airdrop") && IsOwner) {
            if (interactObject == null) {
                interactObject = collider.gameObject;
            }

            isActive = true;
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Airdrop") && IsOwner) {
            isActive = false;
        }
    }
}
