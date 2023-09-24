using Misc;
using Skins.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Skins
{
    public class SkinSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image icon;
        [SerializeField] private Button select;
        private int _skinIndex;
        private SkinManager _skinManager;
        private Wallet _wallet;

        [Inject]
        private void Inject(SkinManager skinManager, Wallet wallet)
        {
            _skinManager = skinManager;
            _wallet = wallet;
        }

        private void Awake()
        {
            _skinManager.OnSkinChanged += UpdateSelectedSkin;
            select.onClick.AddListener(SelectSkin);
        }

        private void UpdateSelectedSkin(int previous, int current)
        {
            if (_skinIndex == previous)
            {
                select.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
                select.interactable = true;
            }

            if (_skinIndex == current)
            {
                select.GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
                select.interactable = false;
            }
        }

        private void OnEnable()
        {
            select.interactable = _skinManager.SkinUnlocked(_skinIndex);

            if (_skinManager.SkinUnlocked(_skinIndex))
            {
                if (_skinManager.SelectedSkinIndex == _skinIndex)
                {
                    select.GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
                    select.interactable = false;
                }
                else
                {
                    select.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
                    select.interactable = true;
                }
            }
            else
            {
                var price = _skinManager.GetSkinData(_skinIndex).Result.Price;
                select.GetComponentInChildren<TextMeshProUGUI>().text = price.ToString();
                select.interactable = _wallet.Money >= price;
            }
        }

        public void SetTitle(string title) => titleText.text = title;
        public void SetIcon(Sprite sprite) => icon.sprite = sprite;
        public void SetIndex(int skinIndex) => _skinIndex = skinIndex;
        private void SelectSkin() => _skinManager.SelectSkin(_skinIndex);

        private void OnDestroy() => _skinManager.OnSkinChanged -= UpdateSelectedSkin;
    }
}