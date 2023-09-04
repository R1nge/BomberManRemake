using Unity.Netcode;
using UnityEngine;

namespace Misc
{
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField] private float delay;

        private void Awake()
        {
            if (TryGetComponent(out NetworkObject net))
            {
                Invoke(nameof(DespawnServerRpc), delay);
            }
            else
            {
                Invoke(nameof(Destroy), delay);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DespawnServerRpc() => GetComponent<NetworkObject>().Despawn(true);

        private void Destroy() => Destroy(gameObject);
    }
}