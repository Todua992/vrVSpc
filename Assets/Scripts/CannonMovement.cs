using UnityEngine;

public class CannonMovement : MonoBehaviour {
    [SerializeField] private GameObject target;

    private void Start() => target = GameObject.Find("Target (Temporary)");   

    private void Update() => transform.LookAt(target.transform);
}