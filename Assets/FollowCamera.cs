using UnityEngine;

public class FollowCamera : MonoBehaviour {
    [SerializeField] private Transform follow;

    private void Update() {
        transform.position = follow.position;
    }
}
