using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Skins.Bombs
{
    public class BombSkinsUI : MonoBehaviour
    {
        [SerializeField] private GameObject UI;
        [SerializeField] private Button open, close;
        [SerializeField] private Transform slotParent;
        [SerializeField] private BombSkinSlot slot;
        private DiContainer _diContainer;
        private BombSkinManager _bombSkinManager;

        [Inject]
        private void Inject(DiContainer diContainer, BombSkinManager bombSkinManager)
        {
            _diContainer = diContainer;
            _bombSkinManager = bombSkinManager;
        }

        private void Open() => UI.SetActive(true);

        private void Close() => UI.SetActive(false);

        private void Awake() => Init();

        private void Init()
        {
            UI.SetActive(false);

            open.onClick.AddListener(Open);
            close.onClick.AddListener(Close);

            for (int i = 0; i < _bombSkinManager.Skins.Length; i++)
            {
                var skin = _bombSkinManager.Skins[i];
                var slotInstance = _diContainer.InstantiatePrefabForComponent<BombSkinSlot>(slot, slotParent);
                slotInstance.SetIcon(skin.Icon);
                slotInstance.SetTitle(skin.Title);
                slotInstance.SetIndex(i);
            }
        }
    }
}