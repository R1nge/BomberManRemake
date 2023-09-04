using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Bomb : NetworkBehaviour, IDamageable
    {
        public event Action<Bomb> OnExplosion;
        [SerializeField] private Collider triggerCollider, collider;
        [SerializeField] private MapPreset preset;
        [SerializeField] private LayerMask ignore;
        private const int DAMAGE = 1;
        private NetworkVariable<bool> _exploded;
        private SpawnerOnGrid _spawnerOnGrid;
        private BombTimer _bombTimer;

        [Inject]
        private void Inject(SpawnerOnGrid spawnerOnGrid)
        {
            _spawnerOnGrid = spawnerOnGrid;
        }

        private void Awake()
        {
            _bombTimer = GetComponent<BombTimer>();
            _bombTimer.OnTimeRunOut += OnTimeRunOut;
            _exploded = new NetworkVariable<bool>();
        }

        private void OnTimeRunOut() => Explode();

        public void TakeDamage(int amount) => Explode();

        private void Explode()
        {
            if (_exploded.Value) return;
            _exploded.Value = true;
            var position = transform.position;
            _spawnerOnGrid.SpawnBombVfx(position);
            Raycast(position, Vector3.forward, 10, 2);
            Raycast(position, Vector3.back, 10, 2);
            Raycast(position, Vector3.left, 10, 2);
            Raycast(position, Vector3.right, 10, 2);
            DoDamageInside();
            OnExplosion?.Invoke(this);
        }

        private void Raycast(Vector3 pos, Vector3 dir, int dist, float rad)
        {
            Ray ray = new Ray(pos, dir);
            // if (Physics.SphereCast(ray, rad, out var hit, dist * preset.Size))
            // {
            //     if (hit.transform.TryGetComponent(out NetworkObject net))
            //     {
            //         DoDamageServerRpc(net, DAMAGE);
            //     }
            // }

            if (Physics.Raycast(ray, out var hit, dist * preset.Size, ~ignore))
            {
                if (hit.transform.TryGetComponent(out NetworkObject net))
                {
                    var amount = Mathf.FloorToInt((hit.distance + preset.Size) / preset.Size);

                    if (hit.transform.TryGetComponent(out IDamageable damageable))
                    {
                        amount += 1;
                        print($"DAMAGED {net.gameObject.name}");
                        DoDamageServerRpc(net, DAMAGE);
                    }

                    SpawnExplosionVfx(dir, amount);
                }
            }
        }

        private void SpawnExplosionVfx(Vector3 dir, int amount)
        {
            for (int i = 1; i < amount; i++)
            {
                _spawnerOnGrid.SpawnBombVfx(transform.position + dir * i * preset.Size);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DoDamageServerRpc(NetworkObjectReference netRef, int damage)
        {
            if (netRef.TryGet(out NetworkObject net))
            {
                if (net.transform.TryGetComponent(out IDamageable damageable))
                {
                    print($"DAMAGE {net.gameObject.name}");
                    damageable.TakeDamage(damage);
                }
            }
        }

        private void DoDamageInside()
        {
            var coll = new Collider[4];

            var size = Physics.OverlapBoxNonAlloc(transform.position, transform.localScale / 4, coll,
                Quaternion.identity);
            for (int i = 0; i < size; i++)
            {
                if (coll[i].TryGetComponent(out IDamageable damageable))
                {
                    if (coll[i].TryGetComponent(out NetworkObject obj))
                    {
                        if (obj == null || !obj.IsSpawned || obj == NetworkObject) return;
                        damageable.TakeDamage(DAMAGE);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsServer) return;
            SwitchCollider();
        }

        private void SwitchCollider()
        {
            collider.enabled = true;
            triggerCollider.enabled = false;
        }

        public override void OnDestroy()
        {
            _bombTimer.OnTimeRunOut -= OnTimeRunOut;
        }
    }
}