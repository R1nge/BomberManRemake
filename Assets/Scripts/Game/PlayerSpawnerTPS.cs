﻿using System;
using System.Collections.Generic;
using Player;
using Skins;
using Skins.Players;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game
{
    public class PlayerSpawnerTPS : NetworkBehaviour
    {
        [SerializeField] private Transform dynamicParent;
        [SerializeField] private MapPreset mapPreset;
        private readonly List<Vector3> _positions = new();
        private bool _leftTop, _rightTop, _rightBottom, _leftBottom;
        private DiContainer _diContainer;
        private GameSettings _gameSettings;
        private Lobby.Lobby _lobby;
        private SkinManager _skinManager;

        [Inject]
        private void Inject(DiContainer diContainer, GameSettings gameSettings, Lobby.Lobby lobby, SkinManager skinManager)
        {
            _diContainer = diContainer;
            _gameSettings = gameSettings;
            _lobby = lobby;
            _skinManager = skinManager;
        }

        private void InitPositions()
        {
            _positions.Clear();
            _positions.Add(new Vector3(0, 0, 0));
            _positions.Add(new Vector3((_gameSettings.MapWidth - 1) * mapPreset.Size, 0, 0));
            _positions.Add(new Vector3(0, 0, (_gameSettings.MapLength - 1) * mapPreset.Size));
            _positions.Add(new Vector3((_gameSettings.MapWidth - 1) * mapPreset.Size, 0,
                (_gameSettings.MapLength - 1) * mapPreset.Size));
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