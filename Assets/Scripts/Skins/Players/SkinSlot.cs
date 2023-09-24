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
            
            UpdateUI();
        }

        //TODO: update on skin change
        private void OnEnable() => UpdateUI();

        private async void UpdateUI()
        {
            var price = await _skinManager.GetSkinData(_skinIndex);
            
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
                select.interactable = _skinManager.SkinUnlocked(_skinIndex);
                select.GetComponentInChildren<TextMeshProUGUI>().text = price.Price.ToString();
                select.interactable = _wallet.Money >= price.Price;
            }
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

        public void SetTitle(string title) => titleText.text = title;
        public void SetIcon(Sprite sprite) => icon.sprite = sprite;
        public void SetIndex(int skinIndex) => _skinIndex = skinIndex;
        private void SelectSkin() => _skinManager.SelectSkin(_skinIndex);

        private void OnDestroy() => _skinManager.OnSkinChanged -= UpdateSelectedSkin;
    }
}