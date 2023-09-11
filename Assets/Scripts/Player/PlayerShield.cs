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

        public bool IsActive => _isActive.Value;

        private void Awake()
        {
            _isActive = new NetworkVariable<bool>();
            _duration = new NetworkVariable<float>(duration);
            _duration.OnValueChanged += OnDurationChanged;
        }

        private void OnDurationChanged(float _, float durationLeft)
        {
            if (durationLeft <= 0)
            {
                TurnOffServerRpc();
            }
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
        public void TurnOffServerRpc()
        {
            if (!_isActive.Value) return;
            _isActive.Value = false;
            _duration.Value = duration;
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
            }
        }
    }
}