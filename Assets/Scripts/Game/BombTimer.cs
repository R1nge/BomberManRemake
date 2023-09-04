using System;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class BombTimer : NetworkBehaviour
    {
        public event Action<float> OnTimeChanged;
        public event Action OnTimeRunOut;
        [SerializeField] private float explodeDelay;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Collider collider1, collider2;
        private NetworkVariable<float> _time;

        public float ExplosionDelay => explodeDelay;

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += Tick;
            _time = new NetworkVariable<float>();
            _time.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(float _, float time)
        {
            OnTimeChanged?.Invoke(time);
            if (Math.Abs(time - explodeDelay) < .01f)
            {
                if (IsServer)
                {
                    OnTimeRunOut?.Invoke();
                }
                
                //TODO: redo
                meshRenderer.enabled = false;
                collider1.enabled = false;
                collider2.enabled = false;
            }
        }


        private void Tick()
        {
            if (IsServer)
            {
                if (_time.Value < explodeDelay)
                {
                    _time.Value += 1f / NetworkManager.Singleton.NetworkTickSystem.TickRate;
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick -= Tick;
        }
    }
}