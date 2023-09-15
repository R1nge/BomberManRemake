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
        private void Inject(SkinManager skinManager) => _skinManager = skinManager;

        private void Awake() => select.onClick.AddListener(SelectSkin);

        public void SetTitle(string title) => titleText.text = title;
        public void SetIcon(Sprite sprite) => icon.sprite = sprite;
        public void SetIndex(int skinIndex) => _skinIndex = skinIndex;
        private void SelectSkin() => _skinManager.SelectSkin(_skinIndex);
    }
}