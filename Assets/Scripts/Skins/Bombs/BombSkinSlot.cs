using Skins.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Skins.Bombs
{
    public class BombSkinSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image icon;
        [SerializeField] private Button select;
        private int _skinIndex;
        private BombSkinManager _bombSkinManager;

        [Inject]
        private void Inject(BombSkinManager bombSkinManager) => _bombSkinManager = bombSkinManager;

        private void Awake() => select.onClick.AddListener(SelectSkin);

        public void SetTitle(string title) => titleText.text = title;
        public void SetIcon(Sprite sprite) => icon.sprite = sprite;
        public void SetIndex(int skinIndex) => _skinIndex = skinIndex;
        private void SelectSkin() => _bombSkinManager.SelectSkin(_skinIndex);
    }
}