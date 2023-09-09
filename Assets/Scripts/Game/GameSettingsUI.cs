using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameSettingsUI : NetworkBehaviour
    {
        [SerializeField] private TMP_InputField sizeX, sizeZ;
        [SerializeField] private TMP_InputField dropChance;
        [SerializeField] private TMP_Dropdown rounds;
        [SerializeField] private TMP_Dropdown modes;
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
            
            dropChance.onValueChanged.AddListener(s =>
            {
                var chance = float.Parse(s);
                
                _gameSettings.SetDropChance(chance);
            });

            rounds.onValueChanged.AddListener(i =>
            {
                _gameSettings.SetRoundsAmount(i + 1);
            });
            
            modes.onValueChanged.AddListener(i =>
            {
                _gameSettings.SetGameMode((GameSettings.GameModes)i);
            });
        }

        public override void OnNetworkSpawn()
        {
            sizeX.gameObject.SetActive(IsOwner);
            sizeZ.gameObject.SetActive(IsOwner);
            dropChance.gameObject.SetActive(IsOwner);
            rounds.gameObject.SetActive(IsOwner);
            modes.gameObject.SetActive(IsOwner);
        }
    }
}