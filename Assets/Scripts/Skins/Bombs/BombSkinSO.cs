using Unity.Netcode;
using UnityEngine;

namespace Skins.Bombs
{
    [CreateAssetMenu(fileName = "Bomb Skin", menuName = "Bomb skin")]
    public class BombSkinSO : ScriptableObject
    {
        [SerializeField] private NetworkObject bombPrefab;

        public NetworkObject BombPrefab => bombPrefab;
    }
}