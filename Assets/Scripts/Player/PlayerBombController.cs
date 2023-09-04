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
        [SerializeField] private PlayerInput playerInput;
        private SpawnerOnGrid _spawnerOnGrid;

        [Inject]
        private void Inject(SpawnerOnGrid spawnerOnGrid)
        {
            _spawnerOnGrid = spawnerOnGrid;
        }

        private void Awake()
        {
            var bombInput = playerInput.actions.FindActionMap("Player").FindAction("Bomb");
            bombInput.performed += SpawnBomb;
        }

        private void SpawnBomb(InputAction.CallbackContext callback)
        {
            if (!IsOwner) return;
            SpawnBombServerRpc();
        }

        [ServerRpc]
        private void SpawnBombServerRpc()
        {
            _spawnerOnGrid.SpawnBomb(transform.position);
        }
    }
}