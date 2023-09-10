using Unity.Netcode;
using UnityEngine;

namespace Skins.Bombs
{
    [CreateAssetMenu(fileName = "Bomb Skin", menuName = "Bomb skin")]
    public class BombSkinSO : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField] private Sprite icon;
        [SerializeField] private NetworkObject bombPrefab;
        
        public string Title => title;
        public Sprite Icon => icon;
        public NetworkObject BombPrefab => bombPrefab;
    }
}