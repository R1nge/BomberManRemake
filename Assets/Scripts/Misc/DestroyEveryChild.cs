using Game;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class DestroyEveryChild : MonoBehaviour
    {
        private RoundManager _roundManager;

        [Inject]
        private void Inject(RoundManager roundManager)
        {
            _roundManager = roundManager;
        }

        private void Awake()
        {
            _roundManager.LoadNextRound += DestroyChildren;
        }

        private void DestroyChildren()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            print($"DESTROYING CHILDREN {transform.root.root.childCount}");
            foreach (Transform child in transform.root)
            {
                if (child.TryGetComponent(out NetworkObject networkObject))
                {
                    print($"DESPAWN {child.name}");
                    networkObject.Despawn(true);
                }
            }
        }
    }
}