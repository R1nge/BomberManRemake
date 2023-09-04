using TMPro;
using UnityEngine;
using Zenject;

namespace Game
{
    public class MapSettingsUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField sizeX, sizeZ;
        private MapSettings _mapSettings;

        [Inject]
        private void Inject(MapSettings mapSettings)
        {
            _mapSettings = mapSettings;
        }
    }
}