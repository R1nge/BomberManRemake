using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText;
        private PlayerHealth _playerHealth;

        private void Awake()
        {
            _playerHealth = GetComponent<PlayerHealth>();
            _playerHealth.OnInit += UpdateUI;
            _playerHealth.OnDamageTaken += UpdateUI;
        }

        private void UpdateUI(int health) => healthText.text = $"Health: {health}";

        private void OnDestroy()
        {
            _playerHealth.OnInit -= UpdateUI;
            _playerHealth.OnDamageTaken -= UpdateUI;
        }
    }
}