using UnityEngine;

namespace Misc
{
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField] private float delay;

        private void Awake() => Destroy(gameObject, delay);
    }
}