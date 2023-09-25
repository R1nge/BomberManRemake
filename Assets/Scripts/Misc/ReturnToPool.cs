using Game;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class ReturnToPool : NetworkBehaviour
    {
        [SerializeField] private string prefabName;
        [SerializeField] private float delay;
        private NetworkObjectPool _networkObjectPool;

        [Inject]
        private void Inject(NetworkObjectPool networkObjectPool)
        {
            _networkObjectPool = networkObjectPool;
        }

        private void OnEnable()
        {
            if (!IsServer) return;
            Invoke(nameof(Return), delay);
        }

        private void Return() => _networkObjectPool.ReturnNetworkObject(NetworkObject, prefabName);
    }
}