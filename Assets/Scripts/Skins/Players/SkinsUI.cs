using System.Collections;
using UnityEngine;
using Zenject;

namespace Skins.Players
{
    public class SkinsUI : MonoBehaviour
    {
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

        private void Start() => Init();

        private void Init()
        {
            for (int i = 0; i < _skinManager.Skins.Length; i++)
            {
                var skin = _skinManager.Skins[i];
                var slotInstance = _diContainer.InstantiatePrefabForComponent<SkinSlot>(slot, slotParent);
                StartCoroutine(Wait_C(skin, slotInstance, i));
            }
        }

        private IEnumerator Wait_C(SkinSo skin, SkinSlot slotInstance, int i)
        {
            yield return new WaitForSeconds(1f);
            //slotInstance.SetIcon(skin.Icon);
            slotInstance.SetTitle(skin.Title);
            slotInstance.SetIndex(i);
        }
    }
}