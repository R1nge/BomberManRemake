﻿using Unity.Netcode;
using UnityEngine;

namespace Skins.Players
{
    public class SkinManager : MonoBehaviour
    {
        [SerializeField] private SkinSo[] skins;
        private int _selectedSkin;

        public void SelectSkin(int index) => _selectedSkin = index;

        public SkinSo GetSkinSo(int index) => skins[index];

        public int SkinsAmount => skins.Length;

        public int SkinIndex => _selectedSkin;

        public NetworkObject GetLobby(int index) => skins[index].LobbyPrefab;
        public NetworkObject GetSkinFPS(int index) => skins[index].PrefabFPS;
        public NetworkObject GetSkinTPS(int index) => skins[index].PrefabTPS;
        public NetworkObject GetEndGame(int index) => skins[index].EndGamePrefab;
    }
}