using Unity.Netcode;
using UnityEngine;

public class SetCanvas : NetworkBehaviour {
    public override void OnNetworkSpawn() {
        if (IsHost) {
            Canvas canvas = GetComponent<Canvas>();

            canvas.enabled = true;

            canvas.worldCamera = GameObject.Find("Camera (head)").GetComponent<Camera>();
        }
    }
}