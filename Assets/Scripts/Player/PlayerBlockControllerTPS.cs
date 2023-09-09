using Game;
using UnityEngine;

namespace Player
{
    public class PlayerBlockControllerTPS : PlayerBlockController
    {
        [SerializeField] private Transform model;

        protected override void Raycast()
        {
            Ray ray = new Ray(model.position, model.forward);
            if (Physics.Raycast(ray, out var hit, distance))
            {
                if (hit.transform.TryGetComponent(out Destructable destructable))
                {
                    DestroyBlockServerRpc(destructable.gameObject);
                }
            }
        }
    }
}