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
        [SerializeField] private PlayerInput playerInput;
        private NetworkVariable<int> _bombsAvailable;
        private SpawnerOnGrid _spawnerOnGrid;

        [Inject]
        private void Inject(SpawnerOnGrid spawnerOnGrid)
        {
            _spawnerOnGrid = spawnerOnGrid;
        }

        private void Awake()
        {
            _bombsAvailable = new NetworkVariable<int>(bombStartAmount);
            _bombsAvailable.OnValueChanged += OnBombValueChanged;
            var bombInput = playerInput.actions.FindActionMap("Player").FindAction("Bomb");
            bombInput.performed += SpawnBomb;
        }

        private void OnBombValueChanged(int _, int amount) => OnBombAmountChanged?.Invoke(amount);

        private void Start() => OnInit?.Invoke(_bombsAvailable.Value);

        private void SpawnBomb(InputAction.CallbackContext callback)
        {
            if (!IsOwner) return;
            if (_bombsAvailable.Value == 0) return;
            SpawnBombServerRpc();
        }

        [ServerRpc]
        private void SpawnBombServerRpc()
        {
            _bombsAvailable.Value--;
            var bomb = _spawnerOnGrid.SpawnBomb(transform.position);
            bomb.OnExplosion += ReturnBomb;
        }


        private void ReturnBomb(Bomb bomb)
        {
            bomb.OnExplosion -= ReturnBomb;
            _bombsAvailable.Value++;
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseBombAmountServerRpc(int amount)
        {
            _bombsAvailable.Value += amount;
        }
    }
}