using Unity.Netcode;
using UnityEngine;

namespace Skins
{
    [CreateAssetMenu(fileName = "Skin", menuName = "Skin")]
    public class SkinSO : ScriptableObject
    {
        [SerializeField] private string name;
        [SerializeField] private NetworkObject lobbyPrefab, prefabFPS, prefabTPS;

        public string Name => name;
        public NetworkObject LobbyPrefab => lobbyPrefab;
        public NetworkObject PrefabFPS => prefabFPS;
        public NetworkObject PrefabTPS => prefabTPS;
    }
}