using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private float speed;
        private CharacterController _characterController;
        private NetworkVariable<bool> _canMove;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _canMove = new NetworkVariable<bool>();
        }

        public void OnMove(InputValue value)
        {
            if (!IsOwner) return;
            MoveServerRpc(value.Get<Vector2>());
        }

        [ServerRpc]
        private void MoveServerRpc(Vector3 direction)
        {
            transform.position += direction;
        }
    }
}