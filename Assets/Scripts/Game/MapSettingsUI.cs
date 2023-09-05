using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class MapSettingsUI : NetworkBehaviour
    {
        [SerializeField] private TMP_InputField sizeX, sizeZ;
        private MapSettings _mapSettings;

        [Inject]
        private void Inject(MapSettings mapSettings)
        {
            _mapSettings = mapSettings;
        }

        private void Awake()
        {
            sizeX.onEndEdit.AddListener(s =>
            {
                var size = int.Parse(s);
                
                if (size % 2 != 1)
                {
                    size++;
                }

                _mapSettings.SetWidth(size);
            });
            sizeZ.onEndEdit.AddListener(s =>
            {
                var size = int.Parse(s);
                
                if (size % 2 != 1)
                {
                    size++;
                }

                _mapSettings.SetLength(size);
            });
        }

        public override void OnNetworkSpawn()
        {
            sizeX.gameObject.SetActive(IsOwner);
            sizeZ.gameObject.SetActive(IsOwner);
        }
    }
}