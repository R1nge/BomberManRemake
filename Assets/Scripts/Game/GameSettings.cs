using System;
using UnityEngine;

namespace Game
{
    public class GameSettings
    {
        private int _mapWidth = 15, _mapLength = 15;
        private float _dropChance = .5f;
        private int _roundsAmount = 1;
        private GameModes _gameMode;

        public int MapWidth => _mapWidth;
        public int MapLength => _mapLength;
        public float DropChance => _dropChance;
        public int RoundsAmount => _roundsAmount;
        public GameModes GameMode => _gameMode;

        public void SetMapWidth(int width) => _mapWidth = width;

        public void SetMapLength(int length) => _mapLength = length;

        public void SetDropChance(float chance) => _dropChance = Math.Clamp(chance / 100, 0, 1);

        public void SetRoundsAmount(int amount) => _roundsAmount = amount;

        public void SetGameMode(GameModes gameMode) => _gameMode = gameMode;

        public void ResetSettings()
        {
            _mapWidth = 15;
            _mapLength = 15;
            _dropChance = .5f;
            _roundsAmount = 1;
            _gameMode = 0;
        }

        public enum GameModes
        {
            Fps,
            Tps
        }
    }
}