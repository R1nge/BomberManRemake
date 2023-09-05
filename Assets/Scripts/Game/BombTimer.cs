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