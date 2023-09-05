using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerBombUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI bombs;
        private PlayerBombController _playerBombController;

        private void Awake()
        {
            _playerBombController = GetComponent<PlayerBombController>();
            _playerBombController.OnInit += UpdateUI;
            _playerBombController.OnBombAmountChanged += UpdateUI;
        }

        public override void OnNetworkSpawn() => bombs.gameObject.SetActive(IsOwner);

        private void UpdateUI(int bombAmount) => bombs.text = $"Bombs: {bombAmount}";

        public override void OnDestroy()
        {
            _playerBombController.OnInit -= UpdateUI;
            _playerBombController.OnBombAmountChanged -= UpdateUI;
        }
    }
}