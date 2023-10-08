using System;
using Game.StateMachines;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game
{
    public class MapSelector : NetworkBehaviour
    {
        [SerializeField] private MapPreset[] presets;
        private GameStateController _gameStateController;
        private int _selectedMapIndex;
        public MapPreset SelectedMap => presets[_selectedMapIndex];

        [Inject]
        private void Inject(GameStateController gameStateController) => _gameStateController = gameStateController;

        private void Awake() => _gameStateController.OnStateChanged += StateChanged;

        private void StateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.PreStart:
                    SelectRandomMap();
                    break;
            }
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            SelectRandomMap();
        }

        private void SelectRandomMap() => _selectedMapIndex = Random.Range(0, presets.Length);

        public override void OnDestroy() => _gameStateController.OnStateChanged -= StateChanged;
    }
}