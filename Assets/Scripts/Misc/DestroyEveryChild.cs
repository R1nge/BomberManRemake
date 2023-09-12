using System;
using Game;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class DestroyEveryChild : MonoBehaviour
    {
        private GameStateController _gameStateController;

        [Inject]
        private void Inject(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
        }

        private void Awake()
        {
           _gameStateController.OnCleanUpBeforeEnd += DestroyChildren;
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
            }
        }

        private void OnDestroy()
        {
            _gameStateController.OnCleanUpBeforeEnd -= DestroyChildren;
        }
    }
}