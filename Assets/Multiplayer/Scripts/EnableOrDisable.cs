using Unity.Netcode;
using UnityEngine;

public class EnableOrDisable : NetworkBehaviour {
    [SerializeField] private bool enable;
    [SerializeField] private Camera cam = null;
    [SerializeField] private bool onlyHost;

    public override void OnNetworkSpawn() {
        if (!IsHost && !onlyHost) {
            if (cam == null) {
                gameObject.SetActive(enable);
            } else {
                cam.enabled = enable;
                cam.gameObject.GetComponent<AudioListener>().enabled = enable;
            }
        } else if (IsHost && onlyHost) {
            if (cam == null) {
                gameObject.SetActive(enable);
            }
        }
    }
}