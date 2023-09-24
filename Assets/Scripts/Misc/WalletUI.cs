using TMPro;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class WalletUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moneyText;
        private Wallet _wallet;

        [Inject]
        private void Inject(Wallet wallet) => _wallet = wallet;

        private void Awake()
        {
            _wallet.OnMoneyAmountChanged += UpdateUI;
            UpdateUI(_wallet.Money);
        }

        private void UpdateUI(int money) => moneyText.text = $"Money: {money}";

        private void OnDestroy() => _wallet.OnMoneyAmountChanged -= UpdateUI;
    }
}