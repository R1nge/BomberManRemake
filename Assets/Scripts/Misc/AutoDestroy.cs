using Unity.Netcode;
using UnityEngine;

namespace Misc
{
    public class AutoDestroy : NetworkBehaviour
    {
        [SerializeField] private float delay;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            Invoke(nameof(Destroy), delay);
        }

        private void Destroy() => NetworkObject.Despawn(true);
    }
}