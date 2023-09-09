using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerBlockUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI dig;
        private PlayerBlockController _blockController;

        private void Awake()
        {
            _blockController = GetComponent<PlayerBlockController>();
            _blockController.OnDigAmountChanged += UpdateDigUI;
        }

        public override void OnNetworkSpawn() => dig.gameObject.SetActive(IsOwner);

        private void UpdateDigUI(int amount) => dig.text = $"Digs: {amount}";

        public override void OnDestroy()
        {
            _blockController.OnDigAmountChanged -= UpdateDigUI;
            base.OnDestroy();
        }
    }
}