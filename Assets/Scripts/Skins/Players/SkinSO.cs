using Unity.Netcode;
using UnityEngine;

namespace Skins.Players
{
    [CreateAssetMenu(fileName = "Skin", menuName = "Skin")]
    public class SkinSo : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField] private Sprite icon;
        [SerializeField] private NetworkObject lobbyPrefab, prefabFPS, prefabTPS, endGamePrefab;
        [SerializeField] private int price;

        public string Title => title;
        public Sprite Icon => icon;
        public NetworkObject LobbyPrefab => lobbyPrefab;
        public NetworkObject PrefabFPS => prefabFPS;
        public NetworkObject PrefabTPS => prefabTPS;
        public NetworkObject EndGamePrefab => endGamePrefab;
        public int Price => price;
    }

    public class SkinData
    {
        public SkinData(int index, bool unlocked)
        {
            Index = index;
            Unlocked = unlocked;
        }

        public int Index;
        public bool Unlocked;
        public void Unlock() => Unlocked = true;
    }
}