using System;
using Game;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerBlockController : NetworkBehaviour
    {
        public event Action<int> OnDigAmountChanged;
        [SerializeField] private new Transform camera;
        [SerializeField] private int distance;
        [SerializeField] private int digAmount;
        private NetworkVariable<int> _currentDigAmount;

        private void Awake()
        {
            _currentDigAmount = new NetworkVariable<int>(digAmount);
            _currentDigAmount.OnValueChanged += DigAmountChanged;
        }

        private void Start() => OnDigAmountChanged?.Invoke(_currentDigAmount.Value);

        private void DigAmountChanged(int _, int amount) => OnDigAmountChanged?.Invoke(amount);

        public void OnDestroyBlock()
        {
            if (!IsOwner) return;
            Raycast();
        }

        private void Raycast()
        {
            Ray ray = new Ray(camera.position, camera.forward);
            if (Physics.Raycast(ray, out var hit, distance))
            {
                if (hit.transform.TryGetComponent(out Destructable destructable))
                {
                    DestroyBlockServerRpc(destructable.gameObject);
                }
            }
        }

        [ServerRpc]
        private void DestroyBlockServerRpc(NetworkObjectReference destructableRef)
        {
            if (_currentDigAmount.Value > 0)
            {
                if (destructableRef.TryGet(out NetworkObject networkObject))
                {
                    if (networkObject.TryGetComponent(out Destructable destructable))
                    {
                        destructable.SpawnDropServerRpc();
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseDigServerRpc(int amount) => _currentDigAmount.Value += amount;
    }
}