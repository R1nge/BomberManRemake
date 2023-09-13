using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerFpsCamera : NetworkBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private float sensitivity;
        [SerializeField] private float limitX;
        private float _rotationX, _rotationY;

        private void Start()
        {
            camera.enabled = IsOwner;
            camera.GetComponent<AudioListener>().enabled = IsOwner;
        }

        public void OnLook(InputValue value)
        {
            _rotationX += -value.Get<Vector2>().y * sensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -limitX, limitX);
            _rotationY = value.Get<Vector2>().x * sensitivity;
            Rotate();
        }

        private void Rotate()
        {
            transform.rotation *= Quaternion.Euler(0, _rotationY, 0);
            camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        }
    }
}