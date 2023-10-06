using System;
using Game;
using Game.StateMachines;
using Unity.Netcode;
using Zenject;

namespace Player
{
    public class PlayerDestroy : NetworkBehaviour
    {
        private GameStateController2 _gameStateController2;

        [Inject]
        private void Inject(GameStateController2 gameStateController) => _gameStateController2 = gameStateController;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _gameStateController2.OnStateChanged += StateChanged;
            }
        }

        private void StateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.NextRound:
                    Destroy();
                    break;
            }
        }

        private void Destroy() => NetworkObject.Despawn(true);

        public override void OnDestroy()
        {
            if (IsServer)
            {
                _gameStateController2.OnStateChanged -= StateChanged;
            }
        }
    }
}