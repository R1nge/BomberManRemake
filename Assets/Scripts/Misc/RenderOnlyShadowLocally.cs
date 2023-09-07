using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

namespace Misc
{
    public class RenderOnlyShadowLocally : NetworkBehaviour
    {
        [SerializeField] private new Renderer renderer;
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }
    }
}