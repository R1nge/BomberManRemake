using Game;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Lobby
{
    public class GameSettingsUI : NetworkBehaviour
    {
        [SerializeField] private GameObject gameSettingsUI, proceduralSettingsUI;
        [SerializeField] private TMP_Dropdown mapMode, perspectiveMode;
        [SerializeField] private Slider roundAmount, roundTime;

        [SerializeField] private TMP_InputField mapSizeX, mapSizeZ;

        //[SerializeField] private Button next;
        [SerializeField] private TMP_InputField dropChance;
        [SerializeField] private Button ready, start;
        private GameSettings _gameSettings;
        private Lobby _lobby;
        private LobbyNetworkBehaviour _lobbyNetworkBehaviour;

        [Inject]
        private void Inject(GameSettings gameSettings, Lobby lobby)
        {
            _gameSettings = gameSettings;
            _lobby = lobby;
        }

        private void Awake()
        {
            _lobbyNetworkBehaviour =
                FindObjectsByType<LobbyNetworkBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)[0];

            mapMode.onValueChanged.AddListener(mode =>
            {
                _gameSettings.SetMapMode((GameSettings.MapModes)mode);
            });

            perspectiveMode.onValueChanged.AddListener(mode =>
            {
                _gameSettings.SetPerspectiveMode((GameSettings.PerspectiveModes)mode);
            });

            roundAmount.onValueChanged.AddListener(rounds =>
            {
                _gameSettings.SetRoundsAmount((GameSettings.RoundAmount)rounds);
            });

            roundTime.onValueChanged.AddListener(time =>
            {
                _gameSettings.SetRoundTime(time * 60);
            });

            // next.onClick.AddListener(() =>
            // {
            //     gameSettingsUI.gameObject.SetActive(false);
            //     proceduralSettingsUI.gameObject.SetActive(true);
            // });

            mapSizeX.onValueChanged.AddListener(sizeX =>
            {
                var intSizeX = int.Parse(sizeX);

                if (intSizeX % 2 != 1)
                {
                    intSizeX++;
                }

                _gameSettings.SetMapWidth(intSizeX);
            });

            mapSizeZ.onValueChanged.AddListener(sizeZ =>
            {
                var intSizeZ = int.Parse(sizeZ);

                if (intSizeZ % 2 != 1)
                {
                    intSizeZ++;
                }

                _gameSettings.SetMapLength(intSizeZ);
            });

            dropChance.onValueChanged.AddListener(chance =>
            {
                var chanceInt = int.Parse(chance);
                chanceInt = Mathf.Clamp(chanceInt, 0, 100);
                _gameSettings.SetDropChance(chanceInt);
            });

            start.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            });

            ready.onClick.AddListener(() =>
            {
                _lobbyNetworkBehaviour.ChangeReadyStateServerRpc(NetworkManager.Singleton.LocalClientId);
            });

            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
            _lobby.OnReadyStateChanged += ReadyStateChanged;
        }

        private void ClientConnected(ulong clientId) => ReadyStateChanged(clientId, false);

        private void ReadyStateChanged(ulong clientId, bool isReady) => start.interactable = _lobby.CanStartGame();

        public override void OnNetworkSpawn()
        {
            start.gameObject.SetActive(IsOwner);
            start.interactable = _lobby.CanStartGame();
            ready.gameObject.SetActive(true);
            gameSettingsUI.gameObject.SetActive(IsOwner);
            proceduralSettingsUI.gameObject.SetActive(false);
        }

        public override void OnDestroy()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnected;
            }

            _lobby.OnReadyStateChanged -= ReadyStateChanged;
        }
    }
}