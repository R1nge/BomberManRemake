using System;
using Game;
using Game.StateMachines;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class DestroyEveryChild : MonoBehaviour
    {
        private GameStateController2 _gameStateController;

        [Inject]
        private void Inject(GameStateController2 gameStateController)
        {
            _gameStateController = gameStateController;
        }

        private void Awake()
        {
            _gameStateController.OnStateChanged += StateChanged;
        }

        private void StateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.NextRound:
                case GameStates.EndGame:
                    DestroyChildren();
                    break;
            }
        }

        private void DestroyChildren()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            print($"DESTROYING CHILDREN {transform.root.root.childCount}");

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (transform.GetChild(i).TryGetComponent(out NetworkObject networkObject))
                {
                    if (!networkObject.IsSpawned)
                    {
                        Debug.LogError($"Skipped {networkObject.name} during children destruction");
                        continue;
                    }

                    networkObject.Despawn(true);
                }
                else
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }

        private void OnDestroy()
        {
            //_gameStateController.OnCleanUpBeforeEnd -= DestroyChildren;
        }
    }
}