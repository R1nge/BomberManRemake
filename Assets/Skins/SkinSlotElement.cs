using UnityEngine;
using UnityEngine.UIElements;

namespace Skins
{
    public class SkinSlotElement : VisualElement
    {
        private Label _title;
        private Image _icon;

        public SkinSlotElement(string title, Sprite icon)
        {
            _title = new Label(title);
            _icon = new Image
            {
                image = icon.texture
            };
        }
    }
}