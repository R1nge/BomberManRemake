using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class MapSettingsUI : NetworkBehaviour
    {
        [SerializeField] private TMP_InputField sizeX, sizeZ;
        [SerializeField] private TMP_Dropdown rounds;
        private GameSettings _gameSettings;

        [Inject]
        private void Inject(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
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

                _gameSettings.SetMapWidth(size);
            });

            sizeZ.onEndEdit.AddListener(s =>
            {
                var size = int.Parse(s);

                if (size % 2 != 1)
                {
                    size++;
                }

                _gameSettings.SetMapLength(size);
            });

            rounds.onValueChanged.AddListener(i =>
            {
                _gameSettings.SetRoundsAmount(i + 1);
            });
        }

        public override void OnNetworkSpawn()
        {
            sizeX.gameObject.SetActive(IsOwner);
            sizeZ.gameObject.SetActive(IsOwner);
        }
    }
}