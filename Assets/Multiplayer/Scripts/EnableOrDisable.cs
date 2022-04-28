using Unity.Netcode;
using UnityEngine;

public class EnableOrDisable : NetworkBehaviour {
    [SerializeField] private bool enable;
    [SerializeField] private Camera cam = null;

    public override void OnNetworkSpawn() {
        if (!IsHost) {
            if (cam == null) {
                gameObject.SetActive(enable);
            } else {
                cam.enabled = enable;
                cam.gameObject.GetComponent<AudioListener>().enabled = enable; 
            }
        }
    }
}