using Unity.Netcode;
using UnityEngine;

namespace Skins
{
    [CreateAssetMenu(fileName = "Skin", menuName = "Skin")]
    public class SkinSO : ScriptableObject
    {
        [SerializeField] private string name;
        [SerializeField] private NetworkObject prefab;

        public string Name => name;
        public NetworkObject Prefab => prefab;
    }
}