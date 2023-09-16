using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

namespace Misc
{
    public class RenderOnlyShadowLocally : NetworkBehaviour
    {
        [SerializeField] private new Renderer[] renderer;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                for (int i = 0; i < renderer.Length; i++)
                {
                    renderer[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }
            }
        }
    }
}