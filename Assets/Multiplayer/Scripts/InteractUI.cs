using Unity.Netcode;
using UnityEngine;

public class InteractUI : NetworkBehaviour {
    [SerializeField] private GameObject interactUI;

    private GameObject interactObject;
    private bool isActive;
    private bool oldIsActive;
    
    private void Start() {
        interactUI = GameObject.Find("Canvas").transform.Find("Interact").gameObject;
    }

    private void Update() {
        if (interactObject = null) {
            isActive = false;
        }

        if (isActive != oldIsActive) {
            interactUI.SetActive(isActive);
            oldIsActive = isActive;
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Airdrop")) {
            interactObject = collider.gameObject;

            Debug.Log(collider.gameObject);

            if (IsOwner) {
                isActive = true;
            }
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Airdrop")) {
            if (IsOwner) {
                isActive = false;
            }
        }
    }
}
