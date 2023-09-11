﻿using System;
using System.Collections.Generic;
using Skins.Players;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class PlayerSpawnerFPS : NetworkBehaviour
    {
        private event Action<ulong, int> OnPositionsInit;
        [SerializeField] private Transform dynamicParent;
        [SerializeField] private MapPreset mapPreset;
        private readonly List<Vector3> _positions = new();
        private DiContainer _diContainer;
        private GameSettings _gameSettings;
        private Lobby.Lobby _lobby;
        private SkinManager _skinManager;

        [Inject]
        private void Inject(DiContainer diContainer, GameSettings gameSettings, Lobby.Lobby lobby,
            SkinManager skinManager)
        {
            _diContainer = diContainer;
            _gameSettings = gameSettings;
            _lobby = lobby;
            _skinManager = skinManager;
        }

        private void Awake() => OnPositionsInit += OnOnPositionsInit;

        public void OnNextRound(ulong clientId, int index) => InitPositions(clientId, index);
        
        private void OnOnPositionsInit(ulong clientId, int index) => Spawn(clientId, index);

        public void InitPositions(ulong clientId, int index)
        {
            _positions.Clear();
            _positions.Add(new Vector3(0, 0, 0));
            _positions.Add(new Vector3((_gameSettings.MapWidth - 1) * mapPreset.Size, 0, 0));
            _positions.Add(new Vector3(0, 0, (_gameSettings.MapLength - 1) * mapPreset.Size));
            _positions.Add(new Vector3((_gameSettings.MapWidth - 1) * mapPreset.Size, 0,
                (_gameSettings.MapLength - 1) * mapPreset.Size));

            OnPositionsInit?.Invoke(clientId, index);
        }

        private void Spawn(ulong clientId, int index)
        {
            var skinIndex = _lobby.GetData(clientId).Value.SkinIndex;
            print($"SKIN INDEX: {skinIndex}");
            var position = _positions[index];
            var player = _diContainer.InstantiatePrefab(_skinManager.Skins[skinIndex].PrefabFPS, position, Quaternion.identity, null);
            player.transform.parent = dynamicParent;
            player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);
            player.transform.position = position;
            player.transform.parent = dynamicParent;
            player.transform.position = position;
        }

        //Left Bottom new Vector3(0, 0, 0)
        //Right Bottom new Vector3((_gameSettings.MapWidth - 1) * mapPreset.Size, 0, 0)
        //Left Top new Vector3(0, 0, (_gameSettings.MapLength - 1) * mapPreset.Size)
        //Right Top  new Vector3((_gameSettings.MapWidth - 1) * mapPreset.Size, 0, (_gameSettings.MapLength - 1) * mapPreset.Size);

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnPositionsInit -= OnOnPositionsInit;
        }
    }
}