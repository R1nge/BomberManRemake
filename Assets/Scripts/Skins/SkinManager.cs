using Unity.Netcode;
using UnityEngine;

namespace Skins
{
    public class SkinManager : MonoBehaviour
    {
        [SerializeField] private SkinSO[] skins;
        private int _selectedSkin;

        private void Awake()
        {
            _selectedSkin = Random.Range(0, skins.Length);
            print(_selectedSkin);
        }

        public void SelectSkin(int index)
        {
            
        }

        public int SkinIndex => _selectedSkin;

        public NetworkObject GetSkin(int index) => skins[index].Prefab;
    }
}