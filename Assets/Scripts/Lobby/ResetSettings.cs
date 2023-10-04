using Game;
using UnityEngine;
using Zenject;

namespace Lobby
{
    public class ResetSettings : MonoBehaviour
    {
        private GameSettings _gameSettings;
        private Lobby _lobby;

        [Inject]
        private void Inject(GameSettings gameSettings, Lobby lobby)
        {
            _gameSettings = gameSettings;
            _lobby = lobby;
        }

        private void Awake()
        {
            _gameSettings.ResetSettings();
            _lobby.ResetLobby();
        }
    }
}