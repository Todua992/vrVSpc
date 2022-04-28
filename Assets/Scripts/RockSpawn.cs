using UnityEngine;
using System.Threading.Tasks;

public class RockSpawn : MonoBehaviour {
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private GameObject Prefab;
    [SerializeField] private int Maxrocks;
    [SerializeField] private float TimeTillSpawn;
    public int Currentrocks;
    private float CurrentTime;

    private void Update() {
        if (Currentrocks < Maxrocks) {
            CurrentTime -= Time.deltaTime;
            if (CurrentTime <= 0f) {
                Currentrocks++;
                Spawn();
                CurrentTime = TimeTillSpawn;
            }
        }
    }

    private void Spawn() {
        Instantiate(Prefab, SpawnPoint.position, SpawnPoint.rotation);
    }
}