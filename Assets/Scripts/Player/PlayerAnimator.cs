using Unity.Netcode.Components;
using UnityEngine;

namespace Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private NetworkAnimator animator;
        private static readonly int Speed = Animator.StringToHash("Speed");

        public void Move(float speed)
        {
            animator.Animator.SetFloat(Speed, speed);
        }
    }
}