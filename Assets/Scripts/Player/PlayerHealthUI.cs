using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerHealthUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText;
        private PlayerHealth _playerHealth;

        private void Awake()
        {
            _playerHealth = GetComponent<PlayerHealth>();
            _playerHealth.OnInit += UpdateUI;
            _playerHealth.OnDamageTaken += UpdateUI;
        }

        public override void OnNetworkSpawn() => healthText.gameObject.SetActive(IsOwner);

        private void UpdateUI(int health) => healthText.text = $"Health: {health}";

        public override void OnDestroy()
        {
            _playerHealth.OnInit -= UpdateUI;
            _playerHealth.OnDamageTaken -= UpdateUI;
        }
    }
}