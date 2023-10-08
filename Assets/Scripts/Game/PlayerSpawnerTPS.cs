using System.Collections.Generic;
using Player;
using Skins.Players;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class PlayerSpawnerTPS : NetworkBehaviour
    {
        [SerializeField] private Transform dynamicParent;
        private readonly List<Vector3> _positions = new();
        private bool _leftTop, _rightTop, _rightBottom, _leftBottom;
        private DiContainer _diContainer;
        private GameSettings _gameSettings;
        private Lobby.Lobby _lobby;
        private SkinManager _skinManager;
        private MapSelector _mapSelector;

        [Inject]
        private void Inject(DiContainer diContainer, GameSettings gameSettings, Lobby.Lobby lobby,
            SkinManager skinManager, MapSelector mapSelector)
        {
            _diContainer = diContainer;
            _gameSettings = gameSettings;
            _lobby = lobby;
            _skinManager = skinManager;
            _mapSelector = mapSelector;
        }

        private void InitPositions()
        {
            _positions.Clear();
            _positions.Add(new Vector3(0, 0, 0));
            _positions.Add(new Vector3((_gameSettings.MapWidth - 1) * _mapSelector.SelectedMap.Size, 0, 0));
            _positions.Add(new Vector3(0, 0, (_gameSettings.MapLength - 1) * _mapSelector.SelectedMap.Size));
            _positions.Add(new Vector3((_gameSettings.MapWidth - 1) * _mapSelector.SelectedMap.Size, 0,
                (_gameSettings.MapLength - 1) * _mapSelector.SelectedMap.Size));
        }

        public void Spawn(ulong clientId, int index)
        {
            InitPositions();
            var skinIndex = _lobby.GetData(clientId).Value.SkinIndex;
            print($"SKIN INDEX: {skinIndex}");
            var position = _positions[index];
            var player = _diContainer.InstantiatePrefab(_skinManager.GetSkinTPS(skinIndex), position,
                Quaternion.identity, null);
            player.transform.parent = dynamicParent;
            player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);
            player.transform.position = position;

            if (index is 2 or 3)
            {
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                player.GetComponent<PlayerMovementTPS>().SetFlippedClientRpc(true);
            }

            player.transform.parent = dynamicParent;
        }
    }
}