using Game;
using UnityEngine;
using Zenject;

namespace Lobby
{
    public class ResetSettings : MonoBehaviour
    {
        private GameSettings _gameSettings;

        [Inject]
        private void Inject(GameSettings gameSettings) => _gameSettings = gameSettings;

        private void Awake() => _gameSettings.ResetSettings();
    }
}