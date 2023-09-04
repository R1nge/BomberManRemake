using System;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class Bomb : NetworkBehaviour, IDamageable
    {
        [SerializeField] private Collider triggerCollider, collider;
        [SerializeField] private MapPreset preset;

        public void TakeDamage(int amount)
        {
            Explode();
        }

        private void Explode()
        {
            var position = transform.position;
            Raycast(position, Vector3.forward, 2, 2);
            Raycast(position, Vector3.back, 2, 2);
            Raycast(position, Vector3.left, 2, 2);
            Raycast(position, Vector3.right, 2, 2);
        }

        private void Raycast(Vector3 pos, Vector3 dir, int dist, float rad)
        {
            Ray ray = new Ray(pos, dir);
            if (Physics.SphereCast(ray, rad, out var hit, dist * preset.Size))
            {
                if (hit.transform.TryGetComponent(out NetworkObject net))
                {
                }
                else
                {
                }
            }
            else
            {
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
    }
}