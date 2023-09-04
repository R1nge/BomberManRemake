using Unity.Netcode;
using UnityEngine;

namespace Misc
{
    public class AutoDestroyNetwork : NetworkBehaviour
    {
        [SerializeField] private float delay;

        private void Awake() => Invoke(nameof(DespawnServerRpc), delay);

        [ServerRpc(RequireOwnership = false)]
        private void DespawnServerRpc() => GetComponent<NetworkObject>().Despawn(true);
    }
}