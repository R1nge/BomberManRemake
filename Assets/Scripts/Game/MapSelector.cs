using Unity.Netcode;
using UnityEngine;
using Zenject;

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

        private void Awake() => _gameStateController.OnCleanUpBeforeEnd += SelectRandomMap;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            SelectRandomMap();
        }

        private void SelectRandomMap() => _selectedMapIndex = Random.Range(0, presets.Length);

        public override void OnDestroy()
        {
            base.OnDestroy();
            _gameStateController.OnCleanUpBeforeEnd -= SelectRandomMap;
        }
    }
}