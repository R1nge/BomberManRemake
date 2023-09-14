using System;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Skins.Players
{
    public class SkinSlot : MonoBehaviour
    {
        [SerializeField] private UIDocument slot;
        private int _skinIndex;
        private SkinManager _skinManager;

        private const string TITLE = "Title";
        private const string ICON = "Icon";
        private const string SELECT = "Select";

        [Inject]
        private void Inject(SkinManager skinManager) => _skinManager = skinManager;

        private void Awake()
        {
            slot.rootVisualElement.Q<Button>(SELECT).clicked += SelectSkin;
        }

        public void SetTitle(string title) => slot.rootVisualElement.Q<Label>(TITLE).text = title;

        public void SetIcon(Sprite sprite) =>
            slot.rootVisualElement.Q<VisualElement>(ICON).style.backgroundImage = sprite.texture;

        public void SetIndex(int skinIndex) => _skinIndex = skinIndex;
        private void SelectSkin() => _skinManager.SelectSkin(_skinIndex);
    }
}