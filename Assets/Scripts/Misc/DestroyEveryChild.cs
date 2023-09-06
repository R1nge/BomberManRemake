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
            print($"DESTROYING CHILDREN {transform.root.root.childCount}");
            for (int i = transform.root.childCount - 1; i >= 0; i--)
            {
                if (transform.root.GetChild(i).TryGetComponent(out NetworkObject networkObject))
                {
                    networkObject.Despawn(true);
                }
            }
        }
    }
}