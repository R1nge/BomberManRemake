using Unity.Netcode;
using UnityEngine;

namespace Skins.Bombs
{
    public class BombSkinManager : MonoBehaviour
    {
        [SerializeField] private BombSkinSO[] skins;
        private int _selectedSkinIndex;

        public BombSkinSO[] Skins => skins;
        public void SelectSkin(int index) => _selectedSkinIndex = index;
        public int SkinIndex => _selectedSkinIndex;
        public NetworkObject GetSkin(int index) => skins[index].BombPrefab;
    }
}