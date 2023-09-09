using System;
using Game;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public abstract class PlayerBlockController : NetworkBehaviour
    {
        public event Action<int> OnDigAmountChanged;
        [SerializeField] protected int distance;
        [SerializeField] private int digAmount;
        private NetworkVariable<int> _currentDigAmount;
        private PlayerInput _playerInput;

        protected virtual void Awake()
        {
            _currentDigAmount = new NetworkVariable<int>(digAmount);
            _currentDigAmount.OnValueChanged += DigAmountChanged;
            _playerInput = GetComponent<PlayerInput>();
        }

        protected virtual void Start() => OnDigAmountChanged?.Invoke(CurrentDigAmount);
        
        public void OnDestroyBlock()
        {
            if (!IsOwner) return;
            if (!_playerInput.InputEnabled) return;
            Raycast();
        }
        [ServerRpc]
        protected void DestroyBlockServerRpc(NetworkObjectReference destructableRef)
        {
            if (CurrentDigAmount > 0)
            {
                if (destructableRef.TryGet(out NetworkObject networkObject))
                {
                    if (networkObject.TryGetComponent(out Destructable destructable))
                    {
                        destructable.SpawnDropServerRpc();
                        DecreaseDigAmountServerRpc(1);
                    }
                }
            }
        }
        

        protected abstract void Raycast();

        private void DigAmountChanged(int _, int amount) => OnDigAmountChanged?.Invoke(amount);

        protected int CurrentDigAmount => _currentDigAmount.Value;

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseDigAmountServerRpc(int amount) => _currentDigAmount.Value += amount;

        [ServerRpc(RequireOwnership = false)]
        protected void DecreaseDigAmountServerRpc(int amount) => _currentDigAmount.Value -= amount;
    }
}