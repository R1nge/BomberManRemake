using Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Zenject;

namespace Lobby
{
    public class GameSettingsUI : NetworkBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        private GameSettings _gameSettings;
        private Lobby _lobby;
        private const string GAME_SETTINGS = "GameSettings";
        private const string MAP_MODE = "MapModes";
        private const string PERSPECTIVE_MODE = "PerspectiveModes";
        private const string ROUNDS = "Rounds";
        private const string GAME_SETTINGS_NEXT = "GameSettingsNext";
        private const string MAP_CUSTOMIZATION = "MapCustomization";
        private const string MAP_CUSTOMIZATION_NEXT = "MapCustomizationNext";
        private const string PROCEDURAL_MAP_CUSTOMIZATION = "ProceduralMapCustomization";
        private const string MAP_SIZE_X = "MapSizeX";
        private const string MAP_SIZE_Z = "MapSizeZ";
        private const string PROCEDURAL_MAP_NEXT = "ProceduralMapCustomizationNext";
        private const string POWERUP_CUSTOMIZATION = "PowerupCustomization";
        private const string DROP_CHANCE = "DropChance";

        private const string START = "Start";
        private const string READY = "Ready";

        [Inject]
        private void Inject(GameSettings gameSettings, Lobby lobby)
        {
            _gameSettings = gameSettings;
            _lobby = lobby;
        }

        private void Awake()
        {
            var root = uiDocument.rootVisualElement;

            root.Q<EnumField>(MAP_MODE).RegisterValueChangedCallback(evt =>
            {
                _gameSettings.SetMapMode((GameSettings.MapModes)evt.newValue);
            });

            root.Q<EnumField>(PERSPECTIVE_MODE).RegisterValueChangedCallback(evt =>
            {
                _gameSettings.SetPerspectiveMode((GameSettings.PerspectiveModes)evt.newValue);
            });

            root.Q<EnumField>(ROUNDS).RegisterValueChangedCallback(evt =>
            {
                _gameSettings.SetRoundsAmount((GameSettings.RoundAmount)evt.newValue);
            });

            root.Q<Button>(GAME_SETTINGS_NEXT).clicked += () =>
            {
                root.Q<VisualElement>(GAME_SETTINGS).visible = false;
                root.Q<VisualElement>(PROCEDURAL_MAP_CUSTOMIZATION).visible = true;
                //root.Q<VisualElement>(MAP_CUSTOMIZATION).visible = true;
            };

            root.Q<Button>(MAP_CUSTOMIZATION_NEXT).clicked += () =>
            {
                // root.Q<VisualElement>(MAP_CUSTOMIZATION).visible = false;
            };

            root.Q<Button>(PROCEDURAL_MAP_NEXT).clicked += () =>
            {
                root.Q<VisualElement>(PROCEDURAL_MAP_CUSTOMIZATION).visible = false;
                root.Q<VisualElement>(POWERUP_CUSTOMIZATION).visible = true;
            };

            root.Q<SliderInt>(MAP_SIZE_X).RegisterValueChangedCallback(evt =>
            {
                var size = evt.newValue;

                if (size % 2 != 1)
                {
                    size++;
                }

                _gameSettings.SetMapWidth(size);
            });

            root.Q<SliderInt>(MAP_SIZE_Z).RegisterValueChangedCallback(evt =>
            {
                var size = evt.newValue;

                if (size % 2 != 1)
                {
                    size++;
                }

                _gameSettings.SetMapLength(size);
            });

            root.Q<SliderInt>(DROP_CHANCE).RegisterValueChangedCallback(evt =>
            {
                _gameSettings.SetDropChance(evt.newValue);
            });

            root.Q<Button>(START).clicked += () =>
            {
                NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName("Lobby"));
            };

            root.Q<Button>(READY).clicked += () =>
            {
                _lobby.ChangeReadyStateServerRpc(NetworkManager.Singleton.LocalClientId);
            };

            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
            _lobby.OnReadyStateChanged += ReadyStateChanged;
        }

        private void ClientConnected(ulong clientId) => ReadyStateChanged(clientId, false);

        private void ReadyStateChanged(ulong clientId, bool isReady) =>
            uiDocument.rootVisualElement.Q<Button>(START).SetEnabled(_lobby.CanStartGame());

        public override void OnNetworkSpawn()
        {
            uiDocument.rootVisualElement.Q<Button>(START).visible = IsOwner;
            uiDocument.rootVisualElement.Q<Button>(START).SetEnabled(_lobby.CanStartGame());
            uiDocument.rootVisualElement.Q<VisualElement>(GAME_SETTINGS).visible = IsOwner;
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