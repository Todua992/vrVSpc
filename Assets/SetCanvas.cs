using Unity.Netcode;
using UnityEngine;

public class SetCanvas : NetworkBehaviour {
    public override void OnNetworkSpawn() {
        if (IsHost) {
            Canvas canvas = GetComponent<Canvas>();

            canvas.worldCamera = GameObject.Find("Camera (head)").GetComponent<Camera>();
        }
    }
}