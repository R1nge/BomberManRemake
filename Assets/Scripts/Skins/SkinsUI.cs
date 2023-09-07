using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Skins
{
    public class SkinsUI : MonoBehaviour
    {
        [SerializeField] private GameObject UI;
        [SerializeField] private Button open, close;
        [SerializeField] private Transform slotParent;
        [SerializeField] private SkinSlot slot;
        private DiContainer _diContainer;
        private SkinManager _skinManager;

        [Inject]
        private void Inject(DiContainer diContainer, SkinManager skinManager)
        {
            _diContainer = diContainer;
            _skinManager = skinManager;
        }

        private void Open() => UI.SetActive(true);

        private void Close() => UI.SetActive(false);

        private void Awake() => Init();

        private void Init()
        {
            UI.SetActive(false);

            open.onClick.AddListener(Open);
            close.onClick.AddListener(Close);

            for (int i = 0; i < _skinManager.Skins.Length; i++)
            {
                var skin = _skinManager.Skins[i];
                var slotInstance = _diContainer.InstantiatePrefabForComponent<SkinSlot>(slot, slotParent);
                slotInstance.SetIcon(skin.Icon);
                slotInstance.SetTitle(skin.Title);
                slotInstance.SetIndex(i);
            }
        }
    }
}