using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class SpawnFreeCamera : NetworkBehaviour
    {
        [SerializeField] private PlayerFreeCamera freeCamera;
        private PlayerHealth _playerHealth;

        private void Awake()
        {
            _playerHealth = GetComponent<PlayerHealth>();
            _playerHealth.OnDeath += SpawnClientRpc;
        }

        [ClientRpc]
        private void SpawnClientRpc()
        {
            if (IsOwner)
            {
                Instantiate(freeCamera, transform.position, Quaternion.identity);
            }
        }

        public override void OnDestroy() => _playerHealth.OnDeath -= SpawnClientRpc;
    }
}