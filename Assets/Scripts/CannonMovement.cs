using UnityEngine;

public class CannonMovement : MonoBehaviour {
    [SerializeField] private GameObject target;

    private void Update() => transform.LookAt(target.transform);
}