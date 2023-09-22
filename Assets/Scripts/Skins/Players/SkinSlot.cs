using System;
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

        [Inject]
        private void Inject(SkinManager skinManager)
        {
            _skinManager = skinManager;
            print($"INJEECTED {skinManager}");
        }

        private void Awake()
        {
            select.onClick.AddListener(SelectSkin);
        }

        private void OnEnable()
        {
            select.interactable = _skinManager.SkinUnlocked(_skinIndex);
            
            print($"SKIN {_skinIndex} UNLOCKED: {_skinManager.SkinUnlocked(_skinIndex)}");
            
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
                }
                
            }
            else
            {
                select.GetComponentInChildren<TextMeshProUGUI>().text = _skinManager.GetSkinSo(_skinIndex).Price.ToString();
            }
        }

        public void SetTitle(string title) => titleText.text = title;
        public void SetIcon(Sprite sprite) => icon.sprite = sprite;
        public void SetIndex(int skinIndex) => _skinIndex = skinIndex;
        private void SelectSkin() => _skinManager.SelectSkin(_skinIndex);
    }
}