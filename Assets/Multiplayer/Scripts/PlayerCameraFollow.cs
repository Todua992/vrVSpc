using Cinemachine;
using UnityEngine;

public class PlayerCameraFollow : Singleton<PlayerCameraFollow> {
    [SerializeField] private float amplitudeGain = 0.5f;
    [SerializeField] private float frequencyGain = 0.5f;
    [SerializeField] private Vector3 cameraPosition;

    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake() {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void FollowPlayer(Transform transform) {
        if (cinemachineVirtualCamera == null) {
            return;
        }

        transform.position += cameraPosition;

        cinemachineVirtualCamera.Follow = transform;

        var perlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = amplitudeGain;
        perlin.m_FrequencyGain = frequencyGain;
    }
}