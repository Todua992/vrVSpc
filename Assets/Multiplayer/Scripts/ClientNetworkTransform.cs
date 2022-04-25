using Unity.Netcode.Components;
using UnityEngine;

namespace Unity.Netcode.Samples {
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform {
        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            CanCommitToTransform = IsOwner;
        }

        protected override void Update() {
            base.Update();
            if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsListening)) {
                if (CanCommitToTransform) {
                    TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
                }
            }
        }
    }
}