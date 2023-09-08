using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Player
{
    public class PlayerFpsCamera : NetworkBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private float sensitivity;
        [SerializeField] private float limitX;
        private float _rotationX, _rotationY;

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += Rotate;
        }

        private void Start()
        {
            camera.enabled = IsOwner;
            camera.GetComponent<AudioListener>().enabled = IsOwner;
        }

        public void OnLook(InputValue value)
        {
            if (!IsOwner) return;
            _rotationX += -value.Get<Vector2>().y * sensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -limitX, limitX);
            _rotationY = value.Get<Vector2>().x * sensitivity;
        }

        private void Rotate()
        {
            if (!IsOwner) return;
            transform.rotation *= Quaternion.Euler(0, _rotationY, 0);
            camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        }

        [ServerRpc]
        private void RotateServerRpc(float rotationX)
        {
            transform.rotation *= Quaternion.Euler(0, rotationX, 0);
        }

        public override void OnDestroy()
        {
            if (!NetworkManager.Singleton) return;
            NetworkManager.Singleton.NetworkTickSystem.Tick -= Rotate;
        }
    }
}