using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerShield : NetworkBehaviour
    {
        [SerializeField] private GameObject shield;
        [SerializeField] private float duration;
        private GameObject _shieldRef;
        private NetworkVariable<bool> _isActive;
        private NetworkVariable<float> _duration;

        private void Awake()
        {
            _isActive = new NetworkVariable<bool>();
            _duration = new NetworkVariable<float>(duration);
        }

        [ServerRpc(RequireOwnership = false)]
        public void TurnOnServerRpc()
        {
            if (_isActive.Value) return;
            _isActive.Value = true;
            TurnOnClientRpc();
            StartCoroutine(Timer_c());
        }

        [ClientRpc]
        private void TurnOnClientRpc()
        {
            _shieldRef = Instantiate(shield);
            _shieldRef.transform.position = transform.position;
            _shieldRef.transform.parent = transform;
        }

        [ServerRpc(RequireOwnership = false)]
        private void TurnOffServerRpc()
        {
            if (!_isActive.Value) return;
            TurnOffClientRpc();
        }

        [ClientRpc]
        private void TurnOffClientRpc()
        {
            Destroy(_shieldRef);
        }

        private IEnumerator Timer_c()
        {
            while (_duration.Value > 0 && _isActive.Value)
            {
                yield return new WaitForSeconds(1);
                _duration.Value -= 1;
                if (_duration.Value <= 0)
                {
                    TurnOffServerRpc();
                }
            }
        }
    }
}