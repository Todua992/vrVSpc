using Unity.Netcode;
using UnityEngine;

public class SetCanvas : NetworkBehaviour {
    [SerializeField] private bool test;

    public override void OnNetworkSpawn() {
        if (IsHost) {
            if (!test) {
                Canvas canvas = GetComponent<Canvas>();

                canvas.enabled = true;


                canvas.worldCamera = GameObject.Find("Camera (head)").GetComponent<Camera>();
            }
        }
    }
}