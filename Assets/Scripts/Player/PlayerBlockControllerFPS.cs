using Game;
using UnityEngine;

namespace Player
{
    public class PlayerBlockControllerFPS : PlayerBlockController
    {
        [SerializeField] private new Transform camera;

        protected override void Raycast()
        {
            Ray ray = new Ray(camera.position, camera.forward);
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