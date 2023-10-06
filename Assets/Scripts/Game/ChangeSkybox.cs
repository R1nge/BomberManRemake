using Game.StateMachines;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class ChangeSkybox : NetworkBehaviour
    {
        private NetworkVariable<int> _selectedSkybox;
        private GameStateController _gameStateController;
        private MapSelector _mapSelector;

        [Inject]
        private void Inject(GameStateController gameStateController, MapSelector mapSelector)
        {
            _gameStateController = gameStateController;
            _mapSelector = mapSelector;
        }

        private void Awake()
        {
            _gameStateController.OnStateChanged += StateChanged;
            _selectedSkybox = new NetworkVariable<int>();
            _selectedSkybox.OnValueChanged += OnValueChanged;
        }

        private void StateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.NextRound:
                    SelectSkyboxServerRpc();
                    break;
             }
        }

        private void OnValueChanged(int _, int skyboxIndex)
        {
            print("VALUE CHANGED");
            SwapSkyboxClientRpc(skyboxIndex);
        }

        private void Start()
        {
            SelectSkyboxServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SelectSkyboxServerRpc()
        {
            print("SELECT SKYBOX");
            RenderSettings.skybox = _mapSelector.SelectedMap.Skybox;
            //SwapSkyboxClientRpc(_mapSelector.SelectedMap);
        }

        [ClientRpc]
        private void SwapSkyboxClientRpc(int index)
        {
            print("SWAPPED SKYBOX");
            RenderSettings.skybox = _mapSelector.SelectedMap.Skybox;
        }

        public override void OnDestroy()
        {
            _gameStateController.OnStateChanged -= StateChanged;
        }
    }
}