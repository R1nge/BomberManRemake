using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class BombColor : NetworkBehaviour
    {
        [SerializeField] private Color explosionColor;
        [SerializeField] private MeshRenderer meshRenderer;
        private BombTimer _bombTimer;

        private void Awake()
        {
            _bombTimer = GetComponent<BombTimer>();
            _bombTimer.OnTimeChanged += UpdateColor;
        }

        private void UpdateColor(float time)
        {
            var delta = 1f / NetworkManager.Singleton.NetworkTickSystem.TickRate;
            UpdateColorClientRpc(explosionColor, (time / _bombTimer.ExplosionDelay) * delta);
        }

        [ClientRpc]
        private void UpdateColorClientRpc(Color color, float lerp)
        {
            meshRenderer.material.color = Color.Lerp(meshRenderer.materials[0].color, color, lerp);
        }

        public override void OnDestroy()
        {
            _bombTimer.OnTimeChanged -= UpdateColor;
        }
    }
}