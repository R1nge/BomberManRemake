using System;
using System.Collections.Generic;
using ModestTree;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace Game
{
    public class NetworkObjectPool : NetworkBehaviour
    {
        [SerializeField] private List<PoolConfigObject> pooledPrefabsList;
        private readonly HashSet<GameObject> _prefabs = new();
        private readonly Dictionary<string, ObjectPool<NetworkObject>> _pooledObjects = new();
        private DiContainer _diContainer;

        [Inject]
        private void Inject(DiContainer diContainer) => _diContainer = diContainer;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            foreach (var configObject in pooledPrefabsList)
            {
                RegisterPrefabInternal(configObject.prefab, configObject.prewarmCount);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;

            foreach (var prefab in _prefabs)
            {
                NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
                _pooledObjects[prefab.name].Clear();
            }

            _pooledObjects.Clear();
            _prefabs.Clear();
        }

        public void OnValidate()
        {
            for (var i = 0; i < pooledPrefabsList.Count; i++)
            {
                var prefab = pooledPrefabsList[i].prefab;
                if (prefab != null)
                {
                    Assert.IsNotNull(prefab.GetComponent<NetworkObject>(),
                        $"{nameof(NetworkObjectPool)}: Pooled prefab \"{prefab.name}\" at index {i.ToString()} has no {nameof(NetworkObject)} component.");
                }
            }
        }

        public NetworkObject GetNetworkObject(string prefab, Vector3 position, Quaternion rotation)
        {
            var networkObject = _pooledObjects[prefab].Get();

            var noTransform = networkObject.transform;
            noTransform.position = position;
            noTransform.rotation = rotation;

            SetPositionClientRpc(networkObject, position);

            return networkObject;
        }

        [ClientRpc]
        private void SetPositionClientRpc(NetworkObjectReference networkObjectReference, Vector3 position)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                networkObject.gameObject.transform.transform.position = position;
            }
        }

        public void ReturnNetworkObject(NetworkObject networkObject, string prefab)
        {
            _pooledObjects[prefab].Release(networkObject);
        }

        void RegisterPrefabInternal(GameObject prefab, int prewarmCount)
        {
            NetworkObject CreateFunc()
            {
                var instance = _diContainer.InstantiatePrefab(prefab).GetComponent<NetworkObject>();
                instance.Spawn(true);
                return instance;
            }

            void ActionOnGet(NetworkObject networkObject)
            {
                GetClientRpc(networkObject);
            }

            void ActionOnRelease(NetworkObject networkObject)
            {
                ReleaseClientRpc(networkObject);
            }

            void ActionOnDestroy(NetworkObject networkObject)
            {
                Destroy(networkObject.gameObject);
            }

            _prefabs.Add(prefab);

            // Create the pool
            _pooledObjects[prefab.name] = new ObjectPool<NetworkObject>(CreateFunc, ActionOnGet, ActionOnRelease,
                ActionOnDestroy, defaultCapacity: prewarmCount);

            // Populate the pool
            var prewarmNetworkObjects = new List<NetworkObject>();
            for (var i = 0; i < prewarmCount; i++)
            {
                prewarmNetworkObjects.Add(_pooledObjects[prefab.name].Get());
            }

            foreach (var networkObject in prewarmNetworkObjects)
            {
                _pooledObjects[prefab.name].Release(networkObject);
            }

            // Register Netcode Spawn handlers
            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(prefab, this));
        }

        [ClientRpc]
        private void ReleaseClientRpc(NetworkObjectReference networkObjectReference)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                networkObject.gameObject.SetActive(false);
            }
        }

        [ClientRpc]
        private void GetClientRpc(NetworkObjectReference networkObjectReference)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                networkObject.gameObject.SetActive(true);
            }
        }
    }

    [Serializable]
    struct PoolConfigObject
    {
        public GameObject prefab;
        public int prewarmCount;
    }

    class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
    {
        private readonly GameObject _prefab;
        private readonly NetworkObjectPool _pool;

        public PooledPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool)
        {
            _prefab = prefab;
            _pool = pool;
        }

        NetworkObject INetworkPrefabInstanceHandler.Instantiate(ulong ownerClientId, Vector3 position,
            Quaternion rotation)
        {
            return _pool.GetNetworkObject(_prefab.name, position, rotation);
        }

        void INetworkPrefabInstanceHandler.Destroy(NetworkObject networkObject)
        {
            _pool.ReturnNetworkObject(networkObject, _prefab.name);
        }
    }
}