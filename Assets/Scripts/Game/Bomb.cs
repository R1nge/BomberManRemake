using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Bomb : NetworkBehaviour, IDamageable
    {
        public event Action<Bomb> OnExplosion;
        [SerializeField] private NetworkObject soundPrefab;
        [SerializeField] private Collider triggerCollider, secondCollider;
        [SerializeField] private MapPreset preset;
        [SerializeField] private LayerMask ignore;
        private const int DAMAGE = 1;
        private const int CASTDISTANCE = 10;
        private const float SPHERECASTRADIUS = .25f;
        private NetworkVariable<bool> _exploded;
        private SpawnerOnGrid _spawnerOnGrid;
        private BombTimer _bombTimer;

        [Inject]
        private void Inject(SpawnerOnGrid spawnerOnGrid) => _spawnerOnGrid = spawnerOnGrid;

        private void Awake()
        {
            _bombTimer = GetComponent<BombTimer>();
            _bombTimer.OnTimeRunOut += OnTimeRunOut;
            _exploded = new NetworkVariable<bool>();
        }

        private void OnTimeRunOut() => Explode();

        public void TakeDamage(int amount, ulong killerId) => Explode();

        private void Explode()
        {
            if (_exploded.Value) return;
            _exploded.Value = true;
            var position = transform.position;
            SpawnSoundServerRpc();
            _spawnerOnGrid.SpawnBombVfx(position);
            Raycast(position, Vector3.forward, CASTDISTANCE, SPHERECASTRADIUS);
            Raycast(position, Vector3.back, CASTDISTANCE, SPHERECASTRADIUS);
            Raycast(position, Vector3.left, CASTDISTANCE, SPHERECASTRADIUS);
            Raycast(position, Vector3.right, CASTDISTANCE, SPHERECASTRADIUS);
            DoDamageInside();
            
            OnExplosion?.Invoke(this);
            NetworkObject.Despawn(true);
        }

        private void Raycast(Vector3 pos, Vector3 dir, int dist, float rad)
        {
            Ray ray = new Ray(pos, dir);
            if (Physics.SphereCast(ray, rad, out var hit, dist * preset.Size, ~ignore))
            {
                if (hit.transform.TryGetComponent(out NetworkObject net))
                {
                    var amount = Mathf.FloorToInt((hit.distance + preset.Size) / preset.Size);

                    if (hit.transform.TryGetComponent(out IDamageable damageable))
                    {
                        amount += 1;
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
        private void SpawnSoundServerRpc()
        {
            var sound = Instantiate(soundPrefab, transform.position, Quaternion.identity);
            sound.Spawn(true);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DoDamageServerRpc(NetworkObjectReference netRef, int damage)
        {
            if (netRef.TryGet(out NetworkObject net))
            {
                if (net.transform.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage, NetworkObject.OwnerClientId);
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
                        damageable.TakeDamage(DAMAGE, NetworkObject.OwnerClientId);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other) => SwitchCollider();

        private void SwitchCollider()
        {
            secondCollider.enabled = true;
            triggerCollider.enabled = false;
        }

        public override void OnDestroy() => _bombTimer.OnTimeRunOut -= OnTimeRunOut;
    }
}