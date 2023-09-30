using System;
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
        private SlotStatus _slotStatus;

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

        private void OnEnable() => UpdateSelectedSkin(_skinManager.SelectedSkinIndex, _skinManager.SelectedSkinIndex);

        private void UpdateSelectedSkin(int previous, int current)
        {
            var skinUnlocked = _skinManager.SkinUnlocked(_skinIndex);

            if (skinUnlocked)
            {
                _slotStatus = SlotStatus.Unlocked;

                if (_skinIndex == current)
                {
                    _slotStatus = SlotStatus.Selected;
                }
            }
            else
            {
                _slotStatus = SlotStatus.Locked;
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            switch (_slotStatus)
            {
                case SlotStatus.Locked:
                    Locked();
                    break;
                case SlotStatus.Unlocked:
                    Unlocked();
                    break;
                case SlotStatus.Selected:
                    Selected();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Locked()
        {
            var price = _skinManager.GetSkinData(_skinIndex);
            select.interactable = false;
            select.GetComponentInChildren<TextMeshProUGUI>().text = price.Price.ToString();
            select.interactable = _wallet.Money >= price.Price;
        }

        private void Unlocked()
        {
            select.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
            select.interactable = true;
        }

        private void Selected()
        {
            select.GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
            select.interactable = false;
        }

        public void SetTitle(string title) => titleText.text = title;
        public void SetIcon(Sprite sprite) => icon.sprite = sprite;
        public void SetIndex(int skinIndex) => _skinIndex = skinIndex;

        private async void SelectSkin()
        {
            var skinUnlocked = _skinManager.SkinUnlocked(_skinIndex);

            if (skinUnlocked)
            {
                _skinManager.SelectSkin(_skinIndex);
            }

            var unlockTask = _skinManager.UnlockSkin(_skinIndex);

            await unlockTask;

            if (unlockTask.Result)
            {
                _skinManager.SelectSkin(_skinIndex);
            }
        }

        private void OnDestroy() => _skinManager.OnSkinChanged -= UpdateSelectedSkin;

        private enum SlotStatus
        {
            Locked,
            Unlocked,
            Selected
        }
    }
}