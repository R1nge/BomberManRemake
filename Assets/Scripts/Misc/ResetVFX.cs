using UnityEngine;

namespace Misc
{
    public class ResetVFX : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem;

        private void OnEnable()
        {
            particleSystem.time = 0;
            particleSystem.Play();
        }
    }
}