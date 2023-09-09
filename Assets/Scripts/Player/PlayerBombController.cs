using System;
using Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
    public class PlayerBombController : NetworkBehaviour
    {
        public event Action<int> OnInit, OnBombAmountChanged;
        [SerializeField] private int bombStartAmount;
        [SerializeField] private UnityEngine.InputSystem.PlayerInput playerInput;
        private NetworkVariable<int> _bombsAvailable;
        private SpawnerOnGrid _spawnerOnGrid;
        private PlayerInput _playerInput;

        [Inject]
        private void Inject(SpawnerOnGrid spawnerOnGrid) => _spawnerOnGrid = spawnerOnGrid;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _bombsAvailable = new NetworkVariable<int>(bombStartAmount);
            _bombsAvailable.OnValueChanged += OnBombValueChanged;
            var bombInput = playerInput.actions.FindActionMap("Player").FindAction("Bomb");
            bombInput.performed += SpawnBomb;
        }

        private void OnBombValueChanged(int _, int amount) => OnBombAmountChanged?.Invoke(amount);

        private void Start() => OnInit?.Invoke(_bombsAvailable.Value);

        private bool CanSpawn()
        {
            var coll = new Collider[4];

            var size = Physics.OverlapBoxNonAlloc(transform.position, transform.localScale / 4, coll,
                Quaternion.identity);

            for (int i = 0; i < size; i++)
            {
                if (coll[i].TryGetComponent(out Bomb _))
                {
                    return false;
                }
            }

            return true;
        }

        private void SpawnBomb(InputAction.CallbackContext callback)
        {
            if (!IsOwner) return;
            if (!_playerInput.InputEnabled) return;
            if (_bombsAvailable.Value == 0) return;
            if (!CanSpawn()) return;
            SpawnBombServerRpc();
        }

        [ServerRpc]
        private void SpawnBombServerRpc()
        {
            _bombsAvailable.Value--;
            var bomb = _spawnerOnGrid.SpawnBomb(transform.position, NetworkObject.OwnerClientId);
            bomb.OnExplosion += ReturnBomb;
        }

        private void ReturnBomb(Bomb bomb)
        {
            bomb.OnExplosion -= ReturnBomb;
            _bombsAvailable.Value++;
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseBombAmountServerRpc(int amount) => _bombsAvailable.Value += amount;
    }
}