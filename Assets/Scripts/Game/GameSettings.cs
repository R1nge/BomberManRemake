using System;
using UnityEngine;

namespace Game
{
    public class GameSettings
    {
        private int _mapWidth = 15, _mapLength = 15;
        private float _dropChance = .5f;
        private float _roundTime;
        private RoundAmount _roundsAmount = RoundAmount.One;
        private PerspectiveModes _perspectiveMode;
        private MapModes _mapModes;

        public int MapWidth => _mapWidth;
        public int MapLength => _mapLength;
        public float DropChance => _dropChance;
        public float RoundTime => _roundTime;
        public RoundAmount RoundsAmount => _roundsAmount;
        public PerspectiveModes PerspectiveMode => _perspectiveMode;
        public MapModes MapMode => _mapModes;

        public void SetMapWidth(int width) => _mapWidth = width;

        public void SetMapLength(int length) => _mapLength = length;

        public void SetRoundTime(float time) => _roundTime = time;

        public void SetDropChance(float chance) => _dropChance = Math.Clamp(chance / 100, 0, 1);

        public void SetRoundsAmount(RoundAmount amount) => _roundsAmount = amount;

        public void SetPerspectiveMode(PerspectiveModes perspectiveMode) => _perspectiveMode = perspectiveMode;

        public void SetMapMode(MapModes mapMode) => _mapModes = mapMode;

        public void ResetSettings()
        {
            _mapWidth = 15;
            _mapLength = 15;
            _dropChance = .5f;
            _roundTime = 1;
            _roundsAmount = RoundAmount.One;
            _perspectiveMode = 0;
        }

        public enum PerspectiveModes
        {
            Fps,
            Tps
        }
        
        public enum MapModes
        {
            Procedural,
            Predefined
        }
        
        public enum RoundAmount
        {
            One,
            Two,
            Three,
            Four,
            Five
        }
    }
}