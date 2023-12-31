﻿using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Skins.Players
{
    public class SkinsUI : MonoBehaviour
    {
        [SerializeField] private GameObject UI;
        [SerializeField] private Transform slotParent;
        [SerializeField] private SkinSlot slot;
        [SerializeField] private Button open, close;
        private DiContainer _diContainer;
        private SkinManager _skinManager;

        [Inject]
        private void Inject(DiContainer diContainer, SkinManager skinManager)
        {
            _diContainer = diContainer;
            _skinManager = skinManager;
        }

        private void Awake()
        {
            open.onClick.AddListener(Open);
            close.onClick.AddListener(Close);
            Init();
        }

        private void Open() => UI.SetActive(true);

        private void Close() => UI.SetActive(false);

        private void Init()
        {
            for (int i = 0; i < _skinManager.SkinsAmount; i++)
            {
                var skin = _skinManager.GetSkinSo(i);
                var slotInstance = _diContainer.InstantiatePrefabForComponent<SkinSlot>(slot, slotParent);
                slotInstance.SetIcon(skin.Icon);
                slotInstance.SetTitle(skin.Title);
                slotInstance.SetIndex(i);
            }

            Open();
            Close();
        }
    }
}