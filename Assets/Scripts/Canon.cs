using UnityEngine;

public class Canon : MonoBehaviour {
    [SerializeField] private GameObject Target;

    void Update() {   
        transform.LookAt(Target.transform);
    }
}