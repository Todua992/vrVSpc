using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class NetworkObjectPool : Singleton<NetworkObjectPool> {
    [SerializeField] NetworkManager m_NetworkManager;

    [SerializeField] List<PoolConfigObject> PooledPrefabsList;

    HashSet<GameObject> prefabs = new HashSet<GameObject>();

    Dictionary<GameObject, Queue<NetworkObject>> pooledObjects = new Dictionary<GameObject, Queue<NetworkObject>>();


    public void OnValidate() {
        for (var i = 0; i < PooledPrefabsList.Count; i++) {
            var prefab = PooledPrefabsList[i].Prefab;
            if (prefab != null) {
                Assert.IsNotNull(prefab.GetComponent<NetworkObject>(), $"{nameof(NetworkObjectPool)}: Pooled prefab \"{prefab.name}\" at index {i.ToString()} has no {nameof(NetworkObject)} component.");
            }
        }
    }

    public NetworkObject GetNetworkObject(GameObject prefab) {
        return GetNetworkObjectInternal(prefab, Vector3.zero, Quaternion.identity);
    }

    public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation) {
        return GetNetworkObjectInternal(prefab, position, rotation);
    }

    public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab) {
        var go = networkObject.gameObject;

        go.SetActive(false);
        pooledObjects[prefab].Enqueue(networkObject);
    }

    public void AddPrefab(GameObject prefab, int prewarmCount = 0) {
        var networkObject = prefab.GetComponent<NetworkObject>();

        Assert.IsNotNull(networkObject, $"{nameof(prefab)} must have {nameof(networkObject)} component.");
        Assert.IsFalse(prefabs.Contains(prefab), $"Prefab {prefab.name} is already registered in the pool.");

        RegisterPrefabInternal(prefab, prewarmCount);
    }

    private void RegisterPrefabInternal(GameObject prefab, int prewarmCount) {
        prefabs.Add(prefab);

        var prefabQueue = new Queue<NetworkObject>();
        pooledObjects[prefab] = prefabQueue;

        for (int i = 0; i < prewarmCount; i++) {
            var go = CreateInstance(prefab);
            ReturnNetworkObject(go.GetComponent<NetworkObject>(), prefab);
        }

        m_NetworkManager.PrefabHandler.AddHandler(prefab, new DummyPrefabInstanceHandler(prefab, this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private GameObject CreateInstance(GameObject prefab) {
        return Instantiate(prefab);
    }

    private NetworkObject GetNetworkObjectInternal(GameObject prefab, Vector3 position, Quaternion rotation) {
        var queue = pooledObjects[prefab];

        NetworkObject networkObject;
        if (queue.Count > 0) {
            networkObject = queue.Dequeue();
        } else {
            networkObject = CreateInstance(prefab).GetComponent<NetworkObject>();
        }

        var go = networkObject.gameObject;
        go.transform.SetParent(null);
        go.SetActive(true);

        go.transform.position = position;
        go.transform.rotation = rotation;

        return networkObject;
    }

    public void InitializePool() {
        foreach (var configObject in PooledPrefabsList) {
            RegisterPrefabInternal(configObject.Prefab, configObject.PrewarmCount);
        }
    }
}

[Serializable]
struct PoolConfigObject {
    public GameObject Prefab;
    public int PrewarmCount;
}

class DummyPrefabInstanceHandler : INetworkPrefabInstanceHandler {
    GameObject m_Prefab;
    NetworkObjectPool m_Pool;

    public DummyPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool) {
        m_Prefab = prefab;
        m_Pool = pool;
    }

    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation) {
        return m_Pool.GetNetworkObject(m_Prefab, position, rotation);
    }

    public void Destroy(NetworkObject networkObject) {
        m_Pool.ReturnNetworkObject(networkObject, m_Prefab);
    }
}