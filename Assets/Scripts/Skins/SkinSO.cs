using Unity.Netcode;
using UnityEngine;

namespace Skins
{
    [CreateAssetMenu(fileName = "Skin", menuName = "Skin")]
    public class SkinSo : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField] private Sprite icon;
        [SerializeField] private NetworkObject lobbyPrefab, prefabFPS, prefabTPS, endGamePrefab;

        public string Title => title;
        public Sprite Icon => icon;
        public NetworkObject LobbyPrefab => lobbyPrefab;
        public NetworkObject PrefabFPS => prefabFPS;
        public NetworkObject PrefabTPS => prefabTPS;
        public NetworkObject EndGamePrefab => endGamePrefab;
    }
}